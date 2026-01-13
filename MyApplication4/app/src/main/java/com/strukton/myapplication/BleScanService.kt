package com.strukton.myapplication

import android.annotation.SuppressLint
import android.app.Notification
import android.app.NotificationChannel
import android.app.NotificationManager
import android.app.Service
import android.bluetooth.BluetoothAdapter
import android.bluetooth.BluetoothManager
import android.content.Intent
import android.content.IntentFilter
import android.hardware.Sensor
import android.hardware.SensorEvent
import android.hardware.SensorEventListener
import android.hardware.SensorManager
import android.os.BatteryManager
import android.os.Build
import android.os.IBinder
import android.os.PowerManager
import android.util.Log
import androidx.core.app.NotificationCompat
import com.google.android.gms.location.LocationServices
import kotlinx.coroutines.*
import no.nordicsemi.android.support.v18.scanner.*
import java.text.SimpleDateFormat
import java.util.*

class BleScanService : Service() {

    private val serviceScope = CoroutineScope(Dispatchers.IO + SupervisorJob())
    private lateinit var wakeLock: PowerManager.WakeLock

    private lateinit var sensorManager: SensorManager
    private var accelerometer: Sensor? = null
    private var tril = false
    private var lastX = 0f
    private var lastY = 0f
    private var lastZ = 0f
    private val shakeThreshold = 8f

    private var bluetoothAdapter: BluetoothAdapter? = null
    private val nordicScanner: BluetoothLeScannerCompat by lazy {
        BluetoothLeScannerCompat.getScanner()
    }

    private val scanSettings = ScanSettings.Builder()
        .setScanMode(ScanSettings.SCAN_MODE_LOW_LATENCY)
        .setReportDelay(0)
        .setUseHardwareBatchingIfSupported(false)
        .build()

    private val scanFilters: List<ScanFilter> = emptyList()

    private val deviceBuffer = mutableListOf<TagFullDto>()
    private var tagCounter = 0

    private val scanInterval = 10_000L
    private val scanPause = 50_000L
    private val sendInterval = 50_000L

    override fun onCreate() {
        super.onCreate()

        Log.i("BLE_SERVICE", "BleScanService gestart!")

        acquireWakeLock()
        initSensors()
        initBluetooth()

        startForeground(1, buildNotification())

        startScanningLoop()
        startSendingLoop()
    }

    private fun acquireWakeLock() {
        val pm = getSystemService(POWER_SERVICE) as PowerManager
        wakeLock = pm.newWakeLock(
            PowerManager.PARTIAL_WAKE_LOCK,
            "BleScanService::WakeLock"
        )
        wakeLock.acquire()
        Log.i("BLE_WAKELOCK", "WakeLock actief")
    }

    private fun initSensors() {
        sensorManager = getSystemService(SENSOR_SERVICE) as SensorManager
        accelerometer = sensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER)
        accelerometer?.also {
            sensorManager.registerListener(accelListener, it, SensorManager.SENSOR_DELAY_GAME)
        }
        Log.i("BLE_SENSOR", "Accelerometer listener gestart")
    }

    private fun initBluetooth() {
        val bluetoothManager = getSystemService(BLUETOOTH_SERVICE) as BluetoothManager
        bluetoothAdapter = bluetoothManager.adapter
        Log.i("BLE_SERVICE", "Bluetooth enabled = ${bluetoothAdapter?.isEnabled}")
    }

    private fun buildNotification(): Notification {
        val channelId = "BLE_SCAN_CHANNEL"
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            val channel = NotificationChannel(
                channelId,
                "BLE Scan",
                NotificationManager.IMPORTANCE_LOW
            )
            getSystemService(NotificationManager::class.java)
                .createNotificationChannel(channel)
        }

        return NotificationCompat.Builder(this, channelId)
            .setContentTitle("BLE scanner draait")
            .setContentText("App scant BLE-apparaten op de achtergrond")
            .setSmallIcon(R.mipmap.ic_launcher)
            .build()
    }

    private val accelListener = object : SensorEventListener {
        override fun onSensorChanged(e: SensorEvent?) {
            e ?: return

            val dx = kotlin.math.abs(e.values[0] - lastX)
            val dy = kotlin.math.abs(e.values[1] - lastY)
            val dz = kotlin.math.abs(e.values[2] - lastZ)

            if (dx + dy + dz > shakeThreshold) {
                tril = true
                Log.i("BLE_SHAKE", "Trilling gedetecteerd! ΔX=$dx ΔY=$dy ΔZ=$dz")
            }

            lastX = e.values[0]
            lastY = e.values[1]
            lastZ = e.values[2]
        }

        override fun onAccuracyChanged(sensor: Sensor?, accuracy: Int) {}
    }

    @SuppressLint("MissingPermission")
    private val scanCallback = object : ScanCallback() {
        override fun onScanResult(type: Int, res: ScanResult) {
            val name = res.device.name ?: res.scanRecord?.deviceName ?: "Onbekend"
            val rssi = res.rssi

            Log.i("BLE_SCAN_RESULT", "Gevonden: $name | RSSI=$rssi")

            val timestamp = SimpleDateFormat("yyyy-MM-dd HH:mm:ss", Locale.getDefault()).format(Date())

            val scanRecord = res.scanRecord
            val manuf = scanRecord?.manufacturerSpecificData

            var beaconUuid: String? = null
            var major: Int? = null
            var minor: Int? = null

            if (manuf != null && manuf.size() > 0) {
                for (i in 0 until manuf.size()) {
                    val data = manuf.valueAt(i)
                    if (data != null && data.size >= 23 &&
                        data[0] == 0x02.toByte() &&
                        data[1] == 0x15.toByte()
                    ) {
                        val uuidBytes = data.copyOfRange(2, 18)
                        beaconUuid = String.format(
                            "%02X%02X%02X%02X-%02X%02X-%02X%02X-%02X%02X-%02X%02X%02X%02X%02X%02X",
                            *uuidBytes.map { it.toInt() and 0xFF }.toTypedArray()
                        )
                        major = ((data[18].toInt() and 0xFF) shl 8) or (data[19].toInt() and 0xFF)
                        minor = ((data[20].toInt() and 0xFF) shl 8) or (data[21].toInt() and 0xFF)
                    }
                }
            }
            //if (beaconUuid!="E2C56DB5-DFFB-48D2-B060-D0F5A71096E0" && name !="easiBeacon_13o") return

            val batteryStatus = registerReceiver(null, IntentFilter(Intent.ACTION_BATTERY_CHANGED))
            val level = batteryStatus?.getIntExtra(BatteryManager.EXTRA_LEVEL, -1) ?: -1
            val scale = batteryStatus?.getIntExtra(BatteryManager.EXTRA_SCALE, -1) ?: -1
            val batteryPct = if (level > 0 && scale > 0) level * 100 / scale else -1

            val fused = LocationServices.getFusedLocationProviderClient(this@BleScanService)
            fused.lastLocation.addOnSuccessListener { loc ->
                val beaconId = BeaconIdManager.getBeaconId(this@BleScanService, name)
                tagCounter++

                deviceBuffer.add(
                    TagFullDto(
                        version = "1.0",
                        id = beaconId.toString(),
                        name = name,
                        rssi = rssi,
                        latitude = loc?.latitude,
                        longitude = loc?.longitude,
                        accuracyMeters = loc?.accuracy,
                        timestamp = timestamp,
                        battery = batteryPct,
                        sequenceNumber = tagCounter,
                        trilling = tril,
                        log = "",
                        minor = minor
                    )
                )

                Log.i("BLE_BUFFER", "Toegevoegd: $name → ID=$beaconId (buffer=${deviceBuffer.size})")
            }
        }
    }

    private fun startScanningLoop() {
        serviceScope.launch {
            while (isActive) {
                Log.i("BLE_SCAN", "Start scan (Nordic)...")
                try {
                    nordicScanner.startScan(scanFilters, scanSettings, scanCallback)
                } catch (e: Exception) {
                    Log.e("BLE_SCAN_ERROR", "StartScan fout: ${e.message}")
                }

                delay(scanInterval)

                Log.i("BLE_SCAN", "Stop scan (Nordic)...")
                try {
                    nordicScanner.stopScan(scanCallback)
                } catch (e: Exception) {
                    Log.e("BLE_SCAN_ERROR", "StopScan fout: ${e.message}")
                }

                delay(scanPause)
            }
        }
    }

    private fun startSendingLoop() {
        serviceScope.launch {
            while (isActive) {
                sendBuffered()
                delay(sendInterval)
            }
        }
    }

    private suspend fun sendBuffered() {
        if (deviceBuffer.isEmpty()) {
            Log.d("BLE_BUFFER", "Buffer leeg, niets te versturen.")
            return
        }

        val toSend = deviceBuffer.toList()
        deviceBuffer.clear()
        tril = false

        Log.i("BLE_SEND", "Versturen van ${toSend.size} items...")

        for (dto in toSend) {
            try {
                ApiClient.createTag(dto)
                Log.i("BLE_SEND", "Verzonden: ${dto.name} (seq=${dto.sequenceNumber})")
            } catch (e: Exception) {
                Log.e("BLE_SEND_ERROR", "Fout bij verzenden: ${e.message}")
            }
        }
    }

    override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int {
        return START_STICKY
    }

    override fun onDestroy() {
        try {
            nordicScanner.stopScan(scanCallback)
        } catch (_: Exception) {}

        sensorManager.unregisterListener(accelListener)
        serviceScope.coroutineContext.cancel()

        if (this::wakeLock.isInitialized && wakeLock.isHeld) {
            wakeLock.release()
        }

        Log.w("BLE_SERVICE", "BleScanService gestopt.")
        super.onDestroy()
    }

    override fun onBind(intent: Intent?): IBinder? = null
}

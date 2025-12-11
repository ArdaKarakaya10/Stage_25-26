package com.strukton.myapplication

import android.Manifest
import android.annotation.SuppressLint
import android.app.Notification
import android.app.NotificationChannel
import android.app.NotificationManager
import android.app.Service
import android.bluetooth.BluetoothAdapter
import android.bluetooth.BluetoothManager
import android.bluetooth.le.BluetoothLeScanner
import android.bluetooth.le.ScanCallback
import android.bluetooth.le.ScanResult
import android.content.Intent
import android.content.pm.PackageManager
import android.os.BatteryManager
import android.os.Build
import android.os.Handler
import android.os.IBinder
import android.os.Looper
import android.util.Log
import androidx.core.app.ActivityCompat
import androidx.core.app.NotificationCompat
import com.strukton.myapplication.ApiClient
import com.strukton.myapplication.TagFullDto
import com.google.android.gms.location.FusedLocationProviderClient
import com.google.android.gms.location.LocationServices
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import java.text.SimpleDateFormat
import java.util.Date
import java.util.Locale
import kotlin.math.pow
import android.hardware.Sensor
import android.hardware.SensorEvent
import android.hardware.SensorEventListener
import android.hardware.SensorManager



class BleScanService : Service() {
    private lateinit var sensorManager: SensorManager
    private var accelerometer: Sensor? = null
    private var lastX = 0f
    private var lastY = 0f
    private var lastZ = 0f
    private var shakeThreshold = 8f
    private var tril = false

    private var bluetoothAdapter: BluetoothAdapter? = null
    private var scanner: BluetoothLeScanner? = null
    private lateinit var fusedLocationClient: FusedLocationProviderClient

    private val deviceBuffer = mutableListOf<TagFullDto>()
    private val bufferHandler = Handler(Looper.getMainLooper())
    private val sendInterval: Long = 10_000

    private val scanHandler = Handler(Looper.getMainLooper())
    private val scanInterval: Long = 10_000
    private val scanPause: Long = 5_000
    private var tagCounter = 0


    override fun onCreate() {
        super.onCreate()
        Log.i("BLE_SERVICE", "BleScanService gestart!")

        sensorManager = getSystemService(SENSOR_SERVICE) as SensorManager
        accelerometer = sensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER)
        accelerometer?.also { sensor ->
            sensorManager.registerListener(accelerometerListener, sensor, SensorManager.SENSOR_DELAY_GAME)
        }

        val bluetoothManager = getSystemService(BLUETOOTH_SERVICE) as BluetoothManager
        bluetoothAdapter = bluetoothManager.adapter
        fusedLocationClient = LocationServices.getFusedLocationProviderClient(this)

        startForeground(1, buildNotification())

        Log.i("BLE_SERVICE", "Start BLE scanner...")
        startPeriodicScan()

        Log.i("BLE_SERVICE", "Start buffer-verstuurder...")
        startBufferSender()
    }

    private fun buildNotification(): Notification {
        val channelId = "BLE_SCAN_CHANNEL"
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            val channel = NotificationChannel(channelId, "BLE Scan", NotificationManager.IMPORTANCE_LOW)
            getSystemService(NotificationManager::class.java).createNotificationChannel(channel)
        }
        return NotificationCompat.Builder(this, channelId)
            .setContentTitle("BLE scanner draait")
            .setContentText("App scant BLE-apparaten op de achtergrond")
            .setSmallIcon(R.mipmap.ic_launcher)
            .build()
    }
    private val accelerometerListener = object : SensorEventListener {
        override fun onSensorChanged(event: SensorEvent?) {
            event ?: return
            val x = event.values[0]
            val y = event.values[1]
            val z = event.values[2]

            val deltaX = Math.abs(x - lastX)
            val deltaY = Math.abs(y - lastY)
            val deltaZ = Math.abs(z - lastZ)

            if (deltaX + deltaY + deltaZ > shakeThreshold) {
                tril=true
                Log.i("BLE_SHAKE", "Trilling gedetecteerd! ΔX=$deltaX ΔY=$deltaY ΔZ=$deltaZ")
            }

            lastX = x
            lastY = y
            lastZ = z
        }

        override fun onAccuracyChanged(sensor: Sensor?, accuracy: Int) {}
    }

    @SuppressLint("MissingPermission")
    private val scanCallback = object : ScanCallback() {
        override fun onScanResult(callbackType: Int, res: ScanResult?) {
            res ?: return

            val device = res.device
            val name = device.name ?: res.scanRecord?.deviceName ?: "Onbekend"
            //if (name != "CP27-279A" && name != "CP28-8EBC" && name !="easiBeacon_13o") return

            //Log.d("BLE_SCAN", "BLE gevonden: $name | RSSI=${res.rssi}")

            val rssi = res.rssi
            val timestamp = SimpleDateFormat("yyyy-MM-dd HH:mm:ss", Locale.getDefault()).format(Date())

            val scanRecord = res.scanRecord
            val manuf = scanRecord?.manufacturerSpecificData

            var beaconUuid: String? = null
            var major: Int? = null
            var minor: Int? = null

            if (manuf != null && manuf.size() > 0) {
                for (i in 0 until manuf.size()) {
                    val data = manuf.valueAt(i)
                    if (data != null && data.size >= 23 && data[0] == 0x02.toByte() && data[1] == 0x15.toByte()) {
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

            if (beaconUuid!="E2C56DB5-DFFB-48D2-B060-D0F5A71096E0" && name !="easiBeacon_13o") return


            val batteryStatus = registerReceiver(null, android.content.IntentFilter(Intent.ACTION_BATTERY_CHANGED))
            val level = batteryStatus?.getIntExtra(BatteryManager.EXTRA_LEVEL, -1) ?: -1
            val scale = batteryStatus?.getIntExtra(BatteryManager.EXTRA_SCALE, -1) ?: -1
            val batteryPct = if (level > 0 && scale > 0) (level * 100 / scale) else -1

            fusedLocationClient.lastLocation.addOnSuccessListener { location ->
                val lat = location?.latitude
                val lon = location?.longitude
                val acc = location?.accuracy

                val beaconId = BeaconIdManager.getBeaconId(this@BleScanService, name)
                tagCounter++

                deviceBuffer.add(
                    TagFullDto(
                        version = "1.0",
                        id = beaconId.toString(),
                        name = name,
                        rssi = rssi,
                        latitude = lat,
                        longitude = lon,
                        accuracyMeters = acc,
                        timestamp = timestamp,
                        battery = batteryPct,
                        sequenceNumber = tagCounter,
                        trilling =tril,
                        log = "",
                        minor=minor
                    )
                )

                Log.i("BLE_BUFFER", "Beacon: $name → ID=$beaconId (buffer=${deviceBuffer.size},seq=$tagCounter))")
            }
        }
    }


    private fun startPeriodicScan() {
        scanner = bluetoothAdapter?.bluetoothLeScanner ?: return

        val scanRunnable = object : Runnable {
            override fun run() {
                Log.i("BLE_SCAN", "Start scan...")
                scanner?.startScan(scanCallback)

                scanHandler.postDelayed({
                    Log.i("BLE_SCAN", "Stop scan...")
                    scanner?.stopScan(scanCallback)

                    scanHandler.postDelayed(this, scanPause)
                }, scanInterval)
            }
        }

        scanHandler.post(scanRunnable)
    }

    private fun startBufferSender() {
        val senderRunnable = object : Runnable {
            override fun run() {
                sendBufferedDevices()
                bufferHandler.postDelayed(this, sendInterval)
            }
        }
        bufferHandler.post(senderRunnable)
    }

    private fun sendBufferedDevices() {
        if (deviceBuffer.isEmpty()) {
            Log.d("BLE_BUFFER", "Buffer leeg, niets te versturen.")
            return
        }

        val toSend = deviceBuffer.toList()
        deviceBuffer.clear()
        tril = false

        Log.i("BLE_BUFFER", "Versturen van ${toSend.size} items...")

        CoroutineScope(Dispatchers.IO).launch {
            for (dto in toSend) {
                try {
                    ApiClient.createTag(dto)
                } catch (ex: Exception) {
                    Log.e("API_ERROR", "Fout bij verzenden: ${ex.message}", ex)
                }
            }
        }
    }



    override fun onDestroy() {
        scanner?.stopScan(scanCallback)
        scanHandler.removeCallbacksAndMessages(null)
        bufferHandler.removeCallbacksAndMessages(null)
        sensorManager.unregisterListener(accelerometerListener)
        Log.w("BLE_SERVICE", "BleScanService gestopt.")
        super.onDestroy()
    }

    override fun onBind(intent: Intent?): IBinder? = null
}

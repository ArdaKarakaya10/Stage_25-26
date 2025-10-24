package com.example.blescannerapp

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
import android.content.pm.PackageManager
import android.content.Intent
import android.os.Build
import android.os.Handler
import android.os.IBinder
import android.os.Looper
import android.util.Log
import androidx.core.app.ActivityCompat
import androidx.core.app.NotificationCompat
import com.google.android.gms.location.FusedLocationProviderClient
import com.google.android.gms.location.LocationServices
import kotlin.math.pow

class BleScanService : Service() {

    private var bluetoothAdapter: BluetoothAdapter? = null
    private var scanner: BluetoothLeScanner? = null
    private lateinit var fusedLocationClient: FusedLocationProviderClient
    private val rssiBuffer = mutableListOf<Int>()

    private val scanHandler = Handler(Looper.getMainLooper())
    private val scanInterval: Long = 10000 // 10 sec scan
    private val scanPause: Long = 5000 // 5 sec pauze

    override fun onCreate() {
        super.onCreate()

        val bluetoothManager = getSystemService(BLUETOOTH_SERVICE) as BluetoothManager
        bluetoothAdapter = bluetoothManager.adapter
        fusedLocationClient = LocationServices.getFusedLocationProviderClient(this)

        startForeground(1, buildNotification())

        startPeriodicScan()
    }

    private fun buildNotification(): Notification {
        val channelId = "BLE_SCAN_CHANNEL"
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            val channel = NotificationChannel(channelId, "BLE Scan", NotificationManager.IMPORTANCE_LOW)
            getSystemService(NotificationManager::class.java).createNotificationChannel(channel)
        }
        return NotificationCompat.Builder(this, channelId)
            .setContentTitle("BLE Scanner actief")
            .setContentText("Scant naar BLE-apparaten op de achtergrond")
            .setSmallIcon(R.mipmap.ic_launcher)
            .build()
    }

    private val scanCallback = object : ScanCallback() {
        @SuppressLint("MissingPermission")
        override fun onScanResult(callbackType: Int, result: ScanResult?) {
            result?.let { res ->
                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
                    if (ActivityCompat.checkSelfPermission(this@BleScanService, Manifest.permission.BLUETOOTH_CONNECT) != PackageManager.PERMISSION_GRANTED ||
                        ActivityCompat.checkSelfPermission(this@BleScanService, Manifest.permission.BLUETOOTH_SCAN) != PackageManager.PERMISSION_GRANTED) return
                }

                if (ActivityCompat.checkSelfPermission(this@BleScanService, Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED &&
                    ActivityCompat.checkSelfPermission(this@BleScanService, Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED) return

                fusedLocationClient.lastLocation.addOnSuccessListener { location ->
                    val latitude = location?.latitude ?: "Onbekend"
                    val longitude = location?.longitude ?: "Onbekend"

                    val device = res.device
                    val scanRecord = res.scanRecord

                    val name = device.name ?: scanRecord?.deviceName ?: "Onbekend"
                    val address = device.address
                    val rssiRaw = res.rssi
                    val rssi = smoothRssi(rssiRaw)
                    val distance = rssiToDistance(rssiRaw)

                    val connectable = if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) res.isConnectable else null

                    val readableTime = java.text.SimpleDateFormat(
                        "yyyy-MM-dd HH:mm:ss",
                        java.util.Locale.getDefault()
                    ).format(java.util.Date())

                    val manufacturerData = scanRecord?.getManufacturerSpecificData(0x004C)
                    val beaconInfo = if (manufacturerData != null && manufacturerData.size >= 23) {
                        val uuidBytes = manufacturerData.copyOfRange(2, 18)
                        val majorBytes = manufacturerData.copyOfRange(18, 20)
                        val minorBytes = manufacturerData.copyOfRange(20, 22)

                        val uuid = uuidBytes.joinToString("") { String.format("%02X", it) }
                            .replace(Regex("(.{8})(.{4})(.{4})(.{4})(.{12})"), "$1-$2-$3-$4-$5")
                        val major = ((majorBytes[0].toInt() and 0xFF) shl 8) + (majorBytes[1].toInt() and 0xFF)
                        val minor = ((minorBytes[0].toInt() and 0xFF) shl 8) + (minorBytes[1].toInt() and 0xFF)

                        "iBeacon -> UUID: $uuid, Major: $major, Minor: $minor"
                    } else {
                        "Geen iBeacon UUID aanwezig"
                    }

                    Log.i(
                        "BLE_SERVICE",
                        """
                        === Nieuw apparaat gevonden ===
                        Naam: $name
                        Adres: $address
                        RSSI: $rssi dBm (raw: $rssiRaw)
                        Afstand: %.2f meter
                        Connectable: ${connectable ?: "N/A"}
                        Tijd: $readableTime
                        Locatie: Latitude $latitude, Longitude $longitude
                        $beaconInfo
                        ==================================
                        """.trimIndent().format(distance)
                    )
                }
            }
        }

        override fun onScanFailed(errorCode: Int) {
            Log.e("BLE_SERVICE", "Scan mislukt: $errorCode")
        }
    }

    @SuppressLint("MissingPermission")
    private fun startPeriodicScan() {
        scanner = bluetoothAdapter?.bluetoothLeScanner
        if (scanner == null) {
            Log.e("BLE_SERVICE", "BLE scanner niet beschikbaar")
            return
        }

        val scanRunnable = object : Runnable {
            override fun run() {
                scanner?.startScan(scanCallback)
                scanHandler.postDelayed({
                    scanner?.stopScan(scanCallback)
                    scanHandler.postDelayed(this, scanPause) // pauze tussen scans
                }, scanInterval)
            }
        }
        scanHandler.post(scanRunnable)
    }

    private fun rssiToDistance(rssi: Int, txPower: Int = -59, n: Double = 2.0): Double {
        return 10.0.pow((txPower - rssi) / (10 * n))
    }

    private fun smoothRssi(newRssi: Int, windowSize: Int = 5): Int {
        if (rssiBuffer.size >= windowSize) rssiBuffer.removeAt(0)
        rssiBuffer.add(newRssi)
        return rssiBuffer.average().toInt()
    }

    override fun onDestroy() {
        scanner?.stopScan(scanCallback)
        scanHandler.removeCallbacksAndMessages(null)
        super.onDestroy()
    }

    override fun onBind(intent: Intent?): IBinder? = null
}

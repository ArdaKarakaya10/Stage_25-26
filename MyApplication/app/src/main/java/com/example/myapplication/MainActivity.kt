package com.example.blescannerapp

import android.Manifest
import android.annotation.SuppressLint
import android.app.Activity
import android.bluetooth.BluetoothAdapter
import android.bluetooth.BluetoothManager
import android.bluetooth.le.BluetoothLeScanner
import android.bluetooth.le.ScanCallback
import android.bluetooth.le.ScanResult
import android.content.pm.PackageManager
import android.os.Build
import android.os.Bundle
import android.util.Log
import android.widget.Toast
import androidx.core.app.ActivityCompat
import androidx.core.content.ContextCompat
import android.content.Intent
import android.os.Handler
import android.os.Looper
import com.google.android.gms.location.FusedLocationProviderClient
import com.google.android.gms.location.LocationServices
import kotlin.math.pow

class MainActivity : Activity() {

    private var bluetoothAdapter: BluetoothAdapter? = null
    private var scanner: BluetoothLeScanner? = null
    private lateinit var fusedLocationClient: FusedLocationProviderClient

    private val PERMISSION_REQUEST_CODE = 1
    private val rssiBuffer = mutableListOf<Int>() // voor RSSI-gemiddelde

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        val bluetoothManager = getSystemService(BLUETOOTH_SERVICE) as BluetoothManager
        bluetoothAdapter = bluetoothManager.adapter

        if (bluetoothAdapter == null) {
            Toast.makeText(this, "Bluetooth wordt niet ondersteund op dit apparaat", Toast.LENGTH_LONG).show()
            finish()
            return
        }

        fusedLocationClient = LocationServices.getFusedLocationProviderClient(this)
        checkPermissionsAndStartScan()
    }

    private fun checkPermissionsAndStartScan() {
        val permissions = mutableListOf(
            Manifest.permission.ACCESS_FINE_LOCATION
        )
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
            permissions.add(Manifest.permission.BLUETOOTH_SCAN)
            permissions.add(Manifest.permission.BLUETOOTH_CONNECT)
        }

        val missing = permissions.filter {
            ContextCompat.checkSelfPermission(this, it) != PackageManager.PERMISSION_GRANTED
        }

        if (missing.isNotEmpty()) {
            ActivityCompat.requestPermissions(this, missing.toTypedArray(), PERMISSION_REQUEST_CODE)
        } else {
            startScan()
        }
    }

    @SuppressLint("MissingPermission")
    private fun startScan() {
        if (bluetoothAdapter?.isEnabled == false) {
            startActivityForResult(Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE), 2)
            return
        }

        scanner = bluetoothAdapter?.bluetoothLeScanner
        if (scanner == null) {
            Toast.makeText(this, "BLE-scanner niet beschikbaar", Toast.LENGTH_SHORT).show()
            return
        }

        val scanCallback = object : ScanCallback() {
            override fun onScanResult(callbackType: Int, result: ScanResult?) {
                result?.let { res ->
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
                            "BLE",
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
                Log.e("BLE", "Scan mislukt: $errorCode")
            }
        }

        scanner?.startScan(scanCallback)
        Log.i("BLE", "Scanning gestart...")

        Handler(Looper.getMainLooper()).postDelayed({
            scanner?.stopScan(scanCallback)
            Log.i("BLE", "Scan gestopt")
        }, 10000)
    }

    // --- Helperfuncties ---

    /** Bereken afstand in meters uit RSSI */
    private fun rssiToDistance(rssi: Int, txPower: Int = -59, n: Double = 2.0): Double {
        return 10.0.pow((txPower - rssi) / (10 * n))
    }

    /** Gemiddelde RSSI voor stabielere waarden */
    private fun smoothRssi(newRssi: Int, windowSize: Int = 5): Int {
        if (rssiBuffer.size >= windowSize) rssiBuffer.removeAt(0)
        rssiBuffer.add(newRssi)
        return rssiBuffer.average().toInt()
    }

    override fun onRequestPermissionsResult(
        requestCode: Int,
        permissions: Array<out String>,
        grantResults: IntArray
    ) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults)
        if (requestCode == PERMISSION_REQUEST_CODE) {
            if (grantResults.all { it == PackageManager.PERMISSION_GRANTED }) {
                startScan()
            } else {
                Toast.makeText(this, "Toestemmingen geweigerd — geen scan mogelijk", Toast.LENGTH_LONG).show()
            }
        }
    }
}

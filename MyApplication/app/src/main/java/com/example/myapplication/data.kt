package com.example.blescannerapp.models

data class TagFullDto(
    val id: String? = null,

    val name: String,
    val rssi: Int,
    val distanceMeters: Double?,
    val latitude: Double?,
    val longitude: Double?,
    val accuracyMeters: Float?,
    val timestamp: String,
    val phoneBattery: Int,
    val serviceUuids: List<String>? = null,
    val beaconUuid: String? = null,
    val major: Int? = null,
    val minor: Int? = null,


    )

package com.strukton.myapplication
data class TagFullDto(
    val version: String?,
    val id: String? = null,
    val name: String,
    val rssi: Int,
    val latitude: Double?,
    val longitude: Double?,
    val accuracyMeters: Float?,
    val timestamp: String,
    val battery: Int,
    val sequenceNumber: Int,
    val trilling: Boolean = false,
    val log: String? = null,
    val minor: Int?,
)

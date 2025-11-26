package com.example.blescannerapp

import android.content.Context
import android.content.SharedPreferences

object BeaconIdManager {

    private const val PREF_NAME = "BEACON_IDS"
    private const val KEY_COUNTER = "ID_COUNTER"

    private fun prefs(context: Context): SharedPreferences =
        context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE)

    fun getBeaconId(context: Context, name: String): Int {
        val preferences = prefs(context)

        // bestaat de naam al?
        val existing = preferences.getInt(name, -1)
        if (existing != -1) return existing

        // nieuwe ID toekennen
        val newId = (preferences.getInt(KEY_COUNTER, 0) + 1)

        preferences.edit()
            .putInt(name, newId)
            .putInt(KEY_COUNTER, newId)
            .apply()

        return newId
    }
}

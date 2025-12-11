package com.strukton.myapplication
import android.util.Base64
import android.util.Log
import com.strukton.myapplication.TagFullDto
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.OkHttpClient
import okhttp3.Request
import okhttp3.RequestBody
import org.json.JSONObject
import java.security.MessageDigest
import javax.crypto.Mac
import javax.crypto.spec.SecretKeySpec

object ApiClient {

    private const val BASE_URL = "https://api-tag-test.strukton.com/"
    private const val API_PATH = "api/Tags"
    private const val API_KEY = "demo-key"
    private const val API_SECRET = "demo-secret-CHANGE-ME-LONG-RANDOM"

    private val client = OkHttpClient()

    private fun sha256HexUpper(bytes: ByteArray): String {
        val md = MessageDigest.getInstance("SHA-256")
        return md.digest(bytes).joinToString("") { "%02X".format(it) }
    }

    private fun hmacSha256Base64(message: String, secret: String): String {
        val keySpec = SecretKeySpec(secret.toByteArray(Charsets.UTF_8), "HmacSHA256")
        val mac = Mac.getInstance("HmacSHA256")
        mac.init(keySpec)
        val rawHmac = mac.doFinal(message.toByteArray(Charsets.UTF_8))
        return Base64.encodeToString(rawHmac, Base64.NO_WRAP)
    }

    suspend fun createTag(dto: TagFullDto): TagFullDto {
        val method = "POST"
        val endpoint = API_PATH

        Log.i("API", "➡ Start verzenden van Tag: ${dto.name}")

        val json = JSONObject().apply {
            put("version", dto.version ?: "")
            put("tagID", dto.minor ?: "")
            put("name", dto.name)
            put("rssi", dto.rssi)
            put("latitude", dto.latitude)
            put("longitude", dto.longitude)
            put("accuracyMeters", dto.accuracyMeters)
            put("timestamp", dto.timestamp)
            put("battery", dto.battery)
            put("sequenceNumber", dto.sequenceNumber)
            put("trilling", dto.trilling)
            put("log", dto.log)
        }.toString()

        Log.d("API", "JSON body:\n$json")

        val bodyBytes = json.toByteArray(Charsets.UTF_8)
        val bodyHash = sha256HexUpper(bodyBytes)
        //Log.d("API", "🔐 Body SHA256 = $bodyHash")

        val timestamp = (System.currentTimeMillis() / 1000).toString()
        //Log.d("API", "⏱ Timestamp = $timestamp")

        val canonical = "$method\n/$endpoint\n$timestamp\n$bodyHash"
        // Log.d("API", "📐 Canonical string:\n$canonical")

        val signature = hmacSha256Base64(canonical, API_SECRET)
        //Log.d("API", "✒ Signature (Base64) = $signature")

        val request = Request.Builder()
            .url("$BASE_URL$endpoint")
            .addHeader("X-Api-Key", API_KEY)
            .addHeader("X-Signature", signature)
            .addHeader("X-Timestamp", timestamp)
            .addHeader("X-Body-Hash", bodyHash)
            .post(RequestBody.create("application/json".toMediaType(), json))
            .build()

        Log.i("API", "➡ HTTP request versturen naar /$endpoint")

        val response = client.newCall(request).execute()
        val responseBody = response.body?.string()

        Log.i("API", "⬅ Response code = ${response.code}")
        Log.d("API", "⬅ Response body = $responseBody")

        if (!response.isSuccessful) {
            Log.e("API", "Fout bij verzenden! HTTP ${response.code}")
            throw Exception("HTTP ${response.code}: $responseBody")
        }

        Log.i("API", "Verzenden succesvol")
        return dto
    }
}

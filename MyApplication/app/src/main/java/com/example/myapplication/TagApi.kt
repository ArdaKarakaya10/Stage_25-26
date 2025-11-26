package com.example.blescannerapp.api

import com.example.blescannerapp.models.TagFullDto
import com.example.blescannerapp.models.Tag
import retrofit2.http.Body
import retrofit2.http.POST

interface TagsApi {
    @POST("api/Tags")
    suspend fun createTag(@Body tag: TagFullDto): Tag
}

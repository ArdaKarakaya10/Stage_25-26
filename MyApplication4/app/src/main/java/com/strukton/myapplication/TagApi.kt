package com.strukton.myapplication

import com.strukton.myapplication.TagFullDto
import com.strukton.myapplication.Tag
import retrofit2.http.Body
import retrofit2.http.POST

interface TagsApi {
    @POST("api/Tags")
    suspend fun createTag(@Body tag: TagFullDto): Tag
}

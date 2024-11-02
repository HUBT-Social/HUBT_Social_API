﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace HUBT_Social_API.src.Features.Auth.Dtos.Collections;

[CollectionName("TempUserRegister")]
public class TempUserRegister
{
    [BsonId] [BsonElement("Email")] public string Email { get; set; } = string.Empty;

    [BsonElement("UserName")] public string UserName { get; set; } = string.Empty;


    [BsonElement("Password")] public string Password { get; set; } = string.Empty;

    [BsonElement("ExpireTime")]
    [BsonDateTimeOptions]
    public DateTime ExpireTime { get; set; }
}
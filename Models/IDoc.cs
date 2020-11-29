using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Desafio21diasAPI.Models
{
    public interface IDoc
    {
        [BsonId()]
        public ObjectId Id { get; set; }
    }
}

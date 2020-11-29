using System;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Desafio21diasAPI.Models
{
  public class Cliente : IModel
  // public class Cliente : IDoc
  {
    // [BsonId()]
    // public ObjectId Id { get; set; }
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Telefone { get; set; }
    public string Endereco { get; set; }
    public string Login { get; set; }
    public string Senha { get; set; }

    // [BsonIgnore]
    // [NotMapped]
    public string Token { get; set; }
    public string RegraAcesso { get; set; }
  }
}

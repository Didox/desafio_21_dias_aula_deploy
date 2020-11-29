using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Desafio21diasAPI.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json.Linq;

namespace Desafio21diasAPI.Servicos.Database
{
  public class MongoDbRepositorio : IRepositorio, IRepositorioMongoDb
  {
    public string DadosDoArmazenamento()
    {
      JToken jAppSettings = JToken.Parse(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "appsettings.json")));
      return jAppSettings["mongoCnn"].ToString();
    }

    public void Excluir<T>(ObjectId id)
    {
      documento<T>().DeleteOne(d => ((IDoc)d).Id == id);
    }

    private IMongoCollection<T> documento<T>()
    {
      IMongoClient client = new MongoClient(this.DadosDoArmazenamento());
      var nomeDoBanco = this.DadosDoArmazenamento().Split('/').Last();
      IMongoDatabase database = client.GetDatabase(nomeDoBanco);
      return database.GetCollection<T>((typeof(T)).Name);
    }

    public void Salvar<T>(T modelo)
    {
      var item = (IDoc)modelo;

      if(item.Id == ObjectId.Parse("000000000000000000000000"))
      {
        documento<T>().InsertOne(modelo);
      }
      else
      {
        foreach (var p in modelo.GetType().GetProperties())
        {
          var valor = item.GetType().GetProperty(p.Name).GetValue(item);
          if(valor !=  null)
          {
            var update = Builders<T>.Update.Set(p.Name, valor);
            var filter = Builders<T>.Filter.Eq("_id", item.Id);
            documento<T>().UpdateOne(filter, update);
          }
        }
      }
    }

    public List<T> Todos<T>()
    {
      return documento<T>().AsQueryable().ToList();
    }

    public IMongoQueryable<T> BuscaCriterio<T>()
    {
      return documento<T>().AsQueryable();
    }
    public void RemoverColecao<T>()
    {
        documento<T>().Database.DropCollection(typeof(T).Name);
    }
  }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace Desafio21diasAPI.Servicos.Cache
{
  public class CacheHelper : ICache
  {
    private IDatabase _cache = RedisConnectorHelper.Connection.GetDatabase();

    public void AdicionarListaNoCache<T>(List<T> modelos, string chave, int minutosCache = 0)
    {
      if(minutosCache == 0) minutosCache = minutosCachePadrao();
      var json = JsonSerializer.Serialize(modelos);
      _cache.StringSet(chave, json, TimeSpan.FromMinutes(minutosCache));
    }

    private int minutosCachePadrao()
    {
      JToken jAppSettings = JToken.Parse(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "appsettings.json")));
      return Convert.ToInt32(jAppSettings["redis"]["minutosExpiracao"]);
    }

    public void AdicionarModeloNoCache<T>(T modelo, string chave, int minutosCache = 0)
    {
      if(minutosCache == 0) minutosCache = minutosCachePadrao();
      var json = JsonSerializer.Serialize(modelo);
      _cache.StringSet(chave, json, TimeSpan.FromMinutes(minutosCache));
    }

    public List<T> ListaDeModelosEmCache<T>(string chave)
    {
      var modelosSerializable = _cache.StringGet(chave);

      if(string.IsNullOrEmpty(modelosSerializable)) return new List<T>();
      var modelos = JsonSerializer.Deserialize<List<T>>(modelosSerializable);
      return modelos;
    }

    public T ModeloEmCache<T>(string chave)
    {
      var modelosSerializable = _cache.StringGet(chave);

      if(string.IsNullOrEmpty(modelosSerializable)) return (T)Activator.CreateInstance(typeof(T));
      var modelos = JsonSerializer.Deserialize<T>(modelosSerializable);
      return modelos;
    }

    public void LimpaCache(string chave)
    {
        _cache.KeyDelete(chave);
    }
  }
}
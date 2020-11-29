using System;
using System.IO;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace Desafio21diasAPI.Servicos.Cache
{
  public class RedisConnectorHelper
  {
    static RedisConnectorHelper()
    {
      RedisConnectorHelper.lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
      {
        JToken jAppSettings = JToken.Parse(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "appsettings.json")));
        return ConnectionMultiplexer.Connect(jAppSettings["redis"]["cnn"].ToString());
      });
    }

    private static Lazy<ConnectionMultiplexer> lazyConnection;

    public static ConnectionMultiplexer Connection
    {
      get
      {
        return lazyConnection.Value;
      }
    }

  }
}
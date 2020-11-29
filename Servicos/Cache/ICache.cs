using System.Collections.Generic;

namespace Desafio21diasAPI.Servicos.Cache
{
  public interface ICache
  {
    void AdicionarListaNoCache<T>(List<T> modelos, string chave, int minutosCache = 0);
    void AdicionarModeloNoCache<T>(T modelo, string chave, int minutosCache = 0);
    List<T> ListaDeModelosEmCache<T>(string chave);
    T ModeloEmCache<T>(string chave);
    void LimpaCache(string chave);
  }
}
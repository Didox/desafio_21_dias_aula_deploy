using System;
using System.Collections.Generic;
using Desafio21diasAPI.Models;

namespace Desafio21diasAPI.Servicos.Database
{
    public interface IRepositorioSql
    {
        void Excluir<T>(int id);
        T BuscaPorId<T>(int id);
        List<T> Todos<T>(string criterio);
        void ExecutaSqlQuery<T>(string sql);
        List<T> TodosSqlQuery<T>(string sql);
    }
}

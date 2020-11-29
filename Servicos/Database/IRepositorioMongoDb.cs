using System;
using System.Collections.Generic;
using Desafio21diasAPI.Models;
using MongoDB.Bson;
using MongoDB.Driver.Linq;

namespace Desafio21diasAPI.Servicos.Database
{
    public interface IRepositorioMongoDb
    {
        void Excluir<T>(ObjectId id);
        void RemoverColecao<T>();
        IMongoQueryable<T> BuscaCriterio<T>();
    }
}

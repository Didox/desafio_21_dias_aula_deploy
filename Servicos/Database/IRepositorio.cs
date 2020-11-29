using System;
using System.Collections.Generic;
using Desafio21diasAPI.Models;

namespace Desafio21diasAPI.Servicos.Database
{
    public interface IRepositorio
    {
        void Salvar<T>(T model);
        List<T> Todos<T>();
        string DadosDoArmazenamento();
    }
}

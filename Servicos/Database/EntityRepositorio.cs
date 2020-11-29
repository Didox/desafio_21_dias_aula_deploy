using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Desafio21diasAPI.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Desafio21diasAPI.Servicos.Database
{
  public partial class EntityRepositorio : DbContext
  {
    public EntityRepositorio(DbContextOptions options) : base(options) { }
    public EntityRepositorio() { }
    public DbSet<Cliente> Clientes { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      JToken jAppSettings = JToken.Parse(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "appsettings.json")));
      optionsBuilder.UseSqlServer(jAppSettings["sqlCnn"].ToString());
    }
  }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Desafio21diasAPI.Models;
using Newtonsoft.Json.Linq;

namespace Desafio21diasAPI.Servicos.Database
{
  public class SqlRepositorio : IRepositorio, IRepositorioSql
  {

    public string DadosDoArmazenamento()
    {
      JToken jAppSettings = JToken.Parse(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "appsettings.json")));
      return jAppSettings["ConnectionStrings"]["sqlCnn"].ToString();
    }

    public static string PreparaCampoQuery(string valor)
    {
      valor = valor.Replace("'", "");
      valor = valor.Replace("[", "[[]");
      valor = valor.Replace("%", "[%]");
      valor = valor.Replace("_", "[_]");
      return valor;
    }

    private string nomeTabela(Type modelo)
    {
      return $"{modelo.Name.ToLower()}s";
    }
    private void PreencherObjeto(object modelo, SqlDataReader dr)
    {
      foreach (var p in modelo.GetType().GetProperties())
      {
        try
        {
          if (dr[p.Name] == DBNull.Value || p.Name.ToLower() == "senha") continue;
          p.SetValue(modelo, dr[p.Name]);
        }
        catch { }
      }
    }

    public List<T> Todos<T>(string criterio)
    {
      string tabela = this.nomeTabela(typeof(T));
      string sql = $"select * from {tabela} ";
      if (!string.IsNullOrEmpty(criterio))
      {
        sql += $"where {criterio}";
      }

      Console.WriteLine("====================");
      Console.WriteLine(sql);
      Console.WriteLine("====================");
      return this.TodosSqlQuery<T>(sql);
    }
    public List<T> Todos<T>()
    {
      return this.Todos<T>(string.Empty);
    }

    public T BuscaPorId<T>(int id)
    {
      var objetos = this.Todos<T>($"id = {id}");
      if (objetos.Count == 0) return default(T);
      return objetos[0];
    }

    public void Salvar<T>(T cliente)
    {
      using (SqlConnection conn = new SqlConnection(DadosDoArmazenamento()))
      {
        List<string> cols = new List<string>();
        List<object> values = new List<object>();

        foreach (var p in cliente.GetType().GetProperties())
        {
          if (p.GetValue(cliente) == null || p.Name == "Id" || p.Name == "Token") continue;
          cols.Add(p.Name);
          values.Add(p.GetValue(cliente));
        }

        string tabela = this.nomeTabela(cliente.GetType());

        string sql = string.Empty;
        //if (Convert.ToInt32(cliente.GetType().GetProperty("Id").GetValue(cliente)) == 0)
        var id = ((IModel)cliente).Id;
        if (id == 0)
        {
          sql = $"insert into {tabela} (";
          sql += string.Join(',', cols);
          sql += ") values ( ";
          sql += "@" + string.Join(", @", cols);
          sql += ")";
        }
        else
        {
          sql = $"update {tabela} set ";

          var colsUpdate = new List<string>();
          foreach (string col in cols)
          {
            colsUpdate.Add($"{col}=@{col}");
          }
          sql += string.Join(',', colsUpdate);

          sql += $" where id = {id}";
        }

        SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.CommandType = CommandType.Text;

        for (var i = 0; i < cols.Count; i++)
        {
          var value = values[i];
          var col = cols[i];

          cmd.Parameters.Add($"@{col}", this.getDbType(value));
          cmd.Parameters[$"@{col}"].Value = value;
        }

        try
        {
          conn.Open();
          cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }
    }

    private SqlDbType getDbType(object value)
    {
      var result = SqlDbType.VarChar;

      try
      {
        Type type = value.GetType();

        switch (Type.GetTypeCode(type))
        {
          case TypeCode.Object:
            result = SqlDbType.Variant;
            break;
          case TypeCode.Boolean:
            result = SqlDbType.Bit;
            break;
          case TypeCode.Char:
            result = SqlDbType.NChar;
            break;
          case TypeCode.SByte:
            result = SqlDbType.SmallInt;
            break;
          case TypeCode.Byte:
            result = SqlDbType.TinyInt;
            break;
          case TypeCode.Int16:
            result = SqlDbType.SmallInt;
            break;
          case TypeCode.UInt16:
            result = SqlDbType.Int;
            break;
          case TypeCode.Int32:
            result = SqlDbType.Int;
            break;
          case TypeCode.UInt32:
            result = SqlDbType.BigInt;
            break;
          case TypeCode.Int64:
            result = SqlDbType.BigInt;
            break;
          case TypeCode.UInt64:
            result = SqlDbType.Decimal;
            break;
          case TypeCode.Single:
            result = SqlDbType.Real;
            break;
          case TypeCode.Double:
            result = SqlDbType.Float;
            break;
          case TypeCode.Decimal:
            result = SqlDbType.Money;
            break;
          case TypeCode.DateTime:
            result = SqlDbType.DateTime;
            break;
          case TypeCode.String:
            result = SqlDbType.VarChar;
            break;
        }
      }
      catch (Exception ex)
      {
        throw (ex);
      }

      return result;
    }

    public void Excluir<T>(int id)
    {
      string tabela = this.nomeTabela(typeof(T));
      string sql = $"delete from {tabela} where id = {id}";
      this.ExecutaSqlQuery<T>(sql);
    }

    public void ExecutaSqlQuery<T>(string sql)
    {
      using (SqlConnection conn = new SqlConnection(DadosDoArmazenamento()))
      {
        string tabela = this.nomeTabela(typeof(T));
        SqlCommand cmd = new SqlCommand(sql, conn);
        try
        {
          cmd.CommandType = System.Data.CommandType.Text;
          conn.Open();
          cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }
    }

    public List<T> TodosSqlQuery<T>(string sql)
    {
      var list = new List<T>();
      using (SqlConnection conn = new SqlConnection(DadosDoArmazenamento()))
      {
        SqlCommand cmd = new SqlCommand(sql, conn);
        try
        {

          cmd.CommandType = System.Data.CommandType.Text;

          conn.Open();

          using (SqlDataReader dr = cmd.ExecuteReader())
          {
            while (dr.Read())
            {
              var instancia = Activator.CreateInstance(typeof(T));
              this.PreencherObjeto(instancia, dr);
              list.Add((T)instancia);
            }
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
        return list;
      }
    }
  }
}

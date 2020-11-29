using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Desafio21diasAPI.Models;
using Desafio21diasAPI.Servicos.Autenticacao;
using Desafio21diasAPI.Servicos.Cache;
using Desafio21diasAPI.Servicos.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Desafio21diasAPI.Controllers
{
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(ILogger<ClientesController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("/clientes/login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] Cliente clienteLogin)
        {
            try
            {
                var cliente = UsuarioAutenticacao.Autenticar(clienteLogin.Login, clienteLogin.Senha);

                if (cliente == null)
                    return BadRequest(new { message = "Usuário ou senha inválidos" });

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("/clientes")]
        public IEnumerable<Cliente> Get()
        {
            return new SqlRepositorio().Todos<Cliente>();

            //return new EntityRepositorio().Clientes.ToList();

            // int minutosDeCache = 1;
            // HttpContext.Response.Headers.Add("Cache-Control", $"max-age={minutosDeCache * 60}, public, no-transform");

            // List<Cliente> lista;
            // var cache = new CacheHelper();
            // lista = cache.ListaDeModelosEmCache<Cliente>("clientes");

            // if(lista.Count == 0){
            //     lista = new MongoDbRepositorio().Todos<Cliente>();
            //     cache.AdicionarListaNoCache<Cliente>(lista, "clientes", minutosDeCache);
            // }
            
            // return lista;
        }

        [HttpPost]
        [Route("/clientes")]
        public Cliente Criar([FromBody] Cliente cliente)
        {
            new SqlRepositorio().Salvar<Cliente>(cliente);

            // var db = new EntityRepositorio();
            // db.Clientes.Add(cliente);
            // db.SaveChanges();

            // new MongoDbRepositorio().Salvar<Cliente>(cliente);

            return cliente;
        }

        [HttpGet]
        [Route("/clientes/{id}")]
        public Cliente Ver(int id)
        {
            Cliente cliente = new SqlRepositorio().BuscaPorId<Cliente>(id);
            // Cliente cliente = new EntityRepositorio().Clientes.Where(c => c.Id == id).First();
            // Cliente cliente = new MongoDbRepositorio().BuscaCriterio<Cliente>().Where(c => c.Id == ObjectId.Parse(id)).First();
            return cliente;
        }

        [HttpPut]
        [Route("/clientes/{id}")]
        [Authorize(Roles = "editor, administrador")]
        public IActionResult Atualizar(int id, [FromBody] Cliente cliente)
        {
            var cli = new SqlRepositorio().BuscaPorId<Cliente>(id);
            // var db = new EntityRepositorio();
            // var cli = db.Clientes.Where(c => c.Id == id).First();
            // var cli = new MongoDbRepositorio().BuscaCriterio<Cliente>().Where(c => c.Id == ObjectId.Parse(id)).First();

            var ruleAdm = HttpContext.User.Claims.SingleOrDefault(p => p.Value == "administrador");
            if(ruleAdm == null){
                var loginDado = HttpContext.User.Claims.SingleOrDefault(p => p.Value == ((Cliente)cli).Login);
                if(loginDado == null){
                    return Unauthorized(new { message = "Você não tem acesso para alterar informações de usuário" });
                }
            }

            cliente.Id = id; 
            new SqlRepositorio().Salvar(cliente);
            
            // cli.Nome = cliente.Nome;
            // cli.Endereco = cliente.Endereco;
            // cli.Login = cliente.Login;
            // cli.Senha = cliente.Senha;
            // cli.RegraAcesso = cliente.RegraAcesso;
            // cli.Telefone = cliente.Telefone;
            
            // db.Update(cli);
            // db.SaveChanges();
            
            // cliente.Id = ObjectId.Parse(id); 
            // new MongoDbRepositorio().Salvar(cliente);

            return NoContent();
        }

        [HttpDelete]
        [Authorize(Roles = "administrador")]
        [Route("/clientes/{id}")]
        public void Apagar(int id)
        {
            new SqlRepositorio().Excluir<Cliente>(id);
            
            // var db = new EntityRepositorio();
            // var cli = db.Clientes.Where(c => c.Id == id).First();
            // db.Clientes.Remove(cli);
            // db.SaveChanges();

            // new MongoDbRepositorio().Excluir<Cliente>(ObjectId.Parse(id));
        }
    }
}

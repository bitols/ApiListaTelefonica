using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApiListaTelefonica.Models;
using ApiListaTelefonica.Processos;
using ApiListaTelefonica.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiListaTelefonica.Controllers
{
    [Authorize("Bearer")]
    [Produces("application/json")]
    [Route("api/Contatos")]
    public class ContatosController : Controller
    {
        [HttpPost]
        public IActionResult CadastrarContato(
            [FromServices] ContatoRepository contatoRepository,
            [FromBody] Contato contato)
        {
            if (!ModelState.IsValid)
                return StatusCode(422);

            var usuarioToken = ProcToken.DeserializeUser((ClaimsIdentity)User.Identity);
            contato.usuarioId = usuarioToken.usuarioId;
            contatoRepository.Add(contato);
            return Ok();
        }
        
        [HttpPut]
        public IActionResult EditarContato(
            [FromServices] ContatoRepository contatoRepository,
            [FromBody] Contato contato)
        {
            if (!ModelState.IsValid)
                return StatusCode(422);

            var usuarioToken = ProcToken.DeserializeUser((ClaimsIdentity)User.Identity);
            contato.usuarioId = usuarioToken.usuarioId;
            contatoRepository.Update(contato);
            return Ok();
        }

        [HttpDelete("{id}/remove")]
        public IActionResult DeletarContato(
            [FromServices] ContatoRepository contatoRepository,
            int id)
        {
            var usuarioToken = ProcToken.DeserializeUser((ClaimsIdentity)User.Identity);
            var contato = new Contato();
            contato.contatoId = id;
            contato.usuarioId = usuarioToken.usuarioId;
            contato = contatoRepository.Get<Contato>(contato);
            if (contato!=null)
                contatoRepository.Delete(contato);
            return Ok();

        }

        [HttpGet]
        public IActionResult BuscarContatos(
             [FromServices] ContatoRepository contatoRepository
            )
        {
            var usuarioToken = ProcToken.DeserializeUser((ClaimsIdentity)User.Identity);
            var contatos = contatoRepository.GetAll<Contato>(usuarioToken.usuarioId);
            if (contatos.Count <= 0)
                return NoContent();
            return Ok(contatos);
        }

        [HttpGet("{filtro}")]
        public IActionResult BuscarContatosFiltro(
             [FromServices] ContatoRepository contatoRepository,
            string filtro
            )
        {
            var usuarioToken = ProcToken.DeserializeUser((ClaimsIdentity)User.Identity);
            var contatos = contatoRepository.GetAll<Contato>(usuarioToken.usuarioId);
            if (contatos.Count <= 0)
                return NoContent();
            var retorno = contatos.FindAll(c =>
            {
                return c.contatoNome.ToUpper().Contains(filtro.ToUpper()) || (c.telefones.FindAll(t => t.telefoneNumero.Contains(filtro) || t.tipo.ToUpper().Contains(filtro.ToUpper())).Count >0 );
            });
            if (retorno.Count <= 0)
                return NoContent();
            return Ok(retorno);

        }
    }
}
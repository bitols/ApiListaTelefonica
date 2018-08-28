using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using ApiListaTelefonica.Models;
using ApiListaTelefonica.Processos;
using ApiListaTelefonica.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ApiListaTelefonica.Controllers
{
    [Produces("application/json")]
    [Route("api/Usuarios")]
    public class UsuariosController : Controller
    {
        [HttpPost]
        public IActionResult CadastrarUsuario(
            [FromServices] UsuarioRepository usuarioRepository,
            [FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
                return StatusCode(422);

            if (usuarioRepository.Get<Usuario>(usuario.usuarioLogin) != null)
                return StatusCode(409);
            usuario.usuarioSenha = ProcHash.CalculateSHA256(usuario.usuarioSenha);
            usuarioRepository.Add(usuario);
            return Ok();
        }

        [HttpPost("login")]
        public IActionResult EntrarUsuario(
            [FromServices] UsuarioRepository usuarioRepository,
            [FromServices]SigningConfigurations signingConfigurations,
            [FromServices]TokenConfigurations tokenConfigurations,
            [FromForm]Login login)
        {
            var token = ProcToken.Generate(
                usuarioRepository,
                signingConfigurations,
                tokenConfigurations,
                login);
            if (token == null)
                return StatusCode(401);
            return Ok(token);
        }


    }
}
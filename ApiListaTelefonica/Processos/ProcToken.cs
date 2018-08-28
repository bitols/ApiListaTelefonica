using ApiListaTelefonica.Models;
using ApiListaTelefonica.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiListaTelefonica.Processos
{
    public static class ProcToken
    {
        public static Usuario DeserializeUser(ClaimsIdentity identity)
        {
            IEnumerable<Claim> claims = identity.Claims;
            var usuario = new Usuario();
            foreach(Claim claim in claims)
            {
                if (claim.Type == "id")
                    usuario.usuarioId = Int32.Parse(claim.Value);
                if (claim.Type == "user")
                    usuario.usuarioLogin = claim.Value;
            }
            return usuario;
        }

        public static object Generate(
            [FromServices] UsuarioRepository usuarioRepository,
            [FromServices]SigningConfigurations signingConfigurations,
            [FromServices]TokenConfigurations tokenConfigurations,
            Login login)
        {
            bool credenciaisValidas = false;
            var usuario = usuarioRepository.Get<Usuario>(login.usuario);
            
            login.senha = ProcHash.CalculateSHA256(login.senha);
            credenciaisValidas = (usuario.usuarioSenha == login.senha);
            if (credenciaisValidas)
            {
                ClaimsIdentity identity = new ClaimsIdentity(
                    new[]  {
                        new Claim("id",usuario.usuarioId.ToString()),
                        new Claim("user", usuario.usuarioLogin)
                    }
                );

                DateTime dataCriacao = DateTime.Now;
                DateTime dataExpiracao = dataCriacao + TimeSpan.FromDays(tokenConfigurations.Days);

                var handler = new JwtSecurityTokenHandler();
                var securityToken = handler.CreateToken(new SecurityTokenDescriptor
                {
                    Issuer = tokenConfigurations.Issuer,
                    Audience = tokenConfigurations.Audience,
                    SigningCredentials = signingConfigurations.SigningCredentials,
                    Subject = identity,
                    NotBefore = dataCriacao,
                    Expires = dataExpiracao,

                });

                var tokenStr = handler.WriteToken(securityToken);

                return new
                {
                    apiKey = tokenStr
                };

            }
            else
            {
                return null;
            }
        }
    }
}

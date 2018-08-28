using ApiListaTelefonica.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ApiListaTelefonica.Repositories
{
    public class UsuarioRepository : IRepository
    {
        private IConfiguration _configuracoes;
        public UsuarioRepository(IConfiguration config)
        {
            _configuracoes = config;

        }

        public void Add(dynamic parameters)
        {

            using (SqlConnection conexao = new SqlConnection(
                _configuracoes.GetConnectionString("ListaTelefonica")))
            {
                parameters.usuarioId = conexao.Query<int>(
                    @"INSERT INTO usuario
                        (usuarioNome, 
                         usuarioLogin,
                         usuarioSenha)
                    VALUES
                        (@nome, 
                         @login,
                         @senha); 
                      SELECT 
	                    CAST(SCOPE_IDENTITY() as INT)",
                    new
                    {
                        nome = parameters.usuarioNome,
                        login = parameters.usuarioLogin,
                        senha = parameters.usuarioSenha
                    }
                    ).SingleOrDefault();
            }

        }

        public void Delete(dynamic parameters)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(dynamic parameters)
        {
            using (SqlConnection conexao = new SqlConnection(
                _configuracoes.GetConnectionString("ListaTelefonica")))
            {
                return conexao.Query<T>(
                    @" SELECT 
                         usuarioId,
                         usuarioNome, 
                         usuarioLogin,
                         usuarioSenha
                       FROM
                         Usuario
                       WHERE
                          usuarioLogin = @login",
                    new
                    {
                        login = parameters
                    }

                    ).FirstOrDefault();
            }
        }

        public List<T> GetAll<T>(dynamic parameters)
        {
            throw new NotImplementedException();
        }

        public void Update(dynamic parameters)
        {
            using (SqlConnection conexao = new SqlConnection(
                _configuracoes.GetConnectionString("ListaTelefonica")))
            {
                conexao.Execute(
                    @"UPDATE 
                        Usuario
                      SET
                        usuarioSenha = @senha,
                        usuarioNome = @nome
                      WHERE 
	                    usuarioId = @id",
                    new
                    {
                        senha = parameters.usuarioSenha,
                        nome = parameters.usuarioNome,
                        id = parameters.usuarioId
                    });
            }
        }
    }
}

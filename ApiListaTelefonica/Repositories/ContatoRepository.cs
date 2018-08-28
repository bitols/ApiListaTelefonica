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
    public class ContatoRepository : IRepository
    {
        private IConfiguration _configuracoes;
        public ContatoRepository(IConfiguration config)
        {
            _configuracoes = config;

        }

        public void Add(dynamic parameters)
        {
            using (SqlConnection conexao = new SqlConnection(
                _configuracoes.GetConnectionString("ListaTelefonica")))
            {
                parameters.contatoId = conexao.Query<int>(
                    @"INSERT INTO contato
                        (contatoNome, 
                         usuarioId)
                    VALUES
                        (@nome, 
                         @usuarioId); 
                      SELECT 
	                    CAST(SCOPE_IDENTITY() as INT)",
                    new
                    {
                        nome = parameters.contatoNome,
                        usuarioId = parameters.usuarioId
                    }
                    ).SingleOrDefault();

                if (parameters.contatoId > 0)
                {
                    foreach(var telefone in parameters.telefones)
                    {
                        telefone.contatoId = parameters.contatoId;
                        conexao.Execute(
                        @"INSERT INTO Telefone 
                        (telefoneNumero, 
                         tipoId,
                         contatoId)
                      VALUES
                        (@numero, 
                         (SELECT tipoId FROM Tipo WHERE tipoDescricao = @tipo),
                         @contatoId)",
                        new
                        {
                            numero = telefone.telefoneNumero,
                            contatoId = telefone.contatoId,
                            tipo = telefone.tipo
                        });
                    }

                }
            }
        }

        public void Delete(dynamic parameters)
        {
            using (SqlConnection conexao = new SqlConnection(
                _configuracoes.GetConnectionString("ListaTelefonica")))
            {
                conexao.Execute(
                    @" DELETE FROM Telefone
                       WHERE 
                         contatoId = @contatoId",
                    new
                    {
                        contatoId = parameters.contatoId
                        
                    });
                conexao.Execute(
                    @" DELETE FROM Contato
                       WHERE 
                         contatoId = @contatoId
                         AND usuarioID = @usuarioId",
                    new
                    {
                        contatoId = parameters.contatoId,
                        usuarioId = parameters.usuarioId
                    });
            }
        }

        public T Get<T>(dynamic parameters)
        {
            var contatoDictionary = new Dictionary<int, Contato>();
            var telefonelDictionary = new Dictionary<int, Telefone>();
            var tipoDictionary = new Dictionary<int, Tipo>();

            using (SqlConnection conexao = new SqlConnection(
                _configuracoes.GetConnectionString("ListaTelefonica")))
            {
                return conexao.Query<Contato,Telefone, Tipo,T>(
                    @" SELECT 
                              con.contatoId,
                              con.contatoNome,
                              con.usuarioId,
                              tel.contatoId,  
                              tel.telefoneId,
                              tel.telefoneNumero,
                              tel.tipoId,
                              tip.tipoId,
                              tip.tipoDescricao
                          FROM
                              contato con
                              JOIN telefone tel
                              ON con.contatoId = tel.contatoId
                              JOIN tipo tip 
                              ON tel.tipoId = tip.tipoId
                          WHERE
                              con.contatoId = @id
                              AND con.usuarioId = @usuarioId   ",
                    (con, tel, tip) =>
                    {
                        Tipo tipo;
                        if(!tipoDictionary.TryGetValue(tip.tipoId, out tipo))
                        {
                            tipo = tip;
                            tipoDictionary.Add(tipo.tipoId, tipo);
                        }

                        Contato contato;
                        if (!contatoDictionary.TryGetValue(con.contatoId, out contato))
                        {
                            contato = con;
                            contato.telefones = new List<Telefone>();
                            contatoDictionary.Add(contato.contatoId, contato);
                        }
                        Telefone telefone;
                        if(!telefonelDictionary.TryGetValue(tel.telefoneId, out telefone))
                        {
                            telefone = tel;
                            telefone.tipo = tipo.tipoDescricao;
                            contato.telefones.Add(telefone);
                            telefonelDictionary.Add(telefone.telefoneId, telefone);
                        }


                        
                        return (T)Convert.ChangeType(contato, typeof(T));
                    },
                    new
                    {
                        id = parameters.contatoId,
                        usuarioId = parameters.usuarioId
                    },
                    splitOn:"contatoId, tipoId"

                    ).FirstOrDefault();
            }
        }


        public List<T> GetAll<T>(dynamic parameters)
        {
            var contatoDictionary = new Dictionary<int, Contato>();
            var telefonelDictionary = new Dictionary<int, Telefone>();
            var tipoDictionary = new Dictionary<int, Tipo>();

            using (SqlConnection conexao = new SqlConnection(
                _configuracoes.GetConnectionString("ListaTelefonica")))
            {
                return conexao.Query<Contato, Telefone, Tipo, T>(
                    @" SELECT 
                              con.contatoId,
                              con.contatoNome,
                              con.usuarioId,
                              tel.contatoId,  
                              tel.telefoneId,
                              tel.telefoneNumero,
                              tel.tipoId,
                              tip.tipoId,
                              tip.tipoDescricao
                          FROM
                              contato con
                              JOIN telefone tel
                              ON con.contatoId = tel.contatoId
                              JOIN tipo tip 
                              ON tel.tipoId = tip.tipoId
                          WHERE
                              con.usuarioId = @usuarioId   
                          ORDER BY
                              con.contatoNome ",
                    (con, tel, tip) =>
                    {
                        Tipo tipo;
                        if (!tipoDictionary.TryGetValue(tip.tipoId, out tipo))
                        {
                            tipo = tip;
                            tipoDictionary.Add(tipo.tipoId, tipo);
                        }

                        Contato contato;
                        if (!contatoDictionary.TryGetValue(con.contatoId, out contato))
                        {
                            contato = con;
                            contato.telefones = new List<Telefone>();
                            contatoDictionary.Add(contato.contatoId, contato);
                        }
                        Telefone telefone;
                        if (!telefonelDictionary.TryGetValue(tel.telefoneId, out telefone))
                        {
                            telefone = tel;
                            telefone.tipo = tipo.tipoDescricao;
                            contato.telefones.Add(telefone);
                            telefonelDictionary.Add(telefone.telefoneId, telefone);
                        }



                        return (T)Convert.ChangeType(contato, typeof(T));
                    },
                    new
                    {
                        usuarioId = parameters
                    },
                    splitOn: "contatoId, tipoId"

                    ).Distinct().ToList();
            }
        }

        public void Update(dynamic parameters)
        {
            
            using (SqlConnection conexao = new SqlConnection(
                _configuracoes.GetConnectionString("ListaTelefonica")))
            {
                if (conexao.Execute(
                    @" UPDATE Contato
                       SET
                         contatoNome = @nome
                       WHERE 
                         contatoId = @id
                         AND usuarioId = @usuarioId",
                    new
                    {
                        id = parameters.contatoId,
                        usuarioId = parameters.usuarioId,
                        nome = parameters.contatoNome
                    }) > 0)
                {
                    conexao.Execute(
                        @" DELETE FROM Telefone
                       WHERE 
                         contatoId = @contatoId",
                        new
                        {
                            contatoId = parameters.contatoId
                        });


                    foreach (var telefone in parameters.telefones)
                    {
                        telefone.contatoId = parameters.contatoId;
                        conexao.Execute(
                        @"INSERT INTO Telefone 
                        (telefoneNumero, 
                         tipoId,
                        contatoId)
                    VALUES
                        (@numero, 
                        (SELECT tipoId FROM Tipo WHERE tipoDescricao = @tipo),
                        @contatoId)",
                        new
                        {
                            numero = telefone.telefoneNumero,
                            contatoId = telefone.contatoId,
                            tipo = telefone.tipo
                        });
                    }
                }

            }
        }
    }
}

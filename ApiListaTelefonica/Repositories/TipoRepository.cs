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
    public class TipoRepository: IRepository
    {
        private IConfiguration _configuracoes;
        public TipoRepository(IConfiguration config)
        {
            _configuracoes = config;

        }

        public void Add(dynamic parameters)
        {
            throw new NotImplementedException();
        }

        public void Delete(dynamic parameters)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(dynamic parameters)
        {
            throw new NotImplementedException();
        }


        public List<T> GetAll<T>(dynamic parameters)
        {
            using (SqlConnection conexao = new SqlConnection(
                _configuracoes.GetConnectionString("ListaTelefonica")))
            {
                return conexao.Query<T>(
                    @" SELECT 
                         tipoId,
                         tipoDescricao
                       FROM
                         Tipo
                       ORDER BY
                         tipoDescricao"
                    ).ToList();
            }
        }

        public void Update(dynamic parameters)
        {
            throw new NotImplementedException();
        }
    }
}

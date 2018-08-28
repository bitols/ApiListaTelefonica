using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiListaTelefonica.Repositories
{
    interface IRepository
    {
        List<T> GetAll<T>(dynamic parameters);
        T Get<T>(dynamic parameters);

        void Add(dynamic parameters);
        void Update(dynamic parameters);
        void Delete(dynamic parameters);
    }
}

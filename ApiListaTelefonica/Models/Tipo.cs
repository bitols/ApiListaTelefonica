using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiListaTelefonica.Models
{
    public class Tipo
    {
        [JsonIgnore]
        public int tipoId { get; set; }
        [JsonProperty(PropertyName = "tipo")]
        public string tipoDescricao { get; set; }
    }
}

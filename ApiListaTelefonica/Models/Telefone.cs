using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiListaTelefonica.Models
{
    public class Telefone
    {
        [JsonIgnore]
        public int telefoneId { get; set; }

        [Required]
        [JsonProperty(PropertyName = "numero")]
        public string telefoneNumero { get; set; }

        [JsonIgnore]
        public int tipoId { get; set; }
        [JsonIgnore]
        public int contatoId { get; set; }

        [Required]
        [JsonProperty(PropertyName = "tipo")]
        public string tipo { get; set; }


    }
}

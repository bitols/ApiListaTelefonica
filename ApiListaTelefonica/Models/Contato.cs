using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiListaTelefonica.Models
{
    public class Contato
    {
        [JsonProperty(PropertyName = "id")]
        public int contatoId { get; set; }

        [Required]
        [JsonProperty(PropertyName = "nome")]
        public string contatoNome { get; set; }    
        [JsonIgnore]
        public int usuarioId { get; set; }

        [Required]
        [JsonProperty(PropertyName = "telefones")]
        public List<Telefone> telefones { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiListaTelefonica.Models
{
    public class Usuario
    {
        [JsonProperty(PropertyName = "id")]
        public int usuarioId { get; set; }

        [Required]
        [JsonProperty(PropertyName = "nome")]
        public string usuarioNome { get; set; }

        [Required]
        [JsonProperty(PropertyName = "login")]
        public string usuarioLogin { get; set; }

        [Required]
        [JsonProperty(PropertyName = "senha")]
        public string usuarioSenha { get; set; }
    }
}

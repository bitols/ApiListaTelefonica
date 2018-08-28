using ApiListaTelefonica.Models;
using ApiListaTelefonica.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ApiListaTelefonica.Controllers
{
    [Produces("application/json")]
    [Route("api/Tipos")]
    public class TiposController : Controller
    {

        public IActionResult BuscarTipos([FromServices] TipoRepository tipoRepository)
        {
            var tipos = tipoRepository.GetAll<Tipo>(null);
            if(tipos.Count<=0)
                return NoContent();
            return Ok(tipos);

            
        }
    }
}
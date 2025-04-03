using CrudApi.DTOs;
using CrudApi.Interfaces;
using CrudApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrudApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoDocumentoController : ControllerBase
    {
        private readonly ITipoDocumentoService _tipoDocumentoService;

        public TipoDocumentoController(ITipoDocumentoService tipoDocumentoService)
        {
            _tipoDocumentoService = tipoDocumentoService;
        }

        // 🔹 Obtener todos los tipos de documento
        [HttpGet]
        public async Task<ActionResult<List<TipoDocumentoDTO>>> GetTipoDocumentos()
        {
            var tiposDocumentos = await _tipoDocumentoService.GetTipoDocumentosAsync();
            return Ok(tiposDocumentos);
        }

      
    }
}

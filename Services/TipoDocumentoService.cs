using CrudApi.Data;
using CrudApi.DTOs;
using CrudApi.Interfaces;
using CrudApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrudApi.Services
{
    public class TipoDocumentoService : ITipoDocumentoService
    {
        private readonly ApplicationDbContext _context;

        public TipoDocumentoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TipoDocumentoDTO>> GetTipoDocumentosAsync()
        {
            return await _context.TipoDocumento
                .Select(doc => new TipoDocumentoDTO
                {
                    Id = doc.Id,
                    Nombre = doc.Nombre
                })
                .ToListAsync();
        }
    }
}

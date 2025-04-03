using CrudApi.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrudApi.Interfaces
{
    public interface ITipoDocumentoService
    {
        Task<IEnumerable<TipoDocumentoDTO>> GetTipoDocumentosAsync();
    }
}

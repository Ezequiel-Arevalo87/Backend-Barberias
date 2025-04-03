using System.ComponentModel.DataAnnotations;

namespace CrudApi.DTOs
{
    public class TipoDocumentoDTO
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }
    }
}

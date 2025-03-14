using System.ComponentModel.DataAnnotations;

namespace CrudApi.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;
    }
}

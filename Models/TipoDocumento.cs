using CrudApi.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class TipoDocumento
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Nombre { get; set; }

  
}

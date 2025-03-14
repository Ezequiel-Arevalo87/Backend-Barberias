using CrudApi.DTOs;
using CrudApi.Interfaces;
using CrudApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrudApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<UsuarioDTO>>> GetUsuarios()
        {
            var usuarios = await _usuarioService.GetUsuariosAsync();
            return Ok(usuarios);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDTO?>> GetUsuario(int id)
        {
            var usuario = await _usuarioService.GetUsuarioByIdAsync(id);
            if (usuario == null) return NotFound();
            return Ok(usuario);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] UsuarioCreateDTO usuarioDto)
        {
            var nuevoUsuario = await _usuarioService.CreateUsuarioAsync(usuarioDto);
            return CreatedAtAction(nameof(GetUsuario), new { id = nuevoUsuario.Id }, nuevoUsuario);
        }

        [Authorize(Roles = "Admin,UsuarioB")]
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] UsuarioUpdateDTO usuarioDto)
        {
            if (id <= 0)
                return BadRequest("ID inválido");

            var usuarioActualizado = await _usuarioService.UpdateUsuarioAsync(id, usuarioDto);

            if (usuarioActualizado == null)
                return NotFound("Usuario no encontrado");

            return Ok(usuarioActualizado);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var deleted = await _usuarioService.DeleteUsuarioAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}

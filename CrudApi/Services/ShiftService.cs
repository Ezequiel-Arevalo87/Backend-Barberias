using CrudApi.Data;
using CrudApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrudApi.Services
{
    public class ShiftService : IShiftService
    {
        private readonly ApplicationDbContext _context;

        public ShiftService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Implementación corregida
        public async Task CerrarTurnosVencidosAsync()
        {
            var now = DateTime.Now;

            var turnos = await _context.Turnos
                .Where(t => t.Estado != "CERRADO")
                .ToListAsync();

            foreach (var turno in turnos)
            {
                var horaFin = turno.FechaHora.Add(turno.Duracion); // Calcula hora de finalización

                // Verificar si el turno está en proceso
                if (turno.FechaHora <= now && horaFin > now)
                {
                    turno.Estado = "EN_PROCESO";
                }
                // Verificar si el turno ya venció
                else if (horaFin <= now)
                {
                    turno.Estado = "CERRADO";
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}

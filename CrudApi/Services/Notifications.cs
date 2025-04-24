using CrudApi.DTOs;
using CrudApi.Models;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace CrudApi.Notifications
{
    public class Notifications
    {
        private readonly IConfiguration _configuration;
        private static bool firebaseInitialized = false;

        public Notifications(IConfiguration configuration)
        {
            _configuration = configuration;

            string firebaseJson = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_JSON");

            if (!firebaseInitialized && FirebaseApp.DefaultInstance == null)
            {
                try
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromJson(firebaseJson)
                    });

                    firebaseInitialized = true;
                    Console.WriteLine("✅ FirebaseApp inicializado correctamente.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error al inicializar FirebaseApp: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("ℹ️ FirebaseApp ya estaba inicializado.");
            }
        }
        public async Task<string> SendNotificationAsync(string token, TurnoDTO turno)
        {
            if (FirebaseMessaging.DefaultInstance == null)
                throw new InvalidOperationException("❌ FirebaseMessaging.DefaultInstance no está inicializado.");

            var cultura = new CultureInfo("es-CO");

            // ✅ Ya es hora local, así que usar directo
            var devuelvefechaLocalNotificacion = turno.FechaHoraInicio.AddHours(-5);
            var fechaLocal = turno.FechaHoraInicio;

            string fechaFormateada = fechaLocal.ToString("dddd dd/MM/yyyy 'a las' hh:mm tt", cultura);
            string title = "Turno Confirmado";
            string body = $"Tu turno fue agendado para el {devuelvefechaLocalNotificacion}. Servicio: {turno.ServicioNombre}";

            var message = new Message()
            {
                Token = token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = new Dictionary<string, string>
        {
            { "TurnoId", turno.Id.ToString() },
            { "BarberoId", turno.BarberoId.ToString() },
            { "ClienteId", turno.ClienteId.ToString() },
            {  "FechaHoraInicio", turno.FechaHoraInicio.AddHours(-5).ToString("yyyy-MM-dd HH:mm:ss") },
            { "Estado", turno.Estado.ToString() },
            { "ClienteNombre", turno.ClienteNombre ?? string.Empty },
            { "ClienteApellido", turno.ClienteApellido ?? string.Empty },
            { "ServicioNombre", turno.ServicioNombre ?? string.Empty },
            { "Duracion", turno.Duracion.ToString() },
            { "Tipo", "Cancelacion" },
             { "TurnoId", turno.Id.ToString() },
                 { "NuevoEstado", ((int)EstadoTurno.Cancelado).ToString() }
        }
            };

            return await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }

        public async Task<string> SendCancelacionClienteAsync(string token, TurnoDTO turno)
        {
            var cultura = new CultureInfo("es-CO");
            string fechaFormateada = turno.FechaHoraInicio.AddHours(-5).ToString("dddd dd/MM/yyyy 'a las' hh:mm tt", cultura);

            var message = new Message()
            {
                Token = token,
                Notification = new Notification
                {
                    Title = "Turno Cancelado por el Cliente",
                    Body = $"El cliente {turno.ClienteNombre} canceló su turno programado para el {fechaFormateada}.\n📝 Motivo: {turno.MotivoCancelacion}"
                }
            };

            return await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }

        public async Task<string> SendCancelacionBarberoAsync(string token, TurnoDTO turno)
        {
            var cultura = new CultureInfo("es-CO");
            string fechaFormateada = turno.FechaHoraInicio.AddHours(-5).ToString("dddd dd/MM/yyyy 'a las' hh:mm tt", cultura);

            var message = new Message()
            {
                Token = token,
                Notification = new Notification
                {
                    Title = "Turno Cancelado por el Barbero",
                    Body = $"Tu turno del {fechaFormateada} fue cancelado por el barbero. Motivo: {turno.MotivoCancelacion}"
                }
            };

            return await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }


    }
}

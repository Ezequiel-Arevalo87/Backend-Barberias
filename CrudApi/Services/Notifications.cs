using CrudApi.DTOs;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;

namespace CrudApi.Notifications
{
    public class Notifications
    {
        private readonly IConfiguration _configuration;

        public Notifications(IConfiguration configuration)
        {
            _configuration = configuration;
          

            // Verifica si ya existe una instancia de FirebaseApp
            string firebaseJson = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_JSON");

            if (FirebaseApp.DefaultInstance == null)
            {
                try
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromJson(firebaseJson)
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error al inicializar FirebaseApp: {ex.Message}");
                }
            }

        }

        public async Task<string> SendNotificationAsync(string token, string title, string body, TurnoDTO turno)
        {
            if (FirebaseMessaging.DefaultInstance == null)
            {
                throw new InvalidOperationException("❌ FirebaseMessaging.DefaultInstance no está inicializado.");
            }

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
                    { "FechaHoraInicio", turno.FechaHoraInicio.ToString("s") },
                    { "Estado", turno.Estado.ToString() },
                    { "ClienteNombre", turno.ClienteNombre ?? string.Empty },
                    { "ClienteApellido", turno.ClienteApellido ?? string.Empty },
                    { "ServicioNombre", turno.ServicioNombre ?? string.Empty },
                    { "Duracion", turno.Duracion.ToString() }
                }
            };

            return await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
    }
}

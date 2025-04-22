using CrudApi.DTOs;
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

            
            var fechaLocal = turno.FechaHoraInicio; // Ya viene en hora Colombia


            string fechaFormateada = fechaLocal.ToString("dddd dd/MM/yyyy 'a las' hh:mm tt", cultura);

            string title = "Turno Confirmado";
            string body = $"Tu turno fue agendado para el {fechaFormateada}. Servicio: {turno.ServicioNombre}";

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

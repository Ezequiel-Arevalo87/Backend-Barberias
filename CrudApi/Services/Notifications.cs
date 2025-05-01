using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.Globalization;
using CrudApi.Models;

namespace CrudApi.Notifications
{
    public class Notifications
    {
        private readonly IConfiguration _configuration;
        private static bool firebaseInitialized = false;

        public Notifications(IConfiguration configuration)
        {
            _configuration = configuration;

            // ✅ 1. Intentar con variable de entorno
            string firebaseJson = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_JSON");

            // ✅ 2. Si no está por variable, buscar en archivo .json definido en appsettings
            if (string.IsNullOrEmpty(firebaseJson))
            {
                string rutaArchivo = _configuration["FirebaseCredentialsPath"]; // Ej: "firebase-adminsdk.json"
                if (!string.IsNullOrEmpty(rutaArchivo) && File.Exists(rutaArchivo))
                {
                    firebaseJson = File.ReadAllText(rutaArchivo);
                }
            }

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
            var devuelvefechaLocalNotificacion = turno.FechaHoraInicio.AddHours(-5);

            string title = "Turno Confirmado";
            string body = $"Tu turno fue agendado para el {devuelvefechaLocalNotificacion:dd/MM/yyyy HH:mm}. Servicio: {turno.ServicioNombre}";

            var message = new Message()
            {
                Token = token,
                Data = new Dictionary<string, string>
        {
            { "title", title },
            { "body", body },
            { "tipo", "ACTUALIZAR_TURNO" }, // ✅ Esto es lo que faltaba
            { "TurnoId", turno.Id.ToString() },
            { "BarberoId", turno.BarberoId.ToString() },
            { "ClienteId", turno.ClienteId.ToString() },
            { "FechaHoraInicio", devuelvefechaLocalNotificacion.ToString("yyyy-MM-dd HH:mm:ss") },
            { "Estado", turno.Estado.ToString() },
            { "ClienteNombre", turno.ClienteNombre ?? string.Empty },
            { "ClienteApellido", turno.ClienteApellido ?? string.Empty },
            { "ServicioNombre", turno.ServicioNombre ?? string.Empty },
            { "Duracion", turno.Duracion.ToString() }
        }
            };

            return await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }


        public async Task EnviarNotificacionCancelacionClienteAsync(string token, TurnoDTO turno, string motivo)
        {
            var cultura = new CultureInfo("es-CO");
            string title = "Turno Cancelado";
            string body = $"Tu turno con {turno.BarberoNombre} fue cancelado. Motivo: {motivo}";

            await EnviarMensajePersonalizado(token, turno, title, body);
        }

        public async Task EnviarNotificacionCancelacionBarberoAsync(string token, TurnoDTO turno, string motivo)
        {
            var cultura = new CultureInfo("es-CO");
            string title = "Turno Cancelado por Cliente";
            string body = $"El cliente {turno.ClienteNombre} canceló su turno. Motivo: {motivo}";

            await EnviarMensajePersonalizado(token, turno, title, body);
        }

        // ✅ Nuevo método para cuando el barbero cancela
        public async Task EnviarNotificacionCancelacionPorBarberoAsync(string token, TurnoDTO turno, string motivo)
        {
            string title = "Turno Cancelado por Barbero";
            string body = $"Tu turno fue cancelado por el barbero {turno.BarberoNombre}. Motivo: {motivo}";

            await EnviarMensajePersonalizado(token, turno, title, body);
        }



        public async Task EnviarNotificacionCambioEstadoAsync(string token, TurnoDTO turno)
        {
            string estadoTexto = turno.Estado switch
            {
                EstadoTurno.EnProceso => "El turno ha comenzado.",
                EstadoTurno.Cerrado => "El turno ha finalizado.",
                _ => "Actualización del turno."
            };

            string titulo = "📢 Cambio en el turno";
            string cuerpo = $"{estadoTexto} Servicio: {turno.ServicioNombre}, Cliente: {turno.ClienteNombre}";

            var message = new Message
            {
                Token = token,
                Data = new Dictionary<string, string>
        {
            { "title", titulo },
            { "body", cuerpo },
            { "tipo", "ACTUALIZAR_TURNO" },
            { "TurnoId", turno.Id.ToString() },
            { "BarberoId", turno.BarberoId.ToString() },
            { "ClienteId", turno.ClienteId.ToString() },
            { "FechaHoraInicio", turno.FechaHoraInicio.AddHours(-5).ToString("yyyy-MM-dd HH:mm:ss") },
            { "Estado", ((int)turno.Estado).ToString() },
            { "ClienteNombre", turno.ClienteNombre ?? "" },
            { "ClienteApellido", turno.ClienteApellido ?? "" },
            { "ServicioNombre", turno.ServicioNombre ?? "" },
            { "Duracion", turno.Duracion.ToString() }
        }
            };

            await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }



        private async Task EnviarMensajePersonalizado(string token, TurnoDTO turno, string title, string body)
        {
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
                    { "FechaHoraInicio", turno.FechaHoraInicio.AddHours(-5).ToString("yyyy-MM-dd HH:mm:ss") },
                    { "Estado", turno.Estado.ToString() },
                    { "ClienteNombre", turno.ClienteNombre ?? string.Empty },
                    { "ClienteApellido", turno.ClienteApellido ?? string.Empty },
                    { "ServicioNombre", turno.ServicioNombre ?? string.Empty },
                    { "Duracion", turno.Duracion.ToString() }
                }
            };

            await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
    }
}

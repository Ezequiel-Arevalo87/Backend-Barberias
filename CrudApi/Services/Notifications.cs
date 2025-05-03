using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
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

            string firebaseJson = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_JSON");
            if (string.IsNullOrEmpty(firebaseJson))
            {
                string rutaArchivo = _configuration["FirebaseCredentialsPath"];
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
        }

        public async Task<string> SendNotificationAsync(string token, TurnoDTO turno)
        {
            Console.WriteLine($"🟠 Enviando notificación...");
            Console.WriteLine($"📲 Token: {token}");
            Console.WriteLine($"📅 FechaHoraInicio: {turno.FechaHoraInicio}");

            var fechaLocal = ConvertirAHoraLocalColombia(turno.FechaHoraInicio);

            string title = "📅 Turno Agendado";
            string body = $"Tienes un nuevo turno con {turno.ClienteNombre} el {fechaLocal:dd/MM/yyyy} a las {fechaLocal:hh:mm tt}. Servicio: {turno.ServicioNombre}";

            var message = new Message
            {
                Token = token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        ChannelId = "turno_notificaciones",
                        Sound = "default",
                        ClickAction = "FLUTTER_NOTIFICATION_CLICK"
                    }
                },
                Data = new Dictionary<string, string>
                {
                    { "tipo", "ACTUALIZAR_TURNO" },
                    { "TurnoId", turno.Id.ToString() },
                    { "BarberoId", turno.BarberoId.ToString() },
                    { "ClienteId", turno.ClienteId.ToString() },
                    { "FechaHoraInicio", fechaLocal.ToString("yyyy-MM-dd HH:mm:ss") },
                    { "Estado", turno.Estado.ToString() },
                    { "ClienteNombre", turno.ClienteNombre ?? "" },
                    { "ClienteApellido", turno.ClienteApellido ?? "" },
                    { "ServicioNombre", turno.ServicioNombre ?? "" },
                    { "Duracion", turno.Duracion.ToString() },
                    { "title", title },
                    { "body", body },
                    { "uuid", Guid.NewGuid().ToString() } // 🔥 Evita colapsos
                }
            };

            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine($"✅ Notificación enviada con éxito. ID: {response}");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error enviando notificación: {ex.Message}");
                return "ERROR";
            }
        }

        public async Task EnviarNotificacionCancelacionClienteAsync(string token, TurnoDTO turno, string motivo)
        {
            string title = "Turno Cancelado";
            string body = $"Tu turno con {turno.BarberoNombre} fue cancelado. Motivo: {motivo}";
            await EnviarMensajePersonalizado(token, turno, title, body);
        }

        public async Task EnviarNotificacionCancelacionBarberoAsync(string token, TurnoDTO turno, string motivo)
        {
            string title = "Turno Cancelado por Cliente";
            string body = $"El cliente {turno.ClienteNombre} canceló su turno. Motivo: {motivo}";
            await EnviarMensajePersonalizado(token, turno, title, body);
        }

        public async Task EnviarNotificacionCancelacionPorBarberoAsync(string token, TurnoDTO turno, string motivo)
        {
            string title = "Turno Cancelado por Barbero";
            string body = $"Tu turno fue cancelado por el barbero {turno.BarberoNombre}. Motivo: {motivo}";
            await EnviarMensajePersonalizado(token, turno, title, body);
        }

        public async Task EnviarNotificacionCambioEstadoAsync(string token, TurnoDTO turno)
        {
            var fechaLocal = ConvertirAHoraLocalColombia(turno.FechaHoraInicio);

            string estadoTexto = turno.Estado switch
            {
                EstadoTurno.EnProceso => "🟠 Tu turno ha comenzado.",
                EstadoTurno.Cerrado => "✅ Tu turno ha finalizado.",
                _ => "📢 Actualización del turno."
            };

            string titulo = "📣 Notificación de Turno";
            string cuerpo = $"{estadoTexto} Servicio: {turno.ServicioNombre}, Cliente: {turno.ClienteNombre}";

            var message = new Message
            {
                Token = token,
                Notification = new Notification
                {
                    Title = titulo,
                    Body = cuerpo
                },
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        ChannelId = "turno_notificaciones",
                        Sound = "default",
                        ClickAction = "FLUTTER_NOTIFICATION_CLICK"
                    }
                },
                Data = new Dictionary<string, string>
                {
                    { "tipo", "ACTUALIZAR_TURNO" },
                    { "TurnoId", turno.Id.ToString() },
                    { "BarberoId", turno.BarberoId.ToString() },
                    { "ClienteId", turno.ClienteId.ToString() },
                    { "FechaHoraInicio", fechaLocal.ToString("yyyy-MM-dd HH:mm:ss") },
                    { "Estado", turno.Estado.ToString() },
                    { "ClienteNombre", turno.ClienteNombre ?? "" },
                    { "ClienteApellido", turno.ClienteApellido ?? "" },
                    { "ServicioNombre", turno.ServicioNombre ?? "" },
                    { "Duracion", turno.Duracion.ToString() },
                    { "uuid", Guid.NewGuid().ToString() }
                }
            };

            await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }

        private async Task EnviarMensajePersonalizado(string token, TurnoDTO turno, string title, string body)
        {
            var fechaLocal = ConvertirAHoraLocalColombia(turno.FechaHoraInicio);

            var message = new Message()
            {
                Token = token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        ChannelId = "turno_notificaciones",
                        Sound = "default",
                        ClickAction = "FLUTTER_NOTIFICATION_CLICK"
                    }
                },
                Data = new Dictionary<string, string>
                {
                    { "TurnoId", turno.Id.ToString() },
                    { "BarberoId", turno.BarberoId.ToString() },
                    { "ClienteId", turno.ClienteId.ToString() },
                    { "FechaHoraInicio", fechaLocal.ToString("yyyy-MM-dd HH:mm:ss") },
                    { "Estado", turno.Estado.ToString() },
                    { "ClienteNombre", turno.ClienteNombre ?? "" },
                    { "ClienteApellido", turno.ClienteApellido ?? "" },
                    { "ServicioNombre", turno.ServicioNombre ?? "" },
                    { "Duracion", turno.Duracion.ToString() },
                    { "uuid", Guid.NewGuid().ToString() }
                }
            };

            await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }

        private DateTime ConvertirAHoraLocalColombia(DateTime fechaUtc)
        {
            TimeZoneInfo zonaColombia = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.SpecifyKind(fechaUtc, DateTimeKind.Utc),
                zonaColombia
            );
        }
    }
}

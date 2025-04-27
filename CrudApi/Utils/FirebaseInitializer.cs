using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.Text;

namespace CrudApi.Utils
{
    public static class FirebaseInitializer
    {
        private static bool _isInitialized = false;

        public static void InicializarFirebase(string rutaCredenciales)
        {
            if (!_isInitialized && FirebaseApp.DefaultInstance == null)
            {
                var firebaseCredentialsJson = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_JSON");

                if (!string.IsNullOrEmpty(firebaseCredentialsJson))
                {
                    // 🟠 Estamos en Render (o en entorno con variable)
                    GoogleCredential credential;

                    if (IsBase64String(firebaseCredentialsJson))
                    {
                        var decodedJson = Encoding.UTF8.GetString(Convert.FromBase64String(firebaseCredentialsJson));
                        credential = GoogleCredential.FromJson(decodedJson);
                        Console.WriteLine("✅ Firebase inicializado desde variable de entorno (Base64)");
                    }
                    else
                    {
                        credential = GoogleCredential.FromJson(firebaseCredentialsJson);
                        Console.WriteLine("✅ Firebase inicializado desde variable de entorno (JSON plano)");
                    }

                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = credential
                    });
                }
                else if (!string.IsNullOrEmpty(rutaCredenciales))
                {
                    // 🟢 Estamos en local
                    var credential = GoogleCredential.FromFile(rutaCredenciales);
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = credential
                    });
                    Console.WriteLine("✅ Firebase inicializado desde archivo local");
                }
                else
                {
                    throw new Exception("⚠️ No se encontró configuración válida de Firebase.");
                }

                _isInitialized = true;
            }
        }

        private static bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }
    }
}

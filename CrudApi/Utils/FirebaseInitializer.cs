using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace CrudApi.Utils
{
    public static class FirebaseInitializer
    {
        private static bool _isInitialized = false;

        public static void InicializarFirebase(string rutaCredenciales)
        {
            if (!_isInitialized && FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(rutaCredenciales)
                });

                _isInitialized = true;
            }
        }
    }
}

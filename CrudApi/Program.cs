using CrudApi.Data;
using CrudApi.Utils;
using CrudApi.Services;
using CrudApi.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using CrudApi.Models;
using Hangfire;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using TuProyectoNamespace.Services; // Ajusta si tu namespace es otro

var builder = WebApplication.CreateBuilder(args);

// 🔹 Configurar appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// 🔹 Configurar variables de entorno
builder.Configuration.AddEnvironmentVariables();

// 🔥 Inicializar Firebase con la nueva clave
if (FirebaseApp.DefaultInstance == null)
{
    FirebaseInitializer.InicializarFirebase("barberapp-notifications-firebase-adminsdk-fbsvc-1933931b2e.json");
    Console.WriteLine("✅ Firebase inicializado desde archivo local (nuevo JSON)");
}

// 🔹 Obtener la cadena de conexión
var connectionString = builder.Configuration["DefaultConnection"]
                    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("⚠️ La cadena de conexión no está definida.");
}

// 🔹 Configurar EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 🔹 Registrar servicios
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IHorarioService, HorarioService>();
builder.Services.AddScoped<ITurnoService, TurnoService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IBarberiaService, BarberiaService>();
builder.Services.AddScoped<IBarberoService, BarberoService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ITipoDocumentoService, TipoDocumentoService>();
builder.Services.AddScoped<CrudApi.Notifications.Notifications>();
builder.Services.AddScoped<ISucursalBarberiaService, SucursalBarberiaService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddTransient<EmailService>();
builder.Services.AddScoped<IHorarioBloqueadoService, HorarioBloqueadoService>();


// 🔹 Configurar Hangfire
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();

// 🔹 Configurar controladores
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
    });

// 🔹 Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// 🔹 Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API CrudApi", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer.\n\n" +
                      "Escribe 'Bearer' seguido de un espacio y luego tu token.\n\n" +
                      "Ejemplo: \"Bearer abcdef12345\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// 🔹 Configurar JWT
var jwtKey = builder.Configuration["JwtSettings:Key"];
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
var jwtAudience = builder.Configuration["JwtSettings:Audience"];

if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
{
    throw new Exception("⚠️ La configuración JWT (Key, Issuer o Audience) no está definida.");
}

var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        ValidateLifetime = true
    };
});

builder.Services.AddAuthorization();

// 🔹 Construir aplicación
var app = builder.Build();

// 🔹 Middleware
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// 🔹 Swagger accesible desde la raíz
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API CrudApi v1");
    c.RoutePrefix = string.Empty;
});

// 🔹 Mapear controladores
app.MapControllers();

// 🔹 Activar Dashboard de Hangfire
app.UseHangfireDashboard();

// 🔁 Tarea recurrente: actualizar estados de turnos
RecurringJob.AddOrUpdate<IShiftService>(
    "actualizar-estados-turnos",
    x => x.CerrarTurnosVencidosAsync(),
    Cron.Minutely
);

// 🔹 Ejecutar aplicación
app.Run();

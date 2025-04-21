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
using Hangfire.Dashboard;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Configurar appsettings.json (opcional en Render)
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// ✅ Obtener la cadena de conexión desde el entorno (Render usa ENV)
var dbConnectionString = builder.Configuration["DefaultConnection"];

// 🔹 Configurar EF Core con SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(dbConnectionString));

// 🔹 Registrar servicios en la inyección de dependencias
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

// 🔹 Habilitar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

// 🔹 Configurar Hangfire
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(dbConnectionString));
builder.Services.AddHangfireServer();

// 🔹 Configurar controladores y opciones JSON
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonDateConverter());
    options.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddEndpointsApiExplorer();

// 🔹 Configurar autenticación con JWT
var secretKey = builder.Configuration["JwtSettings:Key"];
var issuer = builder.Configuration["JwtSettings:Issuer"];
var audience = builder.Configuration["JwtSettings:Audience"];

if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
{
    throw new Exception("⚠️ La configuración JWT (Key, Issuer o Audience) no está definida.");
}

var key = Encoding.UTF8.GetBytes(secretKey);

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
        ValidIssuer = issuer,
        ValidAudience = audience,
        ValidateLifetime = true
    };
});

builder.Services.AddAuthorization();

// 🔹 Configurar Swagger con autenticación JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API CrudApi", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. \n\n" +
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

var app = builder.Build();

// 🔹 Middleware
app.UseStaticFiles();
// app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// ✅ Swagger accesible desde la raíz
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

// 🔁 Tarea recurrente: actualizar turnos automáticamente
RecurringJob.AddOrUpdate<IShiftService>(
    "actualizar-estados-turnos",
    x => x.CerrarTurnosVencidosAsync(),
    Cron.Minutely
);

app.Run();

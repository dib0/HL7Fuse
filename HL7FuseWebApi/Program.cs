using HL7FuseWebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

// Configurazione di Serilog con Serilog.Sinks.RollingFile
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/log-.txt",               // Cartella "logs" nella root dell'app
        rollingInterval: RollingInterval.Day, // Crea un nuovo file ogni giorno (es. log-20261121.txt)
        retainedFileCountLimit: 30,           // Mantiene gli ultimi 30 giorni di log
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    // Collega Serilog all'host ASP.NET Core
    builder.Host.UseSerilog();

    // 1. Aggiunge i controller e abilita la lettura di stringhe raw (text/plain e application/hl7-v2) nel Body
    builder.Services.AddControllers(options =>
    {
        options.InputFormatters.Insert(0, new RawStringInputFormatter());
    });

    // 2. Configurazione Autenticazione JWT Bearer
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var key = Encoding.UTF8.GetBytes(jwtSettings["Client_Secret"]!);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

    builder.Services.AddAuthorization();

    // Add services to the container.

    //builder.Services.AddControllers();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    var app = builder.Build();

    // Registra le richieste HTTP in ingresso
    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();

    // 3. L'ordine è fondamentale: prima Authentication, poi Authorization
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "L'applicazione si è arrestata in modo imprevisto");
}
finally
{
    Log.CloseAndFlush();
}
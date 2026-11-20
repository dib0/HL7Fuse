using HL7Fuse.Services;
using HL7Fuse.Services.Protocol;
using HL7FuseWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NHapi.Base.Model;
using NHapi.Base.Parser;
using SuperSocket.SocketBase.Command;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace HL7FuseWebApi.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[Produces("application/json")]
[ApiController]
[Route("api/[controller]")]
public class HL7Controller : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    public HL7Controller(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] Login login)
    {
        // Simulazione asincrona della verifica credenziali (sostituire con query su DB)
        await Task.CompletedTask;

        if (login == null || String.IsNullOrEmpty(login.Client_Id) || String.IsNullOrEmpty(login.Client_Secret) || String.IsNullOrEmpty(login.Grant_Type))
            return Unauthorized(new { message = "Credenziali vuote." });
        if (login.Client_Id != _configuration["Jwt:Client_Id"] || login.Scope != _configuration["Jwt:Scope"] || login.Grant_Type != _configuration["Jwt:Grant_Type"])
            return Unauthorized(new { message = "Credenziali non valide." });
        else
        {
            // Generazione del token JWT
            var jwtSettings = _configuration.GetSection("Jwt");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Client_Secret"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, login.Client_Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, login.Client_Id)
            };
            var expireTimeSecs = Convert.ToInt32(jwtSettings["Expire_Time"]);
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(expireTimeSecs),
                signingCredentials: credentials
            );
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.SetDefaultTimesOnTokenCreation = false;
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                access_token = tokenString,
                token_type = "Bearer",
                expires_in = expireTimeSecs
            });
        }
    }

    [Authorize]
    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] String HL7Message)
    {
        if (string.IsNullOrWhiteSpace(HL7Message))
        {
            return BadRequest(new { message = "Il messaggio HL7 non può essere vuoto." });
        }

        PipeParser parser = new PipeParser();
        IMessage hl7ToProcessMessage = parser.Parse(HL7Message);

        // 1. Legge il nome dell'assembly da appsettings.json
        string? assemblyName = _configuration["commandAssemblies:assembly"];
        if (string.IsNullOrWhiteSpace(assemblyName))
            return BadRequest("Configurazione 'commandAssemblies:assembly' mancante.");

        // 2. Carica l'assembly
        Assembly assembly = Assembly.Load(assemblyName);

        // 3. Recupera il tipo (sostituisci con il nome completo della tua classe o cercala per interfaccia)
        Type interfaceType = typeof(ICommand<MLLPSession, HL7RequestInfo>);
        Type? targetType = assembly.GetTypes()
            .FirstOrDefault(t => interfaceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

        if (targetType == null)
            return NotFound($"Nessuna classe che implementa {interfaceType.Name} trovata in {assemblyName}.");

        // 4. Istanzia la classe risolvendo eventuali dipendenze dal container DI
        ICommand<MLLPSession, HL7RequestInfo> commandInstance = (ICommand<MLLPSession, HL7RequestInfo>)ActivatorUtilities.CreateInstance(_serviceProvider, targetType);
        HL7RequestInfo hL7RequestInfo = new HL7RequestInfo()
        {
             ErrorMessage = null,
             Key = Guid.NewGuid().ToString(),
             Message = hl7ToProcessMessage
        };
        commandInstance.ExecuteCommand(null, hL7RequestInfo);

        // Qui inserirai la tua logica di parsing o inoltro del messaggio HL7
        await Task.CompletedTask;
        return Ok(new
        {
            WasUnknownRequest = hL7RequestInfo.WasUnknownRequest,
            ErrorMessage = hL7RequestInfo.ErrorMessage,
            ResponseMessage = parser.Encode(hL7RequestInfo.ResponseMessage)
        });
    }
}

using Microsoft.AspNetCore.Mvc;
using uServiceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace uServiceAPI.Controllers;


[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
   
    private readonly UServiceContext _uServiceDBContext;
    private readonly JwtSettings _JwtSettings;

    public AuthenticationController(UServiceContext uServiceDBContext,IOptions<JwtSettings>options)
    {
        _uServiceDBContext = uServiceDBContext;
        _JwtSettings=options.Value;
    }

   

    [HttpPost("GetToken")]
    public IActionResult GetToken([FromBody] UserToken userToken)
    {
        // Verifica las credenciales (puedes implementar tu lógica aquí)

        // Si las credenciales son válidas, genera un token JWT
        if (IsValidUser(userToken.Login) && userToken.Password == this._JwtSettings.SecurityKey)
        {
            var token = GenerateJwtToken(userToken.Login);
            return Ok(new { token });
        }

        return Unauthorized();
    }

    [Authorize]
    [HttpGet("GetSecureData")]
    public IActionResult GetSecureData()
    {
        // Esta acción solo es accesible para usuarios autenticados
        return Ok("Esta es información segura.");
    }

    private bool IsValidUser(string username)
    {
        var user = this._uServiceDBContext.Users.FirstOrDefault(item => item.Login == username);

        return user != null ? true : false;
    }

    private string GenerateJwtToken(string username)
    {
        var secretKey = _JwtSettings.SecurityKey;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1), // Duración del token
            signingCredentials: credentials
        );

        return tokenHandler.WriteToken(token);
    }
}


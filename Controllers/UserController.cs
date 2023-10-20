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
public class UserController : ControllerBase
{
   
    private readonly UServiceContext _uServiceDBContext;
    private readonly JwtSettings _JwtSettings;

    public UserController(UServiceContext uServiceDBContext,IOptions<JwtSettings>options)
    {
        _uServiceDBContext = uServiceDBContext;
        _JwtSettings=options.Value;
    }

    [Authorize]
    [HttpPost("GetUser")]
    public IActionResult GetUser([FromBody] string login)
    {
        var user=this._uServiceDBContext.Users.FirstOrDefault(item => item.Login == login);
        return Ok(user);
    }

    [Authorize]
    [HttpPost("login")]
    public IActionResult Login([FromBody] User credentials)
    {
        // Verifica las credenciales (puedes implementar tu lógica aquí)

        // Si las credenciales son válidas, genera un token JWT
        if (IsValidUser(credentials.Login, credentials.Password))
        {
            return Ok();
        }

        return Unauthorized();
    }

  
    private bool IsValidUser(string username, string password)
    {
        var user = this._uServiceDBContext.Users.FirstOrDefault(item => item.Login == username && item.Password == password);

        return user != null ? true : false;
    }

    private bool ExistUser(string username)
    {
        var user = this._uServiceDBContext.Users.FirstOrDefault(item => item.Login == username);

        return user != null ? true : false;
    }
    
    // [Authorize]
    [HttpPost("CreateUser")]
    public IActionResult CreateUser([FromBody] User user)
    {
        if (user == null)
        {
            return BadRequest("El objeto Usuario es nulo");
        }else if (ExistUser(user.Login)){

            return StatusCode(Conflict().StatusCode,user);
        }

        this._uServiceDBContext.Users.Add(user);
        this._uServiceDBContext.SaveChanges();

        return StatusCode(201,user);
    }
  
}


using System.Linq;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly DataContext _dataContext;
    private readonly ITokenService _tokenService;

    public AccountController(DataContext dataContext, ITokenService tokenService)
    {
        _dataContext = dataContext;
        _tokenService = tokenService;
    }

    [HttpPost("register")] // POST : api/account/register?username=jojo&password=pwd
    public async Task<ActionResult<UserDto>> RegisterAsync(RegisterDto registerDto){
        

        if (await UserNameExist(registerDto.UserName)) return BadRequest("User name Exist");
        

        using var hmac = new HMACSHA512();

        var user = new AppUser{
            Name = registerDto.UserName.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };

        _dataContext.Users.Add(user);
        await _dataContext.SaveChangesAsync();
        return new UserDto{
            UserName = user.Name,
            AuthToken = _tokenService.CreateToken(user)
        };

    }

    private async Task<bool> UserNameExist(string userName)
    {
        return await _dataContext.Users.AnyAsync(user => user.Name == userName.ToLower());
    }

    [HttpPost("login")] // POST : api/account/login
    public async Task<ActionResult<UserDto>> LoginAsync(LoginDto loginDto){
        var user = await _dataContext.Users.SingleOrDefaultAsync(user => user.Name == loginDto.UserName.ToLower());

        return user == default ? Unauthorized("User name does not exist") :
                UserLogin(user, loginDto);
        
    }

    private ActionResult<UserDto> UserLogin(AppUser user, LoginDto loginDto)
    {
        using var hmac = new HMACSHA512(user.PasswordSalt);
         var computedHash =  hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
         for (int i = 0; i < user.PasswordHash.Length; i++)
         {
            if (user.PasswordHash[i] != computedHash[i]) return Unauthorized("Wrong Password");
            
         }
         return  new UserDto{
            UserName = user.Name,
            AuthToken = _tokenService.CreateToken(user)
        };;
    }
}


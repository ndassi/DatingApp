using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt ;
using System.Security.Claims;
using System.Text;

namespace API.Service;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _symmetricSecurityKey;
    public TokenService(IConfiguration config)
    {
        string _key = config["SecurityTokenKey"];
        byte[] _keyBytes  = Encoding.UTF8.GetBytes(_key);
        _symmetricSecurityKey = new SymmetricSecurityKey(_keyBytes);
    }
    public string CreateToken(AppUser appUser)
    {
       
        var signinCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
        List<Claim> claims = new(){
            new Claim("Name", appUser.Name)
        };
        // var tokeOptions = new JwtSecurityToken(
        //     // issuer: "https://localhost:5001",
        //     // audience: "https://localhost:5001",
        //     claims: claims,
        //     expires: DateTime.Now.AddDays(7),
        //     signingCredentials: signinCredentials
        // );
        var securityDes = new SecurityTokenDescriptor{
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7), 
            SigningCredentials = signinCredentials

        };

        var token = new JwtSecurityTokenHandler().CreateToken(securityDes);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenString;
    }
}

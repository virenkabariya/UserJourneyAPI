namespace UserJourney.API.Services
{
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using UserJourney.API.Code;

    public class JwtService
    {
        public static string GenerateSecurityToken(Dictionary<string, object> authClaims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Context.Configuration["JwtConfig:Secret"])), SecurityAlgorithms.HmacSha256),
                Issuer = Context.Configuration["JwtConfig:Issuer"],
                Audience = Context.Configuration["JwtConfig:Audience"],
                Claims = authClaims,
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(Context.Configuration["JwtConfig:ExpirationMinutes"]))
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }

        public static JwtSecurityToken ReadSecurityToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.ReadToken(token) as JwtSecurityToken;
        }

    }
}
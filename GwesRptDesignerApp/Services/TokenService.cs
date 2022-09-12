using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
/*
 * Author       :       Sanjit Adhikary
 * Created On   :       08-SEPTEMBER-2022
 */
namespace GwesRptDesignerApp.Services
{
    public class TokenService : ITokenService
    {
        public IConfiguration Configuration { get; }
        public TokenService(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("AppSettings:Secret")));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var durationInMinutes = int.Parse(Configuration.GetValue<string>("AppSettings:DurationInMinutes"));
            var tokeOptions = new JwtSecurityToken(
                issuer: Configuration.GetValue<string>("AppSettings:ValidIssuer"),
                audience: Configuration.GetValue<string>("AppSettings:ValidAudience"),
                claims: claims,
                expires: DateTime.Now.AddMinutes(durationInMinutes),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return tokenString;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            bool validateLifeTime = false;
            if (Configuration.GetValue<string>("AppSettings:ValidateLifetime") == "true") { validateLifeTime = true; }
            bool validateIssuerSigningKey = false;
            if (Configuration.GetValue<string>("AppSettings:ValidateIssuerSigningKey") == "true") { validateIssuerSigningKey = true; }
            bool validateAudience = false;
            if (Configuration.GetValue<string>("AppSettings:ValidateAudience") == "true") { validateAudience = true; }
            bool validateIssuer = false;
            if (Configuration.GetValue<string>("AppSettings:ValidateIssuer") == "true") { validateIssuer = true; }
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = validateAudience, 
                ValidateIssuer = validateIssuer,
                ValidateIssuerSigningKey = validateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("AppSettings:Secret"))),
                ValidateLifetime = validateLifeTime 
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}

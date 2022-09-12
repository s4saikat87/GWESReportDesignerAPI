using GwesRptDesignerApp.Models;
using GwesRptDesignerApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GwesRptDesignerLib;
using System.Data;
using Microsoft.EntityFrameworkCore;
/*
 * Author       :       Sanjit Adhikary
 * Created On   :       08-SEPTEMBER-2022
 */
namespace GwesRptDesignerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserContext _userContext;
        private readonly ITokenService _tokenService;

        public AuthController(UserContext userContext, ITokenService tokenService)
        {
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [HttpPost, Route("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            if (loginModel is null)
            {
                return BadRequest("Invalid request");
            }

            string gwesConnectionString = _userContext.Database.GetConnectionString();

            var dataAccessLayer = new DataAccessLayer();
            var dsUser = new DataSet();
            if (!loginModel.Password.Contains("==MS"))
            {
                var securityLayer = new SecurityLayer();
                loginModel.Password = securityLayer.SHA256Encrypt(loginModel.Password);
                dsUser = dataAccessLayer.DsValidateUser(gwesConnectionString, loginModel.UserName, loginModel.Password);
            }
            else
            {
                dsUser = dataAccessLayer.DsValidateUser(gwesConnectionString, loginModel.UserName);
            }
            int cntctId = 0;
            if (dsUser.Tables.Count>0 && dsUser.Tables[0].Rows.Count>0 && dsUser.Tables[0].TableName== "Results")
            {
                cntctId = int.Parse(dsUser.Tables[0].Rows[0][2].ToString());
            }
            else
            {
                return Unauthorized();
            }
           

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginModel.UserName),
                new Claim(ClaimTypes.Role, "Manager")
            };
            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            DateTime refreshTokenExpiryTime= DateTime.Now.AddDays(7);

            var dsRefreshTokenSet = new DataSet();
            dsRefreshTokenSet=dataAccessLayer.DsRefreshTokenSet(gwesConnectionString,cntctId,refreshToken,refreshTokenExpiryTime);
            if (dsRefreshTokenSet.Tables.Count > 0 && dsRefreshTokenSet.Tables[0].Rows.Count > 0 && dsRefreshTokenSet.Tables[0].TableName == "Results")
            {
                return Ok(new AuthenticatedResponse
                {
                    Token = accessToken,
                    RefreshToken = refreshToken
                });
            }
            else
            {
                return Unauthorized();
            }

            
        }
    }
}

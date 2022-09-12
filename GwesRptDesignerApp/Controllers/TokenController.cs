using GwesRptDesignerApp.Models;
using GwesRptDesignerApp.Services;
using GwesRptDesignerLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
/*
 * Author       :       Sanjit Adhikary
 * Created On   :       08-SEPTEMBER-2022
 */ 

namespace GwesRptDesignerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UserContext _userContext;
        private readonly ITokenService _tokenService;
        public IConfiguration Configuration { get; }
        
        public TokenController(UserContext userContext, ITokenService tokenService, IConfiguration configuration)
        {
            this._userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            this._tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            Configuration = configuration;
        }

        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh(TokenApiModel tokenApiModel)
        {
            if (tokenApiModel is null)
                return BadRequest("Invalid client request");

            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;

            string gwesConnectionString = _userContext.Database.GetConnectionString();

            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name; //this is mapped to the Name claim by default

            var dataAccessLayer = new DataAccessLayer();
            var dataSetRefreshToken=new DataSet();
            dataSetRefreshToken = dataAccessLayer.DsRefreshTokenGet(gwesConnectionString, username);

            if (dataSetRefreshToken.Tables.Count<=0 || dataSetRefreshToken.Tables[0].Rows.Count<=0
                || dataSetRefreshToken.Tables[0].TableName== "Error" || dataSetRefreshToken.Tables[0].Rows[0][1].ToString()!= refreshToken
                ||DateTime.Parse(dataSetRefreshToken.Tables[0].Rows[0][2].ToString())<=DateTime.Now)
                       
                return BadRequest("Invalid client request");

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            int cntctId = int.Parse(dataSetRefreshToken.Tables[0].Rows[0][0].ToString());
            DateTime refreshTokenExpiryTime = DateTime.Now.AddHours(int.Parse(Configuration.GetValue<string>("AppSettings:DurationInHours")));
            var dsRefreshTokenSet = dataAccessLayer.DsRefreshTokenSet(gwesConnectionString, cntctId, newRefreshToken, refreshTokenExpiryTime);
            if (dsRefreshTokenSet.Tables.Count > 0 && dsRefreshTokenSet.Tables[0].Rows.Count > 0 && dsRefreshTokenSet.Tables[0].TableName == "Results")
            {
                return Ok(new AuthenticatedResponse
                {
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }
            else
            {
                return Unauthorized();
            }

            
        }

        [HttpPost, Authorize]
        [Route("revoke")]
        public IActionResult Revoke()
        {
            string gwesConnectionString = _userContext.Database.GetConnectionString();
            var username = User.Identity.Name;
            var dataAccessLayer = new DataAccessLayer();
            var dataSetRefreshToken = new DataSet();
            dataSetRefreshToken = dataAccessLayer.DsRefreshTokenGet(gwesConnectionString, username);
            if (dataSetRefreshToken.Tables.Count <= 0 || dataSetRefreshToken.Tables[0].Rows.Count <= 0
                || dataSetRefreshToken.Tables[0].TableName == "Error")
                
            return BadRequest();

            int cntctId = int.Parse(dataSetRefreshToken.Tables[0].Rows[0][0].ToString());
            var dsRefreshTokenSet = dataAccessLayer.DsRefreshTokenDel(gwesConnectionString, cntctId);
            if (dsRefreshTokenSet.Tables.Count > 0 && dsRefreshTokenSet.Tables[0].Rows.Count > 0 && dsRefreshTokenSet.Tables[0].TableName == "Results")
            { return NoContent(); }
            else { return Unauthorized(); }
        }
    }
}

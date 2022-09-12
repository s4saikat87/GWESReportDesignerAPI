/*
 * Author       :       Sanjit Adhikary
 * Created On   :       08-SEPTEMBER-2022
 */
namespace GwesRptDesignerApp.Models
{
    public class AuthenticatedResponse
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}

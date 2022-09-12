using Microsoft.EntityFrameworkCore;
/*
 * Author       :       Sanjit Adhikary
 * Created On   :       08-SEPTEMBER-2022
 */
namespace GwesRptDesignerApp.Models
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {
        }

        
    }
}

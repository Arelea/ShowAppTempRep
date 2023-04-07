using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppNov14.Web.Models
{
    public class ApplicationContext :  IdentityDbContext<AppNov14.Web.Models.Users.Users>
    {
         
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();   
        }
    }
}
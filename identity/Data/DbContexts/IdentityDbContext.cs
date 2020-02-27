using identity.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace identity.Data.DbContexts
{
    public class IdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        #region Ctor
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options) { }
        #endregion
    }
}

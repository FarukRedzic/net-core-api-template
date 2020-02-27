using api.model.Entities.TemplateDbContext;
using Microsoft.EntityFrameworkCore;

namespace api.database.DbContexts.TemplateDb
{
    public class TemplateDbContext : DbContext
    {
        #region Ctor
        public TemplateDbContext(DbContextOptions<TemplateDbContext> options) 
            : base(options)
        {

        }
        #endregion

        #region DbSets
        public DbSet<TemplateEntity> TemplateTable { get; set; }
        #endregion

        /// <summary>
        /// Use this method for FluentAPI configuration.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

using System.Data.Entity;
using System.Text;
using IdeaBlade.Application.Framework.Core.Persistence;
using IdeaBlade.EntityModel;

namespace Security
{
    [DataSourceKeyName("SecurityEntities")]
    internal class SecurityDbContext : DbContext
    {
        public SecurityDbContext(string connection = null) : base(connection)
        {
            Database.SetInitializer(new SecurityDbInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<EntityAspect>();
        }

        public DbSet<User> Users { get; set; }
    }

    internal class SecurityDbInitializer : DropCreateDatabaseIfModelChanges<SecurityDbContext>
    {
        protected override void Seed(SecurityDbContext context)
        {
            var password = Encoding.UTF8.GetString(CryptoHelper.GenerateKey("password"));
            var user = new User() {Id = CombGuid.NewGuid(), Username = "Admin", Password = password};

            context.Users.Add(user);
        }
    }
}
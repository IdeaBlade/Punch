//====================================================================================================================
// Copyright (c) 2012 IdeaBlade
//====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
// USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
// http://cocktail.ideablade.com/licensing
//====================================================================================================================

using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Text;
using Cocktail;
using IdeaBlade.EntityModel;

namespace Security
{
    [DataSourceKeyName("SecurityEntities")]
    internal class SecurityDbContext : DbContext
    {
        static SecurityDbContext()
        {
            // This is currently a DevForce requirement in order to use SLQ CE with Code-First.
            // See http://drc.ideablade.com/xwiki/bin/view/Documentation/code-first-sqlce
            // Remove if not using SQL CE.
            Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
        }

        public SecurityDbContext(string connection = null)
            : base(connection)
        {
            Database.SetInitializer(new SecurityDbInitializer());

            // DevForce already performs validation
            Configuration.ValidateOnSaveEnabled = false;
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
            var user = new User { Id = CombGuid.NewGuid(), Username = "Admin", Password = password };

            context.Users.Add(user);
        }
    }
}
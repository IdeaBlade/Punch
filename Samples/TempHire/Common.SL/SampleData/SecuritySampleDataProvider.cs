using System.ComponentModel.Composition;
using System.Text;
using Cocktail;
using DomainModel;
using Security;

namespace Common.SampleData
{
    /// <summary>
    /// Security sample data. Will be loaded TempHireEntities into the same fake store
    /// </summary>
    [Export(typeof(ISampleDataProvider<TempHireEntities>))]
    public class SecuritySampleDataProvider : ISampleDataProvider<TempHireEntities>
    {
        public void AddSampleData(TempHireEntities manager)
        {
            var hash = CryptoHelper.GenerateKey("password");
            var password = Encoding.UTF8.GetString(hash, 0, hash.Length);

            var user = new User
                           {
                               Id = CombGuid.NewGuid(),
                               Username = "Admin",
                               Password = password
                           };
            manager.AttachEntity(user);
        }
    }
}
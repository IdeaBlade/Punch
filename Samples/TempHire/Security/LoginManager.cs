using System.Linq;
using System.Security.Principal;
using IdeaBlade.EntityModel;

namespace Security
{
    /// <summary>
    /// Production login manager
    /// </summary>
    public class LoginManager : IEntityLoginManager
    {
        #region IEntityLoginManager Members

        public virtual IPrincipal Login(ILoginCredential credential, EntityManager entityManager)
        {
            if (credential == null)
                throw new LoginException(LoginExceptionType.NoCredentials, "Credentials are required.");

            if (string.IsNullOrWhiteSpace(credential.UserName))
                throw new LoginException(LoginExceptionType.InvalidUserName, "Username cannot be empty.");

            if (string.IsNullOrWhiteSpace(credential.Password))
                throw new LoginException(LoginExceptionType.InvalidPassword, "Password cannot be empty.");

            var em = new SecurityEntities(entityManager);
            User user =
                em.Users.FirstOrDefault(
                    u => u.Username.ToUpper() == credential.UserName.ToUpper() && u.Password == credential.Password);

            if (user == null)
                throw new LoginException(LoginExceptionType.InvalidPassword, credential.Domain, credential.UserName);

            return new UserBase(new UserIdentity(user.Username, "FORM", true));
        }

        public virtual void Logout(IPrincipal principal, EntityManager entityManager)
        {
        }

        #endregion
    }
}
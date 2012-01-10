using System.Security.Principal;

namespace Security.Messages
{
    public class LoggedInMessage
    {
        public LoggedInMessage(IPrincipal principal)
        {
            Principal = principal;
        }

        public IPrincipal Principal { get; private set; }
    }
}
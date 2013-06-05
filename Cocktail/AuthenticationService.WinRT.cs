// ====================================================================================================================
//   Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//   OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//   OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//   USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//   http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Principal;
using Caliburn.Micro;
using IdeaBlade.EntityModel;
using IdeaBlade.EntityModel.Security;

namespace Cocktail
{
    public partial class AuthenticationService : IHandle<Suspending>, IHandle<Restoring>
    {
        void IHandle<Restoring>.Handle(Restoring message)
        {
            if (message.SessionState.ContainsKey("AuthenticationContext"))
                _authenticationContext = (IAuthenticationContext) message.SessionState["AuthenticationContext"];
        }

        void IHandle<Suspending>.Handle(Suspending message)
        {
            message.SessionState["AuthenticationContext"] = new SerializableAuthenticationContext(_authenticationContext);
        }
    }

    /// <summary>
    ///     Internal use only.
    /// </summary>
    [DataContract]
    internal class SerializableLoginOptions
    {
        internal SerializableLoginOptions(LoginOptions loginOptions)
        {
            CompositionContextName = loginOptions.CompositionContextName;
            DataSourceExtension = loginOptions.DataSourceExtension;
            ServiceKey = loginOptions.ServiceKey;
            UsesDistributedEntityService = loginOptions.UsesDistributedEntityService;
        }

        [DataMember]
        internal bool UsesDistributedEntityService { get; set; }

        [DataMember]
        internal string ServiceKey { get; set; }

        [DataMember]
        internal string DataSourceExtension { get; set; }

        [DataMember]
        internal string CompositionContextName { get; set; }
    }

    /// <summary>
    ///     Internal use only.
    /// </summary>
    [DataContract]
    internal class SerializableAuthenticationContext : IAuthenticationContext
    {
        internal SerializableAuthenticationContext(IAuthenticationContext authenticationContext)
        {
            SessionKey = authenticationContext.SessionKey;
            Principal = authenticationContext.Principal;
            LoginState = authenticationContext.LoginState;
            ExtendedPropertyMap = EncodeExtendedPropertyMap(authenticationContext.ExtendedPropertyMap);
        }

        [DataMember]
        public IDictionary<string, object> ExtendedPropertyMapForSerialization
        {
            get { return EncodeExtendedPropertyMap(ExtendedPropertyMap); }
            internal set { ExtendedPropertyMap = DecodeExtendedPropertyMap(value); }
        }

        /// <summary>
        ///     Token uniquely identifying a user session to the Entity Server.
        /// </summary>
        [DataMember]
        public Guid SessionKey { get; internal set; }

        /// <summary>
        ///     The <see cref="T:System.Security.Principal.IPrincipal" /> representing the logged in user.
        /// </summary>
        [DataMember]
        public IPrincipal Principal { get; internal set; }

        /// <summary>
        ///     Returns whether this context is logged in.
        /// </summary>
        [DataMember]
        public LoginState LoginState { get; internal set; }

        /// <summary>
        ///     Additional properties.
        /// </summary>
        public IDictionary<string, object> ExtendedPropertyMap { get; internal set; }

        private IDictionary<string, object> EncodeExtendedPropertyMap(IDictionary<string, object> source)
        {
            var extendedPropertyMap = new Dictionary<string, object>();
            foreach (var key in source.Keys)
            {
                var value = source[key];
                if (value is LoginOptions)
                    value = new SerializableLoginOptions((LoginOptions) value);

                extendedPropertyMap[key] = value;
            }

            return extendedPropertyMap;
        }

        private IDictionary<string, object> DecodeExtendedPropertyMap(IDictionary<string, object> source)
        {
            var extendedPropertyMap = new Dictionary<string, object>();
            foreach (var key in source.Keys)
            {
                var value = source[key];
                if (value is SerializableLoginOptions)
                {
                    var loginOptions = (SerializableLoginOptions) value;
                    value = new LoginOptions(loginOptions.DataSourceExtension, loginOptions.CompositionContextName,
                                             loginOptions.UsesDistributedEntityService
                                                 ? EntityServiceOption.UseDistributedService
                                                 : EntityServiceOption.UseLocalService, loginOptions.ServiceKey);
                }

                extendedPropertyMap[key] = value;
            }

            return extendedPropertyMap;
        }
    }
}
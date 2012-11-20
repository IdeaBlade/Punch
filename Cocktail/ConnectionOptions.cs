// ====================================================================================================================
//  Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//  WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//  OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//  OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//  USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//  http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;
using IdeaBlade.EntityModel.Security;
using CompositionContext = IdeaBlade.Core.Composition.CompositionContext;

#if NETFX_CORE
using System.Composition;
#else
using System.ComponentModel.Composition;
#endif

namespace Cocktail
{
    /// <summary>
    /// Holds EntityServer connection parameters independent of an EntityManagerProvider.
    /// </summary>
    /// <example>This example shows how to configure and export and EntityManagerProvider using the fake store.
    /// <code>
    ///     public class EntityManagerProviderFactory
    ///     {
    ///         [Export]
    ///         public IEntityManagerProvider&lt;TempHireEntities&gt; TempHireEntityManagerProvider
    ///         {
    ///             get
    ///             {
    ///                 return new EntityManagerProvider&lt;TempHireEntities&gt;().With(ConnectionOptions.Fake.Name);
    ///             }
    ///         }
    ///     }
    /// </code>
    /// </example>
    public sealed class ConnectionOptions
    {
        /// <summary>
        /// The default connection options. Used when nothing else is specified.
        /// </summary>
        public static readonly ConnectionOptions Default = new ConnectionOptions("Default");

        /// <summary>
        /// The connection options used to connect to the fake backing store.
        /// </summary>
        public static readonly ConnectionOptions Fake
            = new ConnectionOptions("Fake", compositionContextName: CompositionContext.Fake.Name);

        /// <summary>
        /// The connection options used at design time.
        /// </summary>
        public static readonly ConnectionOptions DesignTime =
            new ConnectionOptions("DesignTime", false, isDesignTime: true);

        private string _name;
        private string _compositionContextName;

        /// <summary>
        /// Creates a ConnectionOptions instance with the given name and options.
        /// </summary>
        /// <param name="name">The name of the ConnectionOptions.</param>
        /// <param name="shouldConnect">Specifies whether the EntityManager will attempt to connect to the EntityServer as soon as it is created.</param>
        /// <param name="dataSourceExtension">Specifies what run-time data source key(s) will be used.</param>
        /// <param name="entityServiceOption">Specifies whether you will be using local or distributed data sources.</param>
        /// <param name="compositionContextName">The name of the <see cref="CompositionContext"/> used to resolve dependencies and extensions.</param>
        /// <param name="serviceKey">Names the key providing the address of the application server with which the EntityManager will communicate.</param>
        /// <param name="isDesignTime">Specifies whether this ConnectionOptions instance is for design time.</param>
        public ConnectionOptions(string name, bool shouldConnect = true, string dataSourceExtension = null,
                                 EntityServiceOption entityServiceOption = EntityServiceOption.UseDefaultService,
                                 string compositionContextName = null, string serviceKey = null,
                                 bool isDesignTime = false)
        {
            Name = name;
            ShouldConnect = shouldConnect;
            DataSourceExtension = dataSourceExtension;
            EntityServiceOption = entityServiceOption;
            _compositionContextName = compositionContextName;
            ServiceKey = serviceKey;
            IsDesignTime = isDesignTime;
        }

        private ConnectionOptions(ConnectionOptions connectionOptions)
        {
            //Name = connectionOptions.Name;
            ShouldConnect = connectionOptions.ShouldConnect;
            DataSourceExtension = connectionOptions.DataSourceExtension;
            EntityServiceOption = connectionOptions.EntityServiceOption;
            _compositionContextName = connectionOptions._compositionContextName;
            ServiceKey = connectionOptions.ServiceKey;
            IsDesignTime = connectionOptions.IsDesignTime;
        }

        /// <summary>
        /// The name of this ConnectionOptions instance.
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                    throw new InvalidOperationException(StringResources.ConnectionOptionsNoNameYet);
                return _name;
            }
            private set { _name = value; }
        }

        /// <summary>
        /// Specifies whether the EntityManager will attempt to connect to the EntityServer as soon as it is created.
        /// </summary>
        public bool ShouldConnect { get; private set; }

        /// <summary>
        /// Specifies what run-time data source key(s) will be used.
        /// </summary>
        public string DataSourceExtension { get; private set; }

        /// <summary>
        /// Specifies whether you will be using local or distributed data sources.
        /// </summary>
        public EntityServiceOption EntityServiceOption { get; private set; }

        /// <summary>
        /// Names the key providing the address of the application server with which the EntityManager will communicate.
        /// </summary>
        public string ServiceKey { get; private set; }

        /// <summary>
        /// Returns whether fake filtering is performed.
        /// </summary>
        public bool IsFake
        {
            get { return CompositionContext.IsFake; }
        }

        /// <summary>
        /// Specifies whether this ConnectionOptions are used for design time.
        /// </summary>
        public bool IsDesignTime { get; private set; }

        /// <summary>
        /// Returns the <see cref="IdeaBlade.Core.Composition.CompositionContext"/> used for this connection.
        /// </summary>
        public CompositionContext CompositionContext
        {
            get { return CompositionContext.GetByName(_compositionContextName); }
        }

        /// <summary>
        /// Converts the ConnectionOptions to a <see cref="LoginOptions"/> instance.
        /// </summary>
        public LoginOptions ToLoginOptions()
        {
            return new LoginOptions(DataSourceExtension, _compositionContextName, EntityServiceOption,
                                    ServiceKey);
        }

        /// <summary>
        /// Converts the ConnectionOptions to a <see cref="EntityManagerContext"/> instance.
        /// </summary>
        /// <param name="entityManagerOptions">Additional options controlling the behavior of the EntityManager.</param>
        public EntityManagerContext ToEntityManagerContext(EntityManagerOptions entityManagerOptions = null)
        {
            return new EntityManagerContext(ShouldConnect, DataSourceExtension, EntityServiceOption,
                                            _compositionContextName, entityManagerOptions, ServiceKey);
        }

        /// <summary>
        /// Creates a new ConnectionOptions from the current ConnectionOptions and assigns the specified name.
        /// </summary>
        /// <param name="name">The name to be assigned.</param>
        /// <returns>A new ConnectionOptions instance.</returns>
        /// <remarks>WithName must be used last. All the With... operations clear the name property.</remarks>
        public ConnectionOptions WithName(string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            return new ConnectionOptions(this) { Name = name };
        }

        /// <summary>
        /// Creates a new ConnectionOptions from the current ConnectionOptions and assigns the specified <see cref="CompositionContext"/> name.
        /// </summary>
        /// <param name="compositionContextName">The CompositionContext name to be assigned.</param>
        /// <returns>A new ConnectionOptions instance.</returns>
        public ConnectionOptions WithCompositionContext(string compositionContextName)
        {
            if (compositionContextName == null) throw new ArgumentNullException("compositionContextName");
            return new ConnectionOptions(this) { _compositionContextName = compositionContextName };
        }

        /// <summary>
        /// Creates a new ConnectionOptions from the current ConnectionOptions and assigns the specified data source extension.
        /// </summary>
        /// <param name="dataSourceExtension">The data source extension name to be assigned.</param>
        /// <returns>A new ConnectionOptions instance.</returns>
        public ConnectionOptions WithDataSourceExtension(string dataSourceExtension)
        {
            if (dataSourceExtension == null) throw new ArgumentNullException("dataSourceExtension");
            return new ConnectionOptions(this) { DataSourceExtension = dataSourceExtension };
        }

        /// <summary>
        /// Creates a new ConnectionOptions from the current ConnectionOptions and assigns the specified EntityServiceOption.
        /// </summary>
        /// <param name="entityServiceOption">The EntityServiceOption to be assigned.</param>
        /// <returns>A new ConnectionOptions instance.</returns>
        public ConnectionOptions WithEntityServiceOption(EntityServiceOption entityServiceOption)
        {
            return new ConnectionOptions(this) { EntityServiceOption = entityServiceOption };
        }

        /// <summary>
        /// Creates a new ConnectionOptions from the current ConnectionOptions and assigns the specified service key.
        /// </summary>
        /// <param name="serviceKey">The service key to be assigned.</param>
        /// <returns>A new ConnectionOptions instance.</returns>
        public ConnectionOptions WithServiceKey(string serviceKey)
        {
            if (serviceKey == null) throw new ArgumentNullException("serviceKey");
            return new ConnectionOptions(this) { ServiceKey = serviceKey };
        }

        /// <summary>
        /// Creates a new ConnectionOptions from the current ConnectionOptions and specifies whether an EntityManager should connect.
        /// </summary>
        /// <param name="shouldConnect">True if an EntityManager should connect prior to the first server operation.</param>
        /// <returns>A new ConnectionOptions instance.</returns>
        public ConnectionOptions WithShouldConnect(bool shouldConnect)
        {
            return new ConnectionOptions(this) { ShouldConnect = shouldConnect };
        }

        /// <summary>
        /// Retrieves the ConnectionOptions with the specified name.
        /// </summary>
        /// <param name="name">The name of the ConnectionOptions to be retrieved.</param>
        /// <exception cref="InvalidOperationException">Thrown if the ConnectionOptions with the specified name are not found.</exception>
        public static ConnectionOptions GetByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return GetByName(Default.Name);

            IEnumerable<IConnectionOptionsResolver> resolvers = Composition.GetInstances<IConnectionOptionsResolver>()
                .Concat(DefaultConnectionOptionsResolver.Instance);
            ConnectionOptions connectionOptions =
                resolvers.Select(r => r.GetConnectionOptions(name)).FirstOrDefault(c => c != null);

            if (connectionOptions == null)
                throw new InvalidOperationException(string.Format(StringResources.ConnectionOptionsNotFound, name));

            if (connectionOptions.Name != name)
                throw new InvalidOperationException(
                    string.Format(StringResources.ConnectionOptionsNameMissmatch, connectionOptions.Name, name));

            return connectionOptions;
        }
    }

    [PartNotDiscoverable]
    internal class DefaultConnectionOptionsResolver : IConnectionOptionsResolver
    {
        private static DefaultConnectionOptionsResolver _instance;

        protected DefaultConnectionOptionsResolver()
        {
        }

        public static DefaultConnectionOptionsResolver Instance
        {
            get { return _instance ?? (_instance = new DefaultConnectionOptionsResolver()); }
        }

        #region IConnectionOptionsResolver Members

        public ConnectionOptions GetConnectionOptions(string name)
        {
            if (name == ConnectionOptions.Default.Name)
                return ConnectionOptions.Default;

            if (name == ConnectionOptions.Fake.Name)
                return ConnectionOptions.Fake;

            if (name == ConnectionOptions.DesignTime.Name)
                return ConnectionOptions.DesignTime;

            return null;
        }

        #endregion
    }
}
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

using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Linq;

namespace Cocktail
{
    /// <summary>
    /// An implementation of <see cref="ICompositionProvider"/> which uses MEF as the underlying IoC implementation.
    /// </summary>
    internal partial class MefCompositionProvider : ICompositionProvider
    {
        private CompositionHost _container;
        private ContainerConfiguration _configuration;
        private ContainerConfiguration _defaultConfiguration;

        public ContainerConfiguration Configuration
        {
            get { return _configuration ?? DefaultConfiguration; }
        }

        public ContainerConfiguration DefaultConfiguration
        {
            get
            {
                if (_defaultConfiguration != null)
                    return _defaultConfiguration;

                return _defaultConfiguration = new ContainerConfiguration()
                    .WithAssemblies(IdeaBlade.Core.Composition.CompositionHost.Instance.ProbeAssemblies);
            }
        }

        public CompositionHost Container
        {
            get { return _container ?? (_container = Configuration.CreateContainer()); }
        }

        /// <summary>
        /// Returns true if the provided type has been previously registered.
        /// </summary>
        public bool IsTypeRegistered<T>()
        {
            return Container.GetExports<T>().Any();
        }

        public Lazy<T> GetInstance<T>()
        {
            return new Lazy<T>(() => Container.GetExport<T>());
        }

        public IEnumerable<Lazy<T>> GetInstances<T>()
        {
            return Container.GetExports<T>().Select(x => new Lazy<T>(() => x));
        }

        public Lazy<object> GetInstance(Type serviceType, string contractName)
        {
             return new Lazy<object>(() => Container.GetExport(serviceType, contractName));
        }

        public IEnumerable<Lazy<object>> GetInstances(Type serviceType, string contractName)
        {
            return Container.GetExports(serviceType, contractName).Select(x => new Lazy<object>(() => x));
        }

        public ICompositionFactory<T> GetInstanceFactory<T>()
        {
            var factory = new MefCompositionFactory<T>();
            Container.SatisfyImports(factory);
            return factory;
        }

        public void BuildUp(object instance)
        {
            // Skip if in design mode.
            if (Execute.InDesignMode)
                return;

            Container.SatisfyImports(instance);
        }
    }
}

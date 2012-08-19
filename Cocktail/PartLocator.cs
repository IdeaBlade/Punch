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

using IdeaBlade.Core;
using IdeaBlade.Core.Composition;
using System;
using System.Linq;
using CompositionContext = IdeaBlade.Core.Composition.CompositionContext;

#if !WinRT
using System.ComponentModel.Composition;
#else
using System.Composition;
#endif

namespace Cocktail
{
    internal class PartLocator<T> where T : class
    {
        private T _instance;
        private readonly Func<CompositionContext> _compositionContextDelegate;
        private readonly bool _useFactory;

        private bool _probed;
        private Func<T> _defaultGenerator;

        internal PartLocator(bool useFactory = false, Func<CompositionContext> compositionContextDelegate = null)
        {
            _compositionContextDelegate = compositionContextDelegate ?? (() => CompositionContext.Default);
            _useFactory = useFactory;
            _defaultGenerator = () => null;
            Composition.ProviderChanged +=
                new EventHandler<EventArgs>(OnCompositionProviderChanged).MakeWeak(eh => Composition.ProviderChanged -= eh);
        }

        internal PartLocator(PartLocator<T> partLocator)
        {
            _compositionContextDelegate = partLocator._compositionContextDelegate;
            _useFactory = partLocator._useFactory;
            _defaultGenerator = partLocator._defaultGenerator;
            Composition.ProviderChanged +=
                new EventHandler<EventArgs>(OnCompositionProviderChanged).MakeWeak(eh => Composition.ProviderChanged -= eh);
        }

        internal void OnCompositionProviderChanged(object sender, EventArgs e)
        {
            _instance = null;
            _probed = false;
        }

        internal bool Probed { get { return _probed || _instance != null; } }

        internal bool IsAvailable { get { return GetPart() != null; } }

        internal T GetPart()
        {
            if (Probed) return _instance;

            // Look for the part in the CompositionContext first;
            _instance = CompositionContext.GetExportedInstance(typeof(T)) as T;
            if (_instance != null)
            {
                WriteTrace();
                return _instance;
            }

            // Look for the part in the IoC container.
            if (!Composition.IsTypeRegistered<T>())
                _instance = DefaultGenerator();
            else if (_useFactory)
                _instance = GetPartFromFactory();
            else
                _instance = GetPartFromContainer();

            _probed = true;
            WriteTrace();

            return _instance;
        }

        internal PartLocator<T> WithDefaultGenerator(Func<T> generator)
        {
            if (generator == null)
                throw new ArgumentNullException();

            var partLocator = new PartLocator<T>(this) {_defaultGenerator = generator};
            return partLocator;
        }

        protected Func<T> DefaultGenerator { get { return _defaultGenerator; } }

        private CompositionContext CompositionContext { get { return _compositionContextDelegate() ?? CompositionContext.Default; } }

        private void WriteTrace()
        {
            if (_instance != null)
                TraceFns.WriteLine(String.Format(StringResources.ProbedForServiceAndFoundMatch, typeof(T).Name,
                                                 _instance.GetType().FullName));
            else
                TraceFns.WriteLine(String.Format(StringResources.ProbedForServiceFoundNoMatch, typeof(T).Name));
        }

        private T GetPartFromFactory()
        {
            var factory = Composition.GetInstanceFactory<T>();
            return factory.NewInstance();
        }

        private T GetPartFromContainer()
        {
            var parts = Composition.GetLazyInstances<T>().ToList();
            if (parts.Count() > 1)
                throw new InvalidOperationException(
                    String.Format(StringResources.ProbedForServiceAndFoundMultipleMatches, typeof(T).Name));

            return parts.First().Value;
        }
    }
}

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

using System;
using System.ComponentModel.Composition;
using System.Linq;
using IdeaBlade.Core;
using IdeaBlade.Core.Composition;

namespace Cocktail
{
    internal class PartLocator<T> where T : class
    {
        private T _instance;
        private readonly Func<CompositionContext> _compositionContextDelegate;
        private readonly CreationPolicy _creationPolicy;

        private bool _probed;
        private Func<T> _defaultGenerator;
        private Action<T> _initializer;

        internal PartLocator(CreationPolicy creationPolicy, Func<CompositionContext> compositionContextDelegate = null)
        {
            _compositionContextDelegate = compositionContextDelegate ?? (() => CompositionContext.Default);
            _creationPolicy = creationPolicy;
            _defaultGenerator = () => null;
            _initializer = instance => { };
        }

        internal bool Probed { get { return _probed || _instance != null; } }

        internal bool IsAvailable { get { return GetPart() != null; } }

        private CompositionContext CompositionContext { get { return _compositionContextDelegate() ?? CompositionContext.Default; } }

        private void WriteTrace()
        {
            if (_instance != null)
                TraceFns.WriteLine(String.Format(StringResources.ProbedForServiceAndFoundMatch, typeof(T).Name,
                                                 _instance.GetType().FullName));
            else
                TraceFns.WriteLine(String.Format(StringResources.ProbedForServiceFoundNoMatch, typeof(T).Name));
        }

        internal T GetPart()
        {
            if (Probed) return _instance;

            // Look for the part in the CompositionContext first;
            _instance = CompositionContext.GetExportedInstance(typeof(T)) as T;
            if (_instance != null)
            {
                WriteTrace();
                _initializer(_instance);
                return _instance;
            }

            // Do not probe if the CompositionHost isn't configured.
            if (!Composition.IsConfigured)
            {
                var defaultInstance = DefaultGenerator();
                if (defaultInstance != null) _initializer(defaultInstance);
                return defaultInstance;
            }

            // Look for the part in the MEF container
            var exports =
                Composition.GetExportsCore(typeof (T), null, _creationPolicy).ToList();
            if (exports.Count() > 1)
                throw new CompositionException(
                    String.Format(StringResources.ProbedForServiceAndFoundMultipleMatches, typeof (T).Name));

            _instance = exports.Any() ? exports.First().Value as T : DefaultGenerator();
            _probed = true;
            WriteTrace();

            if (_instance != null) Initializer(_instance);
            return _instance;
        }

        internal PartLocator<T> WithDefaultGenerator(Func<T> generator)
        {
            if (generator == null)
                throw new ArgumentNullException();

            var clone = (PartLocator<T>)MemberwiseClone();
            clone._defaultGenerator = generator;
            return clone;
        }

        internal PartLocator<T> WithInitializer(Action<T> initializer)
        {
            if (initializer == null)
                throw new ArgumentNullException();

            var clone = (PartLocator<T>)MemberwiseClone();
            clone._initializer = initializer;
            return clone;
        }

        protected Func<T> DefaultGenerator { get { return _defaultGenerator; } }
        protected Action<T> Initializer { get { return _initializer; } }
    }
}

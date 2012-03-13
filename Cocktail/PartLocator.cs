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

        internal PartLocator(CreationPolicy creationPolicy, Func<CompositionContext> compositionContextDelegate = null)
        {
            _compositionContextDelegate = compositionContextDelegate ?? (() => CompositionContext.Default);
            _creationPolicy = creationPolicy;
            _defaultGenerator = () => null;
            Composition.Cleared +=
                new EventHandler<EventArgs>(OnCompositionCleared).MakeWeak(eh => Composition.Cleared -= eh);
        }

        internal PartLocator(PartLocator<T> partLocator)
        {
            _compositionContextDelegate = partLocator._compositionContextDelegate;
            _creationPolicy = partLocator._creationPolicy;
            _defaultGenerator = partLocator._defaultGenerator;
            Composition.Cleared +=
                new EventHandler<EventArgs>(OnCompositionCleared).MakeWeak(eh => Composition.Cleared -= eh);
        }

        internal void OnCompositionCleared(object sender, EventArgs e)
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

            // Look for the part in the MEF container
            var exports =
                Composition.GetExportsCore(typeof (T), null, _creationPolicy).ToList();
            if (exports.Count() > 1)
                throw new CompositionException(
                    String.Format(StringResources.ProbedForServiceAndFoundMultipleMatches, typeof (T).Name));

            _instance = exports.Any() ? exports.First().Value as T : DefaultGenerator();
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
    }
}

//====================================================================================================================
//Copyright (c) 2011 IdeaBlade
//====================================================================================================================
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//====================================================================================================================
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//the Software.
//====================================================================================================================
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================

using System;
using System.ComponentModel.Composition;
using IdeaBlade.Core;
using System.Linq;

namespace IdeaBlade.Application.Framework.Core.Composition
{
    internal class PartLocator<T> where T : class
    {
        private T _instance;
        private readonly CompositionContext _compositionContext;
        private readonly CreationPolicy _creationPolicy;

        private readonly bool _optional;
        private bool _probed;
        private Func<T> _defaultGenerator;
        private Action<T> _initializer;

        internal PartLocator(CreationPolicy creationPolicy, bool optional = false, CompositionContext compositionContext = null)
        {
            _compositionContext = compositionContext ?? CompositionContext.Default;
            _creationPolicy = creationPolicy;
            _optional = optional;
            _defaultGenerator = () => null;
            _initializer = instance => { };
        }

        internal bool Probed { get { return _probed; } }

        internal bool IsAvailable { get { return GetPart() != null; } }

        internal bool Optional { get { return _optional; } }

        internal T GetPart()
        {
            if (Probed) return _instance;

            // Do not probe if the CompositionHost isn't configured.
            if (!CompositionHelper.IsConfigured) return DefaultGenerator();

            if (Optional)
            {
                var instances = CompositionHelper.GetExportsCore(
                    _compositionContext.ChildContainer, typeof (T), null, _creationPolicy).ToList();
                if (instances.Count() > 1)
                    throw new CompositionException(String.Format(
                        StringResources.ProbedForServiceAndFoundMultipleMatches, typeof(T).Name));
                _instance = instances.Any() ? (T) instances.First().Value : DefaultGenerator();
            }
            else
                _instance = _compositionContext.GetInstance<T>(_creationPolicy);

            _probed = true;

            if (_instance != null)
                TraceFns.WriteLine(String.Format(StringResources.ProbedForServiceAndFoundMatch, typeof(T).Name,
                                                 _instance.GetType().FullName));
            else
                TraceFns.WriteLine(String.Format(StringResources.ProbedForServiceFoundNoMatch, typeof(T).Name));

            if (_instance != null) Initializer(_instance);
            return _instance;
        }

        internal PartLocator<T> With(T instance)
        {
            if (instance != null) _initializer(instance);

            var clone = (PartLocator<T>)MemberwiseClone();
            clone._instance = instance;
            clone._probed = instance != null;
            return clone;
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

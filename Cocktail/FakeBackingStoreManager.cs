//====================================================================================================================
//Copyright (c) 2012 IdeaBlade
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
using System.Collections.Generic;
using System.Linq;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    internal class FakeBackingStoreManager
    {
        private readonly List<Type> _typesUsingFakeStore = new List<Type>();
        private static FakeBackingStoreManager _instance;

        protected FakeBackingStoreManager()
        {
        }

        public static FakeBackingStoreManager Instance
        {
            get { return _instance ?? (_instance = new FakeBackingStoreManager()); }
        }

        public bool IsInitialized
        {
            get { return GetFakeStoreEntityManagerProviders().All(emp => emp.IsInitialized); }
        }

        public void Register<T>() where T : EntityManager
        {
            _typesUsingFakeStore.Add(typeof(IEntityManagerProvider<T>));
        }

#if !SILVERLIGHT
        public void InitializeAll()
        {
            GetFakeStoreEntityManagerProviders().ForEach(emp => emp.Initialize());
        }
#endif

        public INotifyCompleted InitializeAllAsync()
        {
            return Coroutine.StartParallel(InitializeAllAsyncCore);
        }

        private IEnumerable<INotifyCompleted> InitializeAllAsyncCore()
        {
            return GetFakeStoreEntityManagerProviders().Select(emp => emp.InitializeAsync());
        }

        private IEnumerable<IEntityManagerProvider> GetFakeStoreEntityManagerProviders()
        {
            return _typesUsingFakeStore.Select(t => (IEntityManagerProvider)CompositionHelper.GetInstance(t, null));
        }
    }
}
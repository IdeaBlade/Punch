using System;
using System.Collections.Generic;
using System.Linq;
using IdeaBlade.Application.Framework.Core.Composition;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace IdeaBlade.Application.Framework.Core.Persistence
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
            _typesUsingFakeStore.Add(typeof (IEntityManagerProvider<T>));
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
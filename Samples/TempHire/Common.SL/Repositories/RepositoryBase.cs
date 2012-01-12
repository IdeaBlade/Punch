using System;
using System.Collections.Generic;
using Cocktail;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;
using Action = System.Action;

namespace Common.Repositories
{
    public abstract class RepositoryBase<T> : IRepository
        where T : EntityManager
    {
        private readonly IEntityManagerProvider<T> _entityManagerProvider;
        private readonly RepositoryBase<T> _baseData;

        protected RepositoryBase(IEntityManagerProvider<T> entityManagerProvider, RepositoryBase<T> baseData = null)
        {
            _entityManagerProvider = entityManagerProvider;
            _baseData = baseData;
            _entityManagerProvider.ManagerCreated +=
                new EventHandler<EventArgs>(OnManagerCreated).MakeWeak(eh => _entityManagerProvider.ManagerCreated -= eh);
        }

        protected T Manager
        {
            get { return _entityManagerProvider.Manager; }
        }

        #region IRepository Members

        public QueryStrategy BaseDataQueryStrategy
        {
            get { return _baseData != null ? QueryStrategy.CacheOnly : Manager.DefaultQueryStrategy; }
        }

        public bool HasChanges()
        {
            return Manager.HasChanges();
        }

        public AsyncOperation SaveAsync(Action onSuccess = null, Action<Exception> onFail = null)
        {
            return Manager.SaveChangesAsync(op => op.OnComplete(onSuccess, onFail)).AsOperationResult();
        }

        public void RejectChanges()
        {
            Manager.RejectChanges();
        }

        #endregion

        protected internal virtual void OnManagerCreated(object sender, EventArgs e)
        {
            if (_baseData == null) return;

            var restoreStrategy = new RestoreStrategy(false, false, MergeStrategy.OverwriteChanges);
            EntityCacheState baseData = _baseData.Manager.CacheStateManager.GetCacheState();
            Manager.CacheStateManager.RestoreCacheState(baseData, restoreStrategy);
        }

        protected AsyncOperation ExecuteQuery<TQuery>(IEntityQuery<TQuery> query,
                                                        Action<IEnumerable<TQuery>> onSuccess,
                                                        Action<Exception> onFail)
        {
            return query.ExecuteAsync(op => op.OnComplete(onSuccess, onFail)).AsOperationResult();
        }
    }
}
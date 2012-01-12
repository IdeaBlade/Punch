using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Cocktail;
using Common.Repositories;
using DomainModel;
using IdeaBlade.EntityModel;
using Action = System.Action;
using Coroutine = IdeaBlade.EntityModel.Coroutine;

namespace TempHire.Repositories
{
    [Export(typeof (ILookupRepository)), PartCreationPolicy(CreationPolicy.Shared)]
    public class LookupRepository : RepositoryBase<TempHireEntities>, ILookupRepository
    {
        [ImportingConstructor]
        public LookupRepository(
            [Import(RequiredCreationPolicy = CreationPolicy.NonShared)] IEntityManagerProvider<TempHireEntities>
                entityManagerProvider)
            : base(entityManagerProvider)
        {
        }

        #region ILookupRepository Members

        public IResult InitializeAsync(Action onSuccess = null, Action<Exception> onFail = null)
        {
            return Coroutine.StartParallel(InitializeCore, op => op.OnComplete(onSuccess, onFail)).AsResult();
        }

        #endregion

        private IEnumerable<INotifyCompleted> InitializeCore()
        {
            // Cache all lookup tables
            yield return Manager.PhoneNumberTypes.ExecuteAsync();
            yield return Manager.AddressTypes.ExecuteAsync();
            yield return Manager.RateTypes.ExecuteAsync();
            yield return Manager.States.ExecuteAsync();
        }
    }
}
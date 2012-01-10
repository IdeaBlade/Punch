using System.ComponentModel.Composition;
using IdeaBlade.Application.Framework.Core.Persistence;
using SampleModel;

namespace Core.Tests.SL.Helpers
{
    [PartNotDiscoverable]
    public class TestEntityManagerInterceptor : EntityManagerInterceptor<NorthwindIBEntities>
    {
        public int QueryingRaised;
        public int QueriedRaised;
        public int FetchingRaised;
        public int SavingRaised;
        public int SavedRaised;
        public int EntityChangingRaised;
        public int EntityChangedRaised;
        public int ClearedRaised;

        private int _counter;

        public override void OnQuerying(NorthwindIBEntities source, IdeaBlade.EntityModel.EntityQueryingEventArgs args)
        {
            QueryingRaised = ++_counter;
        }

        public override void OnQueried(NorthwindIBEntities source, IdeaBlade.EntityModel.EntityQueriedEventArgs args)
        {
            QueriedRaised = ++_counter;
        }

        public override void OnFetching(NorthwindIBEntities source, IdeaBlade.EntityModel.EntityFetchingEventArgs args)
        {
            FetchingRaised = ++_counter;
        }

        public override void OnSaving(NorthwindIBEntities source, IdeaBlade.EntityModel.EntitySavingEventArgs args)
        {
            SavingRaised = ++_counter;
        }

        public override void OnSaved(NorthwindIBEntities source, IdeaBlade.EntityModel.EntitySavedEventArgs args)
        {
            SavedRaised = ++_counter;
        }

        public override void OnEntityChanging(NorthwindIBEntities source, IdeaBlade.EntityModel.EntityChangingEventArgs args)
        {
            EntityChangingRaised = ++_counter;
        }

        public override void OnEntityChanged(NorthwindIBEntities source, IdeaBlade.EntityModel.EntityChangedEventArgs args)
        {
            EntityChangedRaised = ++_counter;
        }

        public override void OnCleared(NorthwindIBEntities source, IdeaBlade.EntityModel.EntityManagerClearedEventArgs args)
        {
            ClearedRaised = ++_counter;
        }
    }
}
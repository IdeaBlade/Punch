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

using System.Collections.Generic;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    internal class FakeBackingStore
    {
        private static readonly Dictionary<string, FakeBackingStore> FakeBackingStores =
            new Dictionary<string, FakeBackingStore>();

        private readonly string _compositionContextName;
        private IEntityServerFakeBackingStore _store;
        private CoroutineOperation _resetOp;

        public FakeBackingStore(string compositionContextName)
        {
            _compositionContextName = compositionContextName;
        }

        private IEntityServerFakeBackingStore Store
        {
            get
            {
                return _store ??
                       (_store = IdeaBlade.Core.Composition.CompositionContext.GetByName(_compositionContextName).GetFakeBackingStore());
            }
        }

        public bool IsInitialized { get; private set; }

        public CoroutineOperation InitializeOperation { get; private set; }

        public OperationResult ResetAsync(EntityManager manager, EntityCacheState storeEcs)
        {
            _resetOp = Coroutine.Start(() => ResetCore(manager, storeEcs));
            if (InitializeOperation == null)
                InitializeOperation = _resetOp;

            return _resetOp.OnComplete(
                () =>
                {
                    IsInitialized = true;
                    _resetOp = null;
                }, null).AsOperationResult();
        }

        public static bool Exists(string compositionContextName)
        {
            return FakeBackingStores.ContainsKey(compositionContextName);
        }

        public static FakeBackingStore Get(string compositionContextName)
        {
            return FakeBackingStores[compositionContextName];
        }

        public static FakeBackingStore Create(string compositionContextName)
        {
            var fakeBackingStore = new FakeBackingStore(compositionContextName);
            FakeBackingStores.Add(compositionContextName, fakeBackingStore);

            return fakeBackingStore;
        }

        private IEnumerable<INotifyCompleted> ResetCore(EntityManager manager, EntityCacheState storeEcs)
        {
            // Make sure we are connected.
            if (!manager.IsConnected)
                yield return manager.ConnectAsync();

            yield return Store.RestoreAsync(storeEcs);
        }

#if !SILVERLIGHT

        public void Reset(EntityManager manager, EntityCacheState storeEcs)
        {
            if (!manager.IsConnected)
                manager.Connect();

            Store.Restore(storeEcs);

            IsInitialized = true;
        }

#endif
    }
}
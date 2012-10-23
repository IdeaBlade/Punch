// ====================================================================================================================
//   Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//   OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//   OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//   USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//   http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using System;
using Cocktail;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace DomainServices.Repositories
{
    public class GlobalCacheRepository<T> : Repository<T> where T : class
    {
        private readonly IGlobalCache _globalCache;

        public GlobalCacheRepository(IEntityManagerProvider entityManagerProvider, IGlobalCache globalCache)
            : base(entityManagerProvider)
        {
            _globalCache = globalCache;
            entityManagerProvider.ManagerCreated += new EventHandler<EntityManagerCreatedEventArgs>(OnManagerCreated)
                .MakeWeak(eh => entityManagerProvider.ManagerCreated -= eh);

            if (globalCache != null)
                DefaultQueryStrategy = QueryStrategy.CacheOnly;
        }

        internal void OnManagerCreated(object sender, EntityManagerCreatedEventArgs e)
        {
            if (_globalCache == null) return;

            Seed(e.EntityManager);
            e.EntityManager.Cleared += new EventHandler<EntityManagerClearedEventArgs>(OnCleared)
                .MakeWeak(eh => e.EntityManager.Cleared -= eh);
        }

        internal void OnCleared(object sender, EntityManagerClearedEventArgs e)
        {
            Seed(e.EntityManager);
        }

        private void Seed(EntityManager entityManager)
        {
            var entities = _globalCache.Get<T>();
            entityManager.ImportEntities(entities, MergeStrategy.OverwriteChanges);
        }
    }
}
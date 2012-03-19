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
using System.Collections.Generic;
using Cocktail;
using Cocktail.Contrib.UnitOfWork;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace DomainServices.Repositories
{
    public class PreLoadRepository<T> : Repository<T> where T : class
    {
        private readonly IPreLoader _preLoader;

        public PreLoadRepository(IEntityManagerProvider entityManagerProvider, IPreLoader preLoader)
            : base(entityManagerProvider)
        {
            _preLoader = preLoader;
            entityManagerProvider.ManagerCreated += new EventHandler<EntityManagerCreatedEventArgs>(OnManagerCreated)
                .MakeWeak(eh => entityManagerProvider.ManagerCreated -= eh);
        }

        protected bool IsPreLoaded
        {
            get { return _preLoader != null; }
        }

        internal void OnManagerCreated(object sender, EntityManagerCreatedEventArgs e)
        {
            IEnumerable<T> entites = _preLoader.EntityManager.FindEntities<T>(EntityState.Unchanged);
            e.EntityManager.ImportEntities(entites, MergeStrategy.OverwriteChanges);
        }

        protected override IEntityQuery<T> GetFindQuery(IPredicateDescription predicate, ISortSelector sortSelector,
                                                        string includeProperties)
        {
            IEntityQuery<T> query = base.GetFindQuery(predicate, sortSelector, includeProperties);
            return IsPreLoaded ? query.With(QueryStrategy.CacheOnly) : query;
        }

        protected override IEntityQuery GetKeyQuery(params object[] keyValues)
        {
            IEntityQuery query = base.GetKeyQuery(keyValues);
            return IsPreLoaded ? query.With(QueryStrategy.CacheOnly) : query;
        }
    }
}
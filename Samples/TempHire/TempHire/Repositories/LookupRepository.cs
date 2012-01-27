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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Cocktail;
using Common.Repositories;
using DomainModel;
using IdeaBlade.EntityModel;
using Action = System.Action;
using Coroutine = IdeaBlade.EntityModel.Coroutine;

namespace TempHire.Repositories
{
    [Export(typeof(ILookupRepository)), PartCreationPolicy(CreationPolicy.Shared)]
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

        public OperationResult InitializeAsync(Action onSuccess = null, Action<Exception> onFail = null)
        {
            return Coroutine.StartParallel(InitializeCore, op => op.OnComplete(onSuccess, onFail)).AsOperationResult();
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
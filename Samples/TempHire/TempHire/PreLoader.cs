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
using System.ComponentModel.Composition;
using Cocktail;
using DomainModel;
using DomainServices.Repositories;
using IdeaBlade.EntityModel;

namespace TempHire
{
    [Export(typeof(IPreLoader)), PartCreationPolicy(CreationPolicy.Shared)]
    public class PreLoader : IPreLoader
    {
        private readonly IEntityManagerProvider<TempHireEntities> _entityManagerProvider;

        [ImportingConstructor]
        public PreLoader(
            [Import(RequiredCreationPolicy = CreationPolicy.NonShared)] IEntityManagerProvider<TempHireEntities>
                entityManagerProvider)
        {
            _entityManagerProvider = entityManagerProvider;
        }

        #region IPreLoader Members

        public TempHireEntities EntityManager
        {
            get { return _entityManagerProvider.Manager; }
        }

        public OperationResult LoadAsync(Action onSuccess = null, Action<Exception> onFail = null)
        {
            return Coroutine.StartParallel(InitializeCore, op => op.OnComplete(onSuccess, onFail)).AsOperationResult();
        }

        #endregion

        private IEnumerable<INotifyCompleted> InitializeCore()
        {
            // Cache all lookup tables
            yield return EntityManager.PhoneNumberTypes.ExecuteAsync();
            yield return EntityManager.AddressTypes.ExecuteAsync();
            yield return EntityManager.RateTypes.ExecuteAsync();
            yield return EntityManager.States.ExecuteAsync();
        }
    }
}
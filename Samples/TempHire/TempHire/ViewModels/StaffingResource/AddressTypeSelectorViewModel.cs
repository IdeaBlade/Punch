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

using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Common.Errors;
using Common.Factories;
using DomainModel;
using DomainServices;
using IdeaBlade.Linq;

namespace TempHire.ViewModels.StaffingResource
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class AddressTypeSelectorViewModel : Screen
    {
        private readonly IErrorHandler _errorHandler;
        private BindableCollection<AddressType> _addressTypes;
        private AddressType _selectedAddressType;

        [ImportingConstructor]
        public AddressTypeSelectorViewModel(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        public BindableCollection<AddressType> AddressTypes
        {
            get { return _addressTypes; }
            set
            {
                _addressTypes = value;
                NotifyOfPropertyChange(() => AddressTypes);
                SelectedAddressType = _addressTypes.FirstOrDefault();
            }
        }

        public AddressType SelectedAddressType
        {
            get { return _selectedAddressType; }
            set
            {
                _selectedAddressType = value;
                NotifyOfPropertyChange(() => SelectedAddressType);
            }
        }

        public AddressTypeSelectorViewModel Start(IUnitOfWorkCore unitOfWork)
        {
            var orderBySelector = new SortSelector("Name");
            unitOfWork.AddressTypes.FindAsync(
                null, orderBySelector, null, result => AddressTypes = new BindableCollection<AddressType>(result),
                _errorHandler.HandleError);
            return this;
        }
    }

    [Export(typeof(IPartFactory<AddressTypeSelectorViewModel>))]
    public class AddressTypeSelectorViewModelFactory : PartFactoryBase<AddressTypeSelectorViewModel>
    {
    }
}
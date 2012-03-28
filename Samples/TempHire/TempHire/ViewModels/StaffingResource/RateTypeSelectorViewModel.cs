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
    public class RateTypeSelectorViewModel : Screen
    {
        private readonly IErrorHandler _errorHandler;
        private BindableCollection<RateType> _rateTypes;
        private RateType _selectedRateType;

        [ImportingConstructor]
        public RateTypeSelectorViewModel(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        public BindableCollection<RateType> RateTypes
        {
            get { return _rateTypes; }
            set
            {
                _rateTypes = value;
                NotifyOfPropertyChange(() => RateTypes);
                SelectedRateType = _rateTypes.FirstOrDefault();
            }
        }

        public RateType SelectedRateType
        {
            get { return _selectedRateType; }
            set
            {
                _selectedRateType = value;
                NotifyOfPropertyChange(() => SelectedRateType);
            }
        }

        public RateTypeSelectorViewModel Start(IDomainUnitOfWork unitOfWork)
        {
            var orderBySelector = new SortSelector("Name");
            unitOfWork.RateTypes.FindAsync(
                null, orderBySelector, null, result => RateTypes = new BindableCollection<RateType>(result),
                _errorHandler.HandleError);
            return this;
        }
    }

    [Export(typeof (IPartFactory<RateTypeSelectorViewModel>))]
    public class RateTypeSelectorViewModelFactory : PartFactoryBase<RateTypeSelectorViewModel>
    {
    }
}
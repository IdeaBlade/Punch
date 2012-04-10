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
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Cocktail;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace TempHire.ViewModels.StaffingResource
{
    [Export(typeof(IStaffingResourceDetailSection)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffingResourceContactInfoViewModel : Conductor<IScreen>.Collection.AllActive, IDiscoverableViewModel,
                                                        IHarnessAware, IStaffingResourceDetailSection
    {
        [ImportingConstructor]
        public StaffingResourceContactInfoViewModel(StaffingResourceAddressListViewModel staffingResourceAddressList,
                                                    StaffingResourcePhoneListViewModel staffingResourcePhoneList)
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            DisplayName = "Contact Information";
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
            StaffingResourceAddressList = staffingResourceAddressList;
            StaffingResourcePhoneList = staffingResourcePhoneList;
        }

        public StaffingResourceAddressListViewModel StaffingResourceAddressList { get; private set; }
        public StaffingResourcePhoneListViewModel StaffingResourcePhoneList { get; private set; }

        #region IHarnessAware Members

        public void Setup()
        {
#if HARNESS
            Start(TempHireSampleDataProvider.CreateGuid(1));
#endif
        }

        #endregion

        #region IStaffingResourceDetailSection Members

        public int Index
        {
            get { return 0; }
        }

        void IStaffingResourceDetailSection.Start(Guid staffingResourceId)
        {
            Start(staffingResourceId);
        }

        #endregion

        public StaffingResourceContactInfoViewModel Start(Guid staffingResourceId)
        {
            ActivateItem(StaffingResourceAddressList.Start(staffingResourceId));
            ActivateItem(StaffingResourcePhoneList.Start(staffingResourceId));
            return this;
        }
    }
}
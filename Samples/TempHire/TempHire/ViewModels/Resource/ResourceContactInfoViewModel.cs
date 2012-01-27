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
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Cocktail;
using Common.SampleData;

namespace TempHire.ViewModels.Resource
{
    [Export(typeof (IResourceDetailSection)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResourceContactInfoViewModel : Conductor<IScreen>.Collection.AllActive, IDiscoverableViewModel,
                                                IHarnessAware, IResourceDetailSection
    {
        [ImportingConstructor]
        public ResourceContactInfoViewModel(ResourceAddressListViewModel resourceAddressList,
                                            ResourcePhoneListViewModel resourcePhoneList)
        {
// ReSharper disable DoNotCallOverridableMethodsInConstructor
            DisplayName = "Contact Information";
// ReSharper restore DoNotCallOverridableMethodsInConstructor
            ResourceAddressList = resourceAddressList;
            ResourcePhoneList = resourcePhoneList;
        }

        public ResourceAddressListViewModel ResourceAddressList { get; private set; }
        public ResourcePhoneListViewModel ResourcePhoneList { get; private set; }

        #region IHarnessAware Members

        public void Setup()
        {
#if HARNESS
            Start(TempHireSampleDataProvider.CreateGuid(1));
#endif
        }

        #endregion

        #region IResourceDetailSection Members

        public int Index
        {
            get { return 0; }
        }

        void IResourceDetailSection.Start(Guid resourceId)
        {
            Start(resourceId);
        }

        #endregion

        public ResourceContactInfoViewModel Start(Guid resourceId)
        {
            ActivateItem(ResourceAddressList.Start(resourceId));
            ActivateItem(ResourcePhoneList.Start(resourceId));
            return this;
        }
    }
}
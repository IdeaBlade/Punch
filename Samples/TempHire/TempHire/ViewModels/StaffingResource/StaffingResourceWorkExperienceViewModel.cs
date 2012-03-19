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
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Common.Errors;
using Common.Messages;
using DomainModel;
using DomainServices;

namespace TempHire.ViewModels.StaffingResource
{
    [Export(typeof (IStaffingResourceDetailSection)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffingResourceWorkExperienceViewModel : StaffingResourceScreenBase, IStaffingResourceDetailSection,
                                                           IHandle<SavedMessage>
    {
        [ImportingConstructor]
        public StaffingResourceWorkExperienceViewModel(
            IUnitOfWorkManager<IStaffingResourceUnitOfWork> unitOfWorkManager,
            IErrorHandler errorHandler)
            : base(unitOfWorkManager, errorHandler)
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            DisplayName = "Work Experience";
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        public bool IsEmpty
        {
            get { return StaffingResource == null || StaffingResource.WorkExperience.Count == 0; }
        }

        public override DomainModel.StaffingResource StaffingResource
        {
            get { return base.StaffingResource; }
            set
            {
                if (base.StaffingResource != null)
                    base.StaffingResource.WorkExperience.CollectionChanged -= WorkExperienceCollectionChanged;

                if (value != null)
                    value.WorkExperience.CollectionChanged += WorkExperienceCollectionChanged;

                base.StaffingResource = value;
                NotifyOfPropertyChange(() => IsEmpty);
                NotifyOfPropertyChange(() => WorkExperienceSorted);
            }
        }

        public IEnumerable<WorkExperienceItem> WorkExperienceSorted
        {
            get
            {
                if (StaffingResource != null)
                    return StaffingResource.WorkExperience.OrderBy(e => e.From);
                return new WorkExperienceItem[0];
            }
        }

        #region IHandle<SavedMessage> Members

        public void Handle(SavedMessage message)
        {
            if (message.Entities.Any(e => e == StaffingResource))
                NotifyOfPropertyChange(() => WorkExperienceSorted);
        }

        #endregion

        #region IStaffingResourceDetailSection Members

        public int Index
        {
            get { return 20; }
        }

        void IStaffingResourceDetailSection.Start(Guid staffingResourceId)
        {
            Start(staffingResourceId);
        }

        #endregion

        public void Add()
        {
            StaffingResource.AddWorkExperience();
        }

        public void Delete(WorkExperienceItem workExperience)
        {
            StaffingResource.DeleteWorkExperience(workExperience);
        }

        private void WorkExperienceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => IsEmpty);
            NotifyOfPropertyChange(() => WorkExperienceSorted);
        }
    }
}
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
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Common;
using Common.Messages;
using DomainModel;
using DomainServices;
using IdeaBlade.EntityModel;

namespace TempHire.ViewModels.StaffingResource
{
    [Export(typeof (IStaffingResourceDetailSection)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffingResourceWorkExperienceViewModel : StaffingResourceScreenBase, IStaffingResourceDetailSection,
                                                           IHandle<SavedMessage>
    {
        [ImportingConstructor]
        public StaffingResourceWorkExperienceViewModel(
            IResourceMgtUnitOfWorkManager<IResourceMgtUnitOfWork> unitOfWorkManager)
            : base(unitOfWorkManager)
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            DisplayName = "Work Experience";
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        public bool IsEmpty
        {
            get
            {
                return StaffingResource != null && !StaffingResource.WorkExperience.IsPendingEntityList &&
                       StaffingResource.WorkExperience.Count == 0;
            }
        }

        public bool IsPending
        {
            get { return StaffingResource == null || StaffingResource.WorkExperience.IsPendingEntityList; }
        }

        public override DomainModel.StaffingResource StaffingResource
        {
            get { return base.StaffingResource; }
            set
            {
                if (base.StaffingResource != null)
                {
                    base.StaffingResource.WorkExperience.CollectionChanged -= WorkExperienceCollectionChanged;
                    base.StaffingResource.WorkExperience.PendingEntityListResolved -=
                        WorkExperienceOnPendingEntityListResolved;
                }

                if (value != null)
                {
                    value.WorkExperience.CollectionChanged += WorkExperienceCollectionChanged;
                    value.WorkExperience.PendingEntityListResolved += WorkExperienceOnPendingEntityListResolved;
                }

                base.StaffingResource = value;
                NotifyOfPropertyChange(() => IsEmpty);
                NotifyOfPropertyChange(() => IsPending);
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

        void IStaffingResourceDetailSection.Start(Guid staffingResourceId, EditMode editMode)
        {
            Start(staffingResourceId, editMode);
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

        private void WorkExperienceOnPendingEntityListResolved(
            object sender, PendingEntityListResolvedEventArgs<WorkExperienceItem> pendingEntityListResolvedEventArgs)
        {
            NotifyOfPropertyChange(() => IsPending);
            NotifyOfPropertyChange(() => IsEmpty);
        }

        private void WorkExperienceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => IsEmpty);
            NotifyOfPropertyChange(() => IsPending);
            NotifyOfPropertyChange(() => WorkExperienceSorted);
        }
    }
}
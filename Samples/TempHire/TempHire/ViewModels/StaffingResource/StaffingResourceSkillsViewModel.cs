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
using Common;
using Common.Errors;
using Common.Messages;
using DomainModel;
using DomainServices;
using IdeaBlade.EntityModel;

namespace TempHire.ViewModels.StaffingResource
{
    [Export(typeof (IStaffingResourceDetailSection)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffingResourceSkillsViewModel : StaffingResourceScreenBase, IStaffingResourceDetailSection,
                                                   IHandle<SavedMessage>
    {
        [ImportingConstructor]
        public StaffingResourceSkillsViewModel(IResourceMgtUnitOfWorkManager<IResourceMgtUnitOfWork> unitOfWorkManager,
                                               IErrorHandler errorHandler)
            : base(unitOfWorkManager, errorHandler)
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            DisplayName = "Skills";
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        public IEnumerable<Skill> SkillsSorted
        {
            get
            {
                if (StaffingResource != null)
                    return StaffingResource.Skills.OrderBy(s => s.Description);
                return new Skill[0];
            }
        }

        public bool IsEmpty
        {
            get
            {
                return StaffingResource != null && !StaffingResource.Skills.IsPendingEntityList &&
                       StaffingResource.Skills.Count == 0;
            }
        }

        public bool IsPending
        {
            get { return StaffingResource == null || StaffingResource.Skills.IsPendingEntityList; }
        }

        public override DomainModel.StaffingResource StaffingResource
        {
            get { return base.StaffingResource; }
            set
            {
                if (base.StaffingResource != null)
                {
                    base.StaffingResource.Skills.CollectionChanged -= SkillsCollectionChanged;
                    base.StaffingResource.Skills.PendingEntityListResolved -= SkillsOnPendingEntityListResolved;
                }

                if (value != null)
                {
                    value.Skills.CollectionChanged += SkillsCollectionChanged;
                    value.Skills.PendingEntityListResolved += SkillsOnPendingEntityListResolved;
                }

                base.StaffingResource = value;
                NotifyOfPropertyChange(() => IsEmpty);
                NotifyOfPropertyChange(() => IsPending);
                NotifyOfPropertyChange(() => SkillsSorted);
            }
        }

        #region IHandle<SavedMessage> Members

        public void Handle(SavedMessage message)
        {
            if (message.Entities.Any(e => e == StaffingResource))
                NotifyOfPropertyChange(() => SkillsSorted);
        }

        #endregion

        #region IStaffingResourceDetailSection Members

        public int Index
        {
            get { return 30; }
        }

        void IStaffingResourceDetailSection.Start(Guid staffingResourceId, EditMode editMode)
        {
            Start(staffingResourceId, editMode);
        }

        #endregion

        public void Add()
        {
            StaffingResource.AddSkill();
        }

        public void Delete(Skill skill)
        {
            StaffingResource.DeleteSkill(skill);
        }

        private void SkillsOnPendingEntityListResolved(
            object sender, PendingEntityListResolvedEventArgs<Skill> pendingEntityListResolvedEventArgs)
        {
            NotifyOfPropertyChange(() => IsPending);
            NotifyOfPropertyChange(() => IsEmpty);
        }

        private void SkillsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => IsEmpty);
            NotifyOfPropertyChange(() => IsPending);
            NotifyOfPropertyChange(() => SkillsSorted);
        }
    }
}
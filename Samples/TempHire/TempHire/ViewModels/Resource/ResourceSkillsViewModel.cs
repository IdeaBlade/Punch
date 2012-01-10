using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Common.Errors;
using DomainModel;
using DomainModel.Messages;
using DomainModel.Repositories;
using TempHire.Repositories;

namespace TempHire.ViewModels.Resource
{
    [Export(typeof(IResourceDetailSection)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResourceSkillsViewModel : ResourceScreenBase, IResourceDetailSection, IHandle<SavedMessage>
    {
        [ImportingConstructor]
        public ResourceSkillsViewModel(IRepositoryManager<IResourceRepository> repositoryManager,
                                       IEventAggregator eventAggregator, IErrorHandler errorHandler)
            : base(repositoryManager, errorHandler)
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            DisplayName = "Skills";
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
            eventAggregator.Subscribe(this);
        }

        public IEnumerable<Skill> SkillsSorted
        {
            get
            {
                if (Resource != null)
                    return Resource.Skills.OrderBy(s => s.Description);
                return new Skill[0];
            }
        }

        public bool IsEmpty
        {
            get { return Resource == null || Resource.Skills.Count == 0; }
        }

        public override DomainModel.Resource Resource
        {
            get { return base.Resource; }
            set
            {
                if (base.Resource != null)
                    base.Resource.Skills.CollectionChanged -= SkillsCollectionChanged;

                if (value != null)
                    value.Skills.CollectionChanged += SkillsCollectionChanged;

                base.Resource = value;
                NotifyOfPropertyChange(() => IsEmpty);
                NotifyOfPropertyChange(() => SkillsSorted);
            }
        }

        #region IHandle<SavedMessage> Members

        public void Handle(SavedMessage message)
        {
            if (message.Entities.Any(e => e == Resource))
                NotifyOfPropertyChange(() => SkillsSorted);
        }

        #endregion

        #region IResourceDetailSection Members

        public int Index
        {
            get { return 30; }
        }

        void IResourceDetailSection.Start(Guid resourceId)
        {
            Start(resourceId);
        }

        #endregion

        private void SkillsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => IsEmpty);
            NotifyOfPropertyChange(() => SkillsSorted);
        }

        public void Add()
        {
            Resource.AddSkill();
        }

        public void Delete(Skill skill)
        {
            Resource.DeleteSkill(skill);
        }
    }
}
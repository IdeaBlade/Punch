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
using Cocktail;
using Common.Errors;
using Common.Messages;
using Common.Repositories;
using DomainModel;

namespace TempHire.ViewModels.Resource
{
    [Export(typeof(IResourceDetailSection)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResourceSkillsViewModel : ResourceScreenBase, IResourceDetailSection, IHandle<SavedMessage>
    {
        [ImportingConstructor]
        public ResourceSkillsViewModel(IRepositoryManager<IResourceRepository> repositoryManager,
                                       IErrorHandler errorHandler)
            : base(repositoryManager, errorHandler)
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            DisplayName = "Skills";
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
            EventFns.Subscribe(this);
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
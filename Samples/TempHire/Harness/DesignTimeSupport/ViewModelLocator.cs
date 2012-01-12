using System.Collections.Generic;
using Cocktail;
using Common.BusyWatcher;
using Common.Errors;
using Common.Repositories;
using Common.SampleData;
using Common.Toolbar;
using Common.Workspace;
using DomainModel;
using TempHire.Authentication;
using TempHire.ViewModels;
using TempHire.ViewModels.Login;
using TempHire.ViewModels.Resource;

namespace TempHire.DesignTimeSupport
{
    public class ViewModelLocator : DesignTimeViewModelLocatorBase<TempHireEntities>
    {
        public ResourceAddressListViewModel ResourceAddressListViewModel
        {
            get
            {
                return (ResourceAddressListViewModel)
                       new ResourceAddressListViewModel(new DesignTimeResourceRepositoryManager(EntityManagerProvider),
                                                        null, new ErrorHandler())
                           .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public ResourceSummaryViewModel ResourceSummaryViewModel
        {
            get
            {
                return (ResourceSummaryViewModel)
                       new ResourceSummaryViewModel(new DesignTimeResourceRepositoryManager(EntityManagerProvider), null,
                                                    new ErrorHandler())
                           .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public AddressTypeSelectorViewModel AddressTypeSelectorViewModel
        {
            get
            {
                return new AddressTypeSelectorViewModel(new DesignTimeResourceRepositoryManager(EntityManagerProvider),
                                                        new ErrorHandler())
                    .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public ResourceNameEditorViewModel ResourceNameEditorViewModel
        {
            get
            {
                return new ResourceNameEditorViewModel(new DesignTimeResourceRepositoryManager(EntityManagerProvider),
                                                       new ErrorHandler())
                    .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public ResourcePhoneListViewModel ResourcePhoneListViewModel
        {
            get
            {
                return (ResourcePhoneListViewModel)
                       new ResourcePhoneListViewModel(new DesignTimeResourceRepositoryManager(EntityManagerProvider),
                                                      null, new ErrorHandler())
                           .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public PhoneTypeSelectorViewModel PhoneTypeSelectorViewModel
        {
            get
            {
                return new PhoneTypeSelectorViewModel(new DesignTimeResourceRepositoryManager(EntityManagerProvider),
                                                      new ErrorHandler())
                    .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public ResourceDetailViewModel ResourceDetailViewModel
        {
            get
            {
                var rm = new DesignTimeResourceRepositoryManager(EntityManagerProvider);
                return new ResourceDetailViewModel(rm, new ResourceSummaryViewModel(rm, null, new ErrorHandler()),
                                                   new IResourceDetailSection[]
                                                       {
                                                           new ResourceContactInfoViewModel(
                                                               new ResourceAddressListViewModel(rm, null,
                                                                                                new ErrorHandler()),
                                                               new ResourcePhoneListViewModel(rm, null,
                                                                                              new ErrorHandler())),
                                                           new ResourceRatesViewModel(rm, null, new ErrorHandler())
                                                           ,
                                                           new ResourceWorkExperienceViewModel(rm, 
                                                                                               new ErrorHandler()),
                                                           new ResourceSkillsViewModel(rm, new ErrorHandler())
                                                       },
                                                   new ErrorHandler(), new BusyWatcher())
                    .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public ResourceRatesViewModel ResourceRatesViewModel
        {
            get
            {
                return (ResourceRatesViewModel)
                       new ResourceRatesViewModel(new DesignTimeResourceRepositoryManager(EntityManagerProvider), null,
                                                  new ErrorHandler()).Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public RateTypeSelectorViewModel RateTypeSelectorViewModel
        {
            get
            {
                return
                    new RateTypeSelectorViewModel(new DesignTimeResourceRepositoryManager(EntityManagerProvider),
                                                  new ErrorHandler()).Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public ResourceWorkExperienceViewModel ResourceWorkExperienceViewModel
        {
            get
            {
                return (ResourceWorkExperienceViewModel)
                       new ResourceWorkExperienceViewModel(
                           new DesignTimeResourceRepositoryManager(EntityManagerProvider), new ErrorHandler())
                           .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public ResourceSkillsViewModel ResourceSkillsViewModel
        {
            get
            {
                return (ResourceSkillsViewModel)
                       new ResourceSkillsViewModel(
                           new DesignTimeResourceRepositoryManager(EntityManagerProvider), new ErrorHandler())
                           .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public ResourceSearchViewModel ResourceSearchViewModel
        {
            get
            {
                return new ResourceSearchViewModel(new ResourceRepository(EntityManagerProvider),
                                                   new ErrorHandler(), new BusyWatcher()).Start();
            }
        }

        public ResourceManagementViewModel ResourceManagementViewModel
        {
            get
            {
                var rm = new DesignTimeResourceRepositoryManager(EntityManagerProvider);
                return
                    new ResourceManagementViewModel(
                        new ResourceSearchViewModel(new ResourceRepository(EntityManagerProvider),
                                                    new ErrorHandler(), new BusyWatcher()), null, null, rm,
                        new ErrorHandler(), null);
            }
        }

        public ShellViewModel ShellViewModel
        {
            get
            {
                return
                    new ShellViewModel(new List<IWorkspace>(), new ToolbarManager(), new FakeAuthenticationService(),
                                       null, new BusyWatcher()).Start();
            }
        }

        public LoginViewModel LoginViewModel
        {
            get
            {
                return new LoginViewModel(new FakeAuthenticationService(), null, null, new BusyWatcher(),
                                          new ErrorHandler()) { FailureMessage = "FailureMessage at design time" };
            }
        }

        protected override IEntityManagerProvider<TempHireEntities> CreateEntityManagerProvider()
        {
            return new DesignTimeEntityManagerProvider(new[] { new TempHireSampleDataProvider() });
        }
    }
}
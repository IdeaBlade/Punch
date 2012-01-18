using System;
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
                                                        null, DesignTimeErrorHandler.Instance,
                                                        DesignTimeDialogManager.Instance)
                           .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public ResourceSummaryViewModel ResourceSummaryViewModel
        {
            get
            {
                return (ResourceSummaryViewModel)
                       new ResourceSummaryViewModel(new DesignTimeResourceRepositoryManager(EntityManagerProvider), null,
                                                    DesignTimeErrorHandler.Instance, DesignTimeDialogManager.Instance)
                           .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public AddressTypeSelectorViewModel AddressTypeSelectorViewModel
        {
            get
            {
                return new AddressTypeSelectorViewModel(new DesignTimeResourceRepositoryManager(EntityManagerProvider),
                                                        DesignTimeErrorHandler.Instance)
                    .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public ResourceNameEditorViewModel ResourceNameEditorViewModel
        {
            get
            {
                return new ResourceNameEditorViewModel(new DesignTimeResourceRepositoryManager(EntityManagerProvider),
                                                       DesignTimeErrorHandler.Instance)
                    .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public ResourcePhoneListViewModel ResourcePhoneListViewModel
        {
            get
            {
                return (ResourcePhoneListViewModel)
                       new ResourcePhoneListViewModel(new DesignTimeResourceRepositoryManager(EntityManagerProvider),
                                                      null, DesignTimeErrorHandler.Instance,
                                                      DesignTimeDialogManager.Instance)
                           .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public PhoneTypeSelectorViewModel PhoneTypeSelectorViewModel
        {
            get
            {
                return new PhoneTypeSelectorViewModel(new DesignTimeResourceRepositoryManager(EntityManagerProvider),
                                                      DesignTimeErrorHandler.Instance)
                    .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public ResourceDetailViewModel ResourceDetailViewModel
        {
            get
            {
                var rm = new DesignTimeResourceRepositoryManager(EntityManagerProvider);
                return new ResourceDetailViewModel(rm,
                                                   new ResourceSummaryViewModel(rm, null,
                                                                                DesignTimeErrorHandler.Instance,
                                                                                DesignTimeDialogManager.Instance),
                                                   new IResourceDetailSection[]
                                                       {
                                                           new ResourceContactInfoViewModel(
                                                               new ResourceAddressListViewModel(rm, null,
                                                                                                DesignTimeErrorHandler.
                                                                                                    Instance,
                                                                                                DesignTimeDialogManager.
                                                                                                    Instance),
                                                               new ResourcePhoneListViewModel(rm, null,
                                                                                              DesignTimeErrorHandler.
                                                                                                  Instance,
                                                                                              DesignTimeDialogManager.
                                                                                                  Instance)),
                                                           new ResourceRatesViewModel(rm, null,
                                                                                      DesignTimeErrorHandler.Instance,
                                                                                      DesignTimeDialogManager.Instance)
                                                           ,
                                                           new ResourceWorkExperienceViewModel(rm,
                                                                                               DesignTimeErrorHandler.
                                                                                                   Instance),
                                                           new ResourceSkillsViewModel(rm,
                                                                                       DesignTimeErrorHandler.Instance)
                                                       },
                                                   DesignTimeErrorHandler.Instance, DesignTimeDialogManager.Instance,
                                                   new BusyWatcher())
                    .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public ResourceRatesViewModel ResourceRatesViewModel
        {
            get
            {
                return (ResourceRatesViewModel)
                       new ResourceRatesViewModel(new DesignTimeResourceRepositoryManager(EntityManagerProvider), null,
                                                  DesignTimeErrorHandler.Instance, DesignTimeDialogManager.Instance).
                           Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public RateTypeSelectorViewModel RateTypeSelectorViewModel
        {
            get
            {
                return
                    new RateTypeSelectorViewModel(new DesignTimeResourceRepositoryManager(EntityManagerProvider),
                                                  DesignTimeErrorHandler.Instance).Start(
                                                      TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public ResourceWorkExperienceViewModel ResourceWorkExperienceViewModel
        {
            get
            {
                return (ResourceWorkExperienceViewModel)
                       new ResourceWorkExperienceViewModel(
                           new DesignTimeResourceRepositoryManager(EntityManagerProvider),
                           DesignTimeErrorHandler.Instance)
                           .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public ResourceSkillsViewModel ResourceSkillsViewModel
        {
            get
            {
                return (ResourceSkillsViewModel)
                       new ResourceSkillsViewModel(
                           new DesignTimeResourceRepositoryManager(EntityManagerProvider),
                           DesignTimeErrorHandler.Instance)
                           .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public ResourceSearchViewModel ResourceSearchViewModel
        {
            get
            {
                return new ResourceSearchViewModel(new ResourceRepository(EntityManagerProvider),
                                                   DesignTimeErrorHandler.Instance, new BusyWatcher()).Start();
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
                                                    DesignTimeErrorHandler.Instance, new BusyWatcher()), null, null, rm,
                        DesignTimeErrorHandler.Instance, DesignTimeDialogManager.Instance, null);
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
                                          DesignTimeErrorHandler.Instance)
                           {FailureMessage = "FailureMessage at design time"};
            }
        }

        protected override IEntityManagerProvider<TempHireEntities> CreateEntityManagerProvider()
        {
            return new DesignTimeEntityManagerProvider(new TempHireSampleDataProvider());
        }

        #region Nested type: DesignTimeDialogManager

        private class DesignTimeDialogManager : IDialogManager
        {
            private static IDialogManager _instance;

            public static IDialogManager Instance
            {
                get { return _instance ?? (_instance = new DesignTimeDialogManager()); }
            }

            #region IDialogManager Members

            public DialogOperationResult ShowDialog(object content, DialogButtons dialogButtons = DialogButtons.OkCancel,
                                                    string title = null)
            {
                throw new NotImplementedException();
            }

            public DialogOperationResult ShowMessage(string message, DialogButtons dialogButtons = DialogButtons.Ok,
                                                     string title = null)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion

        #region Nested type: DesignTimeErrorHandler

        private class DesignTimeErrorHandler : IErrorHandler
        {
            private static IErrorHandler _instance;

            public static IErrorHandler Instance
            {
                get { return _instance ?? (_instance = new DesignTimeErrorHandler()); }
            }

            #region IErrorHandler Members

            public void HandleError(Exception ex)
            {
                // noop
            }

            #endregion
        }

        #endregion
    }
}
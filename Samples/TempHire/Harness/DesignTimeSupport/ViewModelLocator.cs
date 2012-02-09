using System;
using System.Collections.Generic;
using Cocktail;
using Common.Errors;
using Common.Repositories;
using Common.SampleData;
using Common.Toolbar;
using Common.Workspace;
using DomainModel;
using TempHire.Authentication;
using TempHire.ViewModels;
using TempHire.ViewModels.Login;
using TempHire.ViewModels.StaffingResource;

namespace TempHire.DesignTimeSupport
{
    public class ViewModelLocator : DesignTimeViewModelLocatorBase<TempHireEntities>
    {
        public StaffingResourceAddressListViewModel StaffingResourceAddressListViewModel
        {
            get
            {
                return (StaffingResourceAddressListViewModel)
                       new StaffingResourceAddressListViewModel(new DesignTimeResourceRepositoryManager(EntityManagerProvider),
                                                        null, DesignTimeErrorHandler.Instance,
                                                        DesignTimeDialogManager.Instance)
                           .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public StaffingResourceSummaryViewModel StaffingResourceSummaryViewModel
        {
            get
            {
                return (StaffingResourceSummaryViewModel)
                       new StaffingResourceSummaryViewModel(new DesignTimeResourceRepositoryManager(EntityManagerProvider), null,
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

        public StaffingResourceNameEditorViewModel StaffingResourceNameEditorViewModel
        {
            get
            {
                return new StaffingResourceNameEditorViewModel(new DesignTimeResourceRepositoryManager(EntityManagerProvider),
                                                       DesignTimeErrorHandler.Instance)
                    .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public StaffingResourcePhoneListViewModel StaffingResourcePhoneListViewModel
        {
            get
            {
                return (StaffingResourcePhoneListViewModel)
                       new StaffingResourcePhoneListViewModel(new DesignTimeResourceRepositoryManager(EntityManagerProvider),
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

        public StaffingResourceDetailViewModel StaffingResourceDetailViewModel
        {
            get
            {
                var rm = new DesignTimeResourceRepositoryManager(EntityManagerProvider);
                return new StaffingResourceDetailViewModel(rm,
                                                   new StaffingResourceSummaryViewModel(rm, null,
                                                                                DesignTimeErrorHandler.Instance,
                                                                                DesignTimeDialogManager.Instance),
                                                   new IStaffingResourceDetailSection[]
                                                       {
                                                           new StaffingResourceContactInfoViewModel(
                                                               new StaffingResourceAddressListViewModel(rm, null,
                                                                                                DesignTimeErrorHandler.
                                                                                                    Instance,
                                                                                                DesignTimeDialogManager.
                                                                                                    Instance),
                                                               new StaffingResourcePhoneListViewModel(rm, null,
                                                                                              DesignTimeErrorHandler.
                                                                                                  Instance,
                                                                                              DesignTimeDialogManager.
                                                                                                  Instance)),
                                                           new StaffingResourceRatesViewModel(rm, null,
                                                                                      DesignTimeErrorHandler.Instance,
                                                                                      DesignTimeDialogManager.Instance)
                                                           ,
                                                           new StaffingResourceWorkExperienceViewModel(rm,
                                                                                               DesignTimeErrorHandler.
                                                                                                   Instance),
                                                           new StaffingResourceSkillsViewModel(rm,
                                                                                       DesignTimeErrorHandler.Instance)
                                                       },
                                                   DesignTimeErrorHandler.Instance, DesignTimeDialogManager.Instance)
                    .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public StaffingResourceRatesViewModel StaffingResourceRatesViewModel
        {
            get
            {
                return (StaffingResourceRatesViewModel)
                       new StaffingResourceRatesViewModel(new DesignTimeResourceRepositoryManager(EntityManagerProvider), null,
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

        public StaffingResourceWorkExperienceViewModel StaffingResourceWorkExperienceViewModel
        {
            get
            {
                return (StaffingResourceWorkExperienceViewModel)
                       new StaffingResourceWorkExperienceViewModel(
                           new DesignTimeResourceRepositoryManager(EntityManagerProvider),
                           DesignTimeErrorHandler.Instance)
                           .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public StaffingResourceSkillsViewModel StaffingResourceSkillsViewModel
        {
            get
            {
                return (StaffingResourceSkillsViewModel)
                       new StaffingResourceSkillsViewModel(
                           new DesignTimeResourceRepositoryManager(EntityManagerProvider),
                           DesignTimeErrorHandler.Instance)
                           .Start(TempHireSampleDataProvider.CreateGuid(1));
            }
        }

        public StaffingResourceSearchViewModel StaffingResourceSearchViewModel
        {
            get
            {
                return new StaffingResourceSearchViewModel(new StaffingResourceRepository(EntityManagerProvider),
                                                   DesignTimeErrorHandler.Instance).Start();
            }
        }

        public StaffingResourceManagementViewModel StaffingResourceManagementViewModel
        {
            get
            {
                var rm = new DesignTimeResourceRepositoryManager(EntityManagerProvider);
                return
                    new StaffingResourceManagementViewModel(
                        new StaffingResourceSearchViewModel(new StaffingResourceRepository(EntityManagerProvider),
                                                    DesignTimeErrorHandler.Instance), null, null, rm,
                        DesignTimeErrorHandler.Instance, DesignTimeDialogManager.Instance, null);
            }
        }

        public ShellViewModel ShellViewModel
        {
            get
            {
                return
                    new ShellViewModel(new List<IWorkspace>(), new ToolbarManager(), new FakeAuthenticationService(),
                                       null).Start();
            }
        }

        public LoginViewModel LoginViewModel
        {
            get
            {
                return new LoginViewModel(new FakeAuthenticationService(), null, null, DesignTimeErrorHandler.Instance)
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

            public DialogOperationResult<T> ShowDialog<T>(object content, IEnumerable<T> dialogButtons, string title = null)
            {
                throw new NotImplementedException();
            }

            public DialogOperationResult<T> ShowDialog<T>(object content, T cancelButton, IEnumerable<T> dialogButtons, string title = null)
            {
                throw new NotImplementedException();
            }

            public DialogOperationResult<DialogResult> ShowDialog(object content, IEnumerable<DialogResult> dialogButtons, string title = null)
            {
                throw new NotImplementedException();
            }

            public DialogOperationResult<T> ShowMessage<T>(string message, IEnumerable<T> dialogButtons, string title = null)
            {
                throw new NotImplementedException();
            }

            public DialogOperationResult<T> ShowMessage<T>(string message, T cancelButton, IEnumerable<T> dialogButtons, string title = null)
            {
                throw new NotImplementedException();
            }

            public DialogOperationResult<DialogResult> ShowMessage(string message, IEnumerable<DialogResult> dialogButtons, string title = null)
            {
                throw new NotImplementedException();
            }
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
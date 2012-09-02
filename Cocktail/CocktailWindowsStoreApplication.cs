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
using System.Composition;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Caliburn.Micro;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CompositionHost = IdeaBlade.Core.Composition.CompositionHost;

namespace Cocktail
{
    /// <summary>
    ///   Application base class for a Windows 8 Store app.
    /// </summary>
    public abstract class CocktailWindowsStoreApplication : Application
    {
        private readonly Type _rootViewModelType;
        private bool _isInitialized;

        static CocktailWindowsStoreApplication()
        {
            DefaultDebugLogger.SetAsLogger();
        }

        /// <summary>
        ///   Initializes the application object.
        /// </summary>
        /// <param name="rootViewModelType"> The application's default view model type. An instance of this type is active if the user launched the app or tapped a content tile. </param>
        protected CocktailWindowsStoreApplication(Type rootViewModelType)
        {
            _rootViewModelType = rootViewModelType;

            if (Execute.InDesignMode)
                InitializeDesignTime();
        }

        /// <summary>
        ///   Returns the app's root <see cref="Frame" /> control.
        /// </summary>
        public Frame RootFrame { get; private set; }

        /// <summary>
        ///   Retuns the app's root navigator which handles top level UI navigation.
        /// </summary>
        public INavigator RootNavigator { get; private set; }

        /// <summary>
        ///   Called at design time to start the framework.
        /// </summary>
        protected virtual void StartDesignTime()
        {
            AssemblySource.Instance.AddRange(SelectAssemblies());

            Configure();
            IoC.GetInstance = GetInstance;
            IoC.GetAllInstances = GetAllInstances;
            IoC.BuildUp = BuildUp;
        }

        /// <summary>
        ///   Called at runtime to start the framework.
        /// </summary>
        protected virtual void StartRuntime()
        {
            Execute.InitializeWithDispatcher();

            EventAggregator.DefaultPublicationThreadMarshaller = Execute.OnUIThread;
            AssemblySource.Instance.AddRange(SelectAssemblies());

            PrepareApplication();
            Configure();

            IoC.GetInstance = GetInstance;
            IoC.GetAllInstances = GetAllInstances;
            IoC.BuildUp = BuildUp;
        }

        /// <summary>
        ///   Provides an opportunity to hook into the application object.
        /// </summary>
        protected virtual void PrepareApplication()
        {
            Resuming += (sender, args) => OnResuming();
            Suspending += (sender, args) => OnSuspending(args);
            UnhandledException += (sender, args) => OnUnhandledException(args);

            RootFrame = CreateApplicationFrame();
            RootNavigator = CreateRootNavigator();
        }

        /// <summary>
        ///   Override to configure the framework and setup your IoC container.
        /// </summary>
        protected virtual void Configure()
        {
        }

        /// <summary>
        ///   Override to tell the framework where to find assemblies to inspect for views, etc.
        /// </summary>
        /// <returns> A list of assemblies to inspect. </returns>
        protected virtual IEnumerable<Assembly> SelectAssemblies()
        {
            return new[] {GetType().GetTypeInfo().Assembly};
        }

        /// <summary>
        ///   Configures the framework when the user launched the app normally.
        /// </summary>
        /// <param name="args"> Details about the launch request and process. </param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var shouldRestoreApplicationState = args.PreviousExecutionState == ApplicationExecutionState.Terminated;
            ConfigureFrameworkAndActivate(() => OnActivationKindLaunchAsync(args), shouldRestoreApplicationState);
        }

        /// <summary>
        ///   Displays initial content when the user launched the app normally.
        /// </summary>
        /// <param name="args"> Details about the launch request and process. </param>
        /// <returns> A <see cref="Task" /> to await completion of the activation </returns>
        protected async Task OnActivationKindLaunchAsync(LaunchActivatedEventArgs args)
        {
            if (RootFrame.Content == null)
            {
                // When the navigation stack isn't restored, navigate to the first page,
                // configuring the new page by passing the arguments that were pass to the app
                // as a navigation parameter.
                await RootNavigator.NavigateToAsync(
                    _rootViewModelType, target => Navigator.TryInjectParameter(target, args.Arguments, "Arguments"));
            }
        }

        /// <summary>
        ///   Invoked if app is launched after it had previously been terminated. Override to restore saved application state.
        /// </summary>
        /// <returns> </returns>
        protected virtual Task RestoreApplicationStateAsync()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        ///   Override to perform operations when the app transitions from Suspended state to Running state.
        /// </summary>
        protected virtual void OnResuming()
        {
        }

        /// <summary>
        ///   Override to perform operations when the app transitions from Running state to Suspended state.
        /// </summary>
        /// <param name="e"> </param>
        protected virtual void OnSuspending(SuspendingEventArgs e)
        {
        }

        /// <summary>
        ///   Override to perform operations if an app exception goes unhandled.
        /// </summary>
        /// <param name="args"> The unhandled exception details </param>
        protected virtual void OnUnhandledException(UnhandledExceptionEventArgs args)
        {
        }

        /// <summary>
        ///   Creates the app's root <see cref="Frame" /> control.
        /// </summary>
        protected virtual Frame CreateApplicationFrame()
        {
            return new Frame();
        }

        /// <summary>
        ///   Implement to instantiate the root navigator to handle top level application navigation.
        /// </summary>
        /// <returns> </returns>
        protected abstract INavigator CreateRootNavigator();

        /// <summary>
        ///   Provides an opportunity to perform asynchronous configuration at runtime.
        /// </summary>
        protected virtual Task StartRuntimeAsync()
        {
            return Task.FromResult(true);
        }

        private async Task InitializeRuntimeAsync()
        {
            if (_isInitialized)
                return;
            _isInitialized = true;

            StartRuntime();
            await StartRuntimeAsync();
        }

        private void InitializeDesignTime()
        {
            if (_isInitialized)
                return;
            _isInitialized = true;

            try
            {
                StartDesignTime();
            }
            catch
            {
                //if something fails at design-time, there's really nothing we can do...
                _isInitialized = false;
            }
        }

        private async void ConfigureFrameworkAndActivate(Func<Task> activatorAction,
                                                         bool shouldRestoreApplicationState = false)
        {
            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active.
            if (rootFrame == null)
            {
                // Initialization creates the root frame and root navigator
                await InitializeRuntimeAsync();

                if (shouldRestoreApplicationState)
                    await RestoreApplicationStateAsync();

                // Place the root frame in the current Window
                Window.Current.Content = RootFrame;
            }

            // Perform activation
            await activatorAction();

            // Ensure the current window is active
            Window.Current.Activate();
        }

        private object GetInstance(Type serviceType, string key)
        {
            return Composition.GetInstance(serviceType, key);
        }

        private IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return Composition.GetInstances(serviceType, null);
        }

        private void BuildUp(object instance)
        {
            Composition.BuildUp(instance);
        }
    }

    /// <summary>
    ///   Application base class for a Windows 8 Store app that uses MEF as the IoC implementation.
    /// </summary>
    public class CocktailMefWindowsStoreApplication : CocktailWindowsStoreApplication
    {
        private readonly MefCompositionProvider _compositionProvider;

        /// <summary>
        ///   Initializes the application object.
        /// </summary>
        /// <param name="rootViewModelType"> The type of the root view. </param>
        protected CocktailMefWindowsStoreApplication(Type rootViewModelType) : base(rootViewModelType)
        {
            _compositionProvider = new MefCompositionProvider();
        }

        /// <summary>
        ///   Override to setup up MEF export conventions.
        /// </summary>
        /// <param name="conventions"> </param>
        protected virtual void PrepareConventions(ConventionBuilder conventions)
        {
            // Automatic export of ViewModels.
            conventions
                .ForTypesMatching(type => type.Name.EndsWith("ViewModel"))
                .Export();
        }

        /// <summary>
        ///   Override to configure the framework and setup your IoC container.
        /// </summary>
        protected override void Configure()
        {
            base.Configure();

            EnsureBootstrapperHasNoExports();

            var conventions = new ConventionBuilder();
            PrepareConventions(conventions);

            _compositionProvider.Configure(conventions);
            Composition.SetProvider(_compositionProvider);

            if (RootNavigator != null)
                AddExportedValue(RootNavigator);
            Composition.BuildUp(this);
        }

        /// <summary>
        ///   Adds a singleton instance to the composition container.
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="value"> </param>
        protected void AddExportedValue<T>(T value)
        {
            _compositionProvider.AddExportedValue(value);
        }

        /// <summary>
        ///   Override to tell the framework where to find assemblies to inspect for views, etc.
        /// </summary>
        /// <returns> A list of assemblies to inspect. </returns>
        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return CompositionHost.Instance.ProbeAssemblies;
        }

        /// <summary>
        ///   Implement to instantiate the root navigator to handle top level application navigation.
        /// </summary>
        /// <returns> </returns>
        protected override INavigator CreateRootNavigator()
        {
            return new Navigator(RootFrame);
        }

        /// <summary>
        ///   Ensures that no MEF ExportAttributes are used in the Bootstrapper
        /// </summary>
        private void EnsureBootstrapperHasNoExports()
        {
            var type = GetType().GetTypeInfo();

            // Throw exception if class is decorated with ExportAttribute
            if (type.GetCustomAttributes(typeof(ExportAttribute), true).Any())
                throw new CompositionFailedException(StringResources.BootstrapperMustNotBeDecoratedWithExports);

            // Throw exception if any of the class members are decorated with ExportAttribute
            if (type.DeclaredMembers.Any(m => m.GetCustomAttributes(typeof(ExportAttribute), true).Any()))
                throw new CompositionFailedException(StringResources.BootstrapperMustNotBeDecoratedWithExports);
        }
    }
}
//====================================================================================================================
//Copyright (c) 2012 IdeaBlade
//====================================================================================================================
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//====================================================================================================================
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//the Software.
//====================================================================================================================
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using IdeaBlade.Core;

namespace Cocktail
{
    /// <summary>ViewModel implementing the Development Harness. Specify this ViewModel as the TRootModel in the Application Bootstrapper to create a Development
    /// Harness</summary>
    /// <example>
    /// 	<code title="Development Harness Bootstrapper" description="Demonstrates how to specify the HarnessView model in the Application Bootstrapper to create a Development Harness." lang="CS">
    /// public class AppBootstrapper : FrameworkBootstrapper&lt;HarnessViewModel&gt;
    /// {
    ///     // Additional code
    /// }</code>
    /// </example>
    [Export]
    public class HarnessViewModel : Conductor<object>
    {
        private readonly Dictionary<string, object> _viewModels;
        private bool _ready;

        /// <summary>Initializes a new instance.</summary>
        /// <param name="viewModels">The list of discovered ViewModels injected through MEF.</param>
        [ImportingConstructor]
        public HarnessViewModel([ImportMany] IEnumerable<IDiscoverableViewModel> viewModels)
        {
            _viewModels = new Dictionary<string, object>();
            viewModels.ForEach(vm => _viewModels.Add(vm.GetType().Name, vm));

            EventFns.Subscribe(this);
        }

        /// <summary>Bindable collection exposing the names of all discovered ViewModels.</summary>
        public BindableCollection<string> Names
        {
            get { return new BindableCollection<string>(_viewModels.Keys.OrderBy(k => k)); }
        }

        /// <summary>Indicates if the ViewModel is ready and initialized.</summary>
        public bool Ready
        {
            get { return _ready; }
            private set
            {
                _ready = value;
                NotifyOfPropertyChange(() => Ready);
            }
        }

        /// <summary>Activates the ViewModel with the given name.</summary>
        public void ActivateViewModel(string name)
        {
            object viewModel;
            if (_viewModels.TryGetValue(name, out viewModel))
            {
                var harnessAware = viewModel as IHarnessAware;
                if (harnessAware != null)
                    harnessAware.Setup();

                ActivateItem(viewModel);
            }
        }

        /// <summary>
        /// Initializing the view model
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();

#if SILVERLIGHT
            var op = FakeBackingStoreManager.Instance.InitializeAllAsync();
            op.WhenCompleted(
                args =>
                {
                    Ready = FakeBackingStoreManager.Instance.IsInitialized;

                    if (!Ready)
                        MessageBox.Show(StringResources.ThePersistenceLayerFailedToInitialize);
                });
#else
            FakeBackingStoreManager.Instance.InitializeAll();
            Ready = FakeBackingStoreManager.Instance.IsInitialized;

            if (!Ready)
                MessageBox.Show(StringResources.ThePersistenceLayerFailedToInitialize);
#endif
        }
    }
}
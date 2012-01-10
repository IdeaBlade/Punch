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

using System;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>
    /// Base class for a design time ViewModelLocator. 
    /// 
    /// To implement a concrete ViewModelLocator for your project,
    /// extend from this class and override the CreateEntityManager()
    /// method. Return an instance of type subclass of
    /// BaseDesignTimeEntityManagerProvider.
    /// 
    /// Add references to all the view models in the
    /// module or application and by adding the following lines of code
    /// for each ViewModel. Implement a Start() method to initialize
    /// the ViewModel.
    /// 
    /// <code>
    /// public CustomerListViewModel CustomerListViewModel
    /// {
    ///     get
    ///     {
    ///         return new CustomerListViewModel().Start();
    ///     }
    /// }
    /// </code>
    /// <para>
    /// In Silverlight and WPF, place the ViewModelLocator in the App.xaml resources:
    /// </para>
    /// <code>
    /// &lt;Application.Resources&gt;
    ///     &lt;designTime:ViewModelLocator xmlns:designTime="clr-namespace:your-namespace"
    ///                                  x:Key="ViewModeLocator" /&gt;
    /// &lt;/Application.Resources&gt;
    /// </code>
    /// <para>
    /// Then use:
    /// </para>
    /// <code>
    /// d:DataContext="{Binding Source={StaticResource ViewModelLocator}, Path=ViewModelName}"
    /// </code>
    /// </summary>
    /// <typeparam name="T">The type of the EntityManager</typeparam>
    public abstract class BaseDesignTimeViewModelLocator<T>
        where T : EntityManager
    {
        // ReSharper disable StaticFieldInGenericType
        private static readonly Func<bool> IsInDesignModeDefault = () => DesignModeProber.IsInDesignMode;
        // ReSharper restore StaticFieldInGenericType

        /// <summary>
        /// Function to determine if in DesignMode. Can be replaced for testing.
        /// </summary>
        // ReSharper disable StaticFieldInGenericType
        public static Func<bool> IsInDesignMode = IsInDesignModeDefault;
        // ReSharper restore StaticFieldInGenericType

        /// <summary>
        /// Restore <see cref="IsInDesignMode"/> to default method. For testing.
        /// </summary>
        public static void ResetIsInDesignModeToDefault()
        {
            IsInDesignMode = IsInDesignModeDefault;
        }

        /// <summary>
        /// Creates an instance of the concrete DesignTimeEntityManagerProvider
        /// </summary>
        protected abstract IEntityManagerProvider<T> CreateEntityManagerProvider();

        /// <summary>Returns the EntityManagerProvider used by the ViewModelLocator.</summary>
        protected IEntityManagerProvider<T> EntityManagerProvider { get { return CreateEntityManagerProvider(); } }
    }
}
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
    /// <summary>Interface identifying an EntityManagerProvider.</summary>
    public interface IEntityManagerProvider
    {
        /// <summary>
        /// Signals that a Save of at least one entity has been performed
        /// or changed entities have been imported from another entity manager.
        /// Clients may use this event to force a data refresh. 
        /// </summary>
        event EventHandler<DataChangedEventArgs> DataChanged;

        /// <summary>
        /// Returns the context in use by this provider.
        /// </summary>
        CompositionContext Context { get; }

        /// <summary>Indicates whether the persistence layer has been properly initialized.</summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Returns true if the last save operation aborted due to a validation error.
        /// </summary>
        bool HasValidationError { get; }

        /// <summary>
        /// Returns true if a save is in progress. A <see cref="InvalidOperationException"/> is thrown 
        /// if EntityManager.SaveChangesAsync is called while a previous SaveChangesAsync is still in progress.
        /// </summary>
        bool IsSaving { get; }

        /// <summary>Initializes the persistence layer.</summary>
        INotifyCompleted InitializeAsync();

#if !SILVERLIGHT

        /// <summary>Initializes the persistence layer.</summary>
        void Initialize();

#endif
    }

    /// <summary>Generic interface identifying an EntityManagerProvider.</summary>
    /// <typeparam name="T">The type of the EntityManager</typeparam>
    public interface IEntityManagerProvider<out T> : IEntityManagerProvider
        where T : EntityManager
    {
        /// <summary>Returns the EntityManager managed by this provider.</summary>
        T Manager { get; }

        /// <summary>
        /// Event fired after the EntityManager got created.
        /// </summary>
        event EventHandler<EventArgs> ManagerCreated;
    }
}
// ====================================================================================================================
// Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// ====================================================================================================================
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
// the Software.
// ====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;
using IdeaBlade.Validation;

namespace Cocktail
{
    /// <summary>
    /// Interface for a helper that saves entities asynchronously.
    /// </summary>
    public interface IEntitySaver
    {
        /// <summary>
        /// Save asynchronously the entities in the <see param="manager"/> with pending changes.
        /// </summary>
        OperationResult SaveAsync(EntityManager manager, Action onSuccess = null, Action<Exception> onFail = null);
    }

    /// <summary>
    /// A helper that saves entities asynchronously.
    /// </summary>
    public class EntitySaver : IEntitySaver
    {
        /// <summary>Create an <see cref="EntitySaver"/></summary>
        /// <param name="entityManagerDelegates">
        /// Delegate(s) that invoke custom validation of certain entities.
        /// </param>
        /// <param name="validationErrorNotifiers">
        /// Notification delegate(s) invoked when there are validation errors.
        /// </param>
        /// <remarks>
        /// The delegate lists are passed through to the
        /// </remarks>
        public EntitySaver(
            IEnumerable<EntityManagerDelegate> entityManagerDelegates = null,
            IEnumerable<IValidationErrorNotification> validationErrorNotifiers = null)
        {
            EntityManagerDelegates = entityManagerDelegates ;
            ValidationErrorNotifiers = validationErrorNotifiers;
        }

        public OperationResult SaveAsync(EntityManager manager, Action onSuccess = null, Action<Exception> onFail = null)
        {
            try
            {
                return SaveAsyncCore(manager, onSuccess, onFail);
            }
            catch (Exception error)
            {
                var nc = new CompletedImmediately(error);
                if (null != onFail)
                {
                    onFail(nc.Error);
                    nc.IsErrorHandled = true;
                }
                return nc.AsOperationResult();
            }
        }

        protected virtual OperationResult SaveAsyncCore(EntityManager manager, Action onSuccess, Action<Exception> onFail)
        {
            if (null == manager) throw new ArgumentNullException("manager");

            var userState = CreateUserState();
            var entitySaverHandlers = CreateEntitySaverHandlers(manager, userState, onSuccess, onFail);
            entitySaverHandlers.AddSavingHandler(manager);

            return manager.SaveChangesAsync(
                SaveOptions,
                op => entitySaverHandlers.OnSaveCompleted(
                    manager,
                    op.UserState,
                    ((IHasAsyncEventArgs)op).AsyncArgs, 
                    onSuccess, onFail),
                userState
                ).AsOperationResult();
        }

        /// <summary>
        /// Create the UserState object that distinguishes one async save call from another.
        /// </summary>
        /// <remarks>
        /// Typically an identifier but can be any object. 
        /// That object is preserved across the async operation lifetime 
        /// but will not be serialized or presented to the server.
        /// </remarks>
        protected virtual object CreateUserState()
        {
            return Guid.NewGuid();
        }

        /// <summary>Get and set <see cref="SaveOptions"/>.</summary>
        protected virtual SaveOptions SaveOptions { get; set; }

        /// <summary>
        /// Create a new instance of <see cref="IEntitySaverHandlers"/>
        /// to assist in processing the save
        /// </summary>
        protected virtual IEntitySaverHandlers CreateEntitySaverHandlers(
            EntityManager manager, object userState, Action onSuccess, Action<Exception> onFail)
        {
            return new EntitySaverHandlers(userState, EntityManagerDelegates, ValidationErrorNotifiers);
        }

        /// <summary>
        /// Get the delegate(s) that invoke custom validation of certain entities.
        /// </summary>
        protected IEnumerable<EntityManagerDelegate> EntityManagerDelegates { get; set; }

        /// <summary>
        /// Get the notification delegate(s) invoked when there are validation errors.
        /// </summary>
        protected IEnumerable<IValidationErrorNotification> ValidationErrorNotifiers { get; set; }
    }

    /// <summary>
    /// Interface for handlers involved in the process 
    /// of saving entity changes within an <see cref="EntitySaver"/>.
    /// </summary>
    public interface IEntitySaverHandlers
    {
        /// <summary>
        /// Add a saving handler to preprocess the entity change-set once
        /// an <see cref="EntityManager"/> begins saving changes.
        /// </summary>
        /// <remarks>
        /// The SavingHandler that is added to the manager 
        /// should be a one-time handler and should remove itself when called.
        /// If not removed immediately, it could be invoked a second time
        /// by a second SaveChangesAsync call which likely will corrupt
        /// any state that is supposed to be available when the Save completes.
        /// See how <see cref="EntitySaverHandlers"/> does it.
        /// </remarks>
        void AddSavingHandler(EntityManager manager);

        /// <summary>
        /// Remove the saving handler that preprocessed the entity change-set 
        /// when <see cref="EntityManager"/> began saving changes.
        /// </summary>
        void RemoveSavingHandler(EntityManager manager);

        /// <summary>
        /// Handler invoked in the <see cref="EntityManager.Saving"/>
        /// when the save completes
        /// </summary>
        /// <remarks>
        /// Invoked by <see cref="EntitySaver.SaveAsyncCore"/>.
        /// Be sure to process the callbacks
        /// You can do so before, after, or in the middle of other
        /// "SaveCompleted" work.
        /// </remarks>
        void OnSaveCompleted(
            EntityManager manager, object userState, INotifyCompletedArgs args, 
            Action onSuccess, Action<Exception> onFail);
    }

    /// <summary>
    /// Handlers involved in the process 
    /// of saving entity changes within an <see cref="EntitySaver"/>.
    /// </summary>
    public class EntitySaverHandlers : IEntitySaverHandlers
    {
        /// <summary>Create an <see cref="EntitySaverHandlers"/></summary>
        /// <param name="userState">
        /// Marker object that distinguishes one async save call from another
        /// and one instance of this type from another.
        /// </param>
        /// <param name="entityManagerDelegates">
        /// Delegate(s) that invoke custom validation of certain entities.
        /// </param>
        /// <param name="validationErrorNotifiers">
        /// Notification delegate(s) invoked when there are validation errors.
        /// </param>
        public EntitySaverHandlers(
            object userState,
            IEnumerable<EntityManagerDelegate> entityManagerDelegates = null,
            IEnumerable<IValidationErrorNotification> validationErrorNotifiers = null)
        {
            UserState = userState;
            EntityManagerDelegates = entityManagerDelegates ?? new EntityManagerDelegate[0];
            ValidationErrorNotifiers = validationErrorNotifiers ?? new IValidationErrorNotification[0];
        }

        /// <summary>
        /// Get the UserState object that distinguishes one async save call from another
        /// and one instance of this <see cref="EntitySaverHandlers"/> type from another.
        /// </summary>
        /// <remarks>
        /// Typically an identifier but can be any object. 
        /// That object is preserved across the async operation lifetime.
        /// </remarks>
        public object UserState { get; private set; }

        public virtual void AddSavingHandler(EntityManager manager)
        {
            manager.Saving -= SavingHandler; // paranoia
            manager.Saving += SavingHandler;
        }

        public virtual void RemoveSavingHandler(EntityManager manager)
        {
            manager.Saving -= SavingHandler;
        }

        public virtual void OnSaveCompleted(
            EntityManager manager, object userState, INotifyCompletedArgs args, 
            Action onSuccess, Action<Exception> onFail)
        {
            CompletedCallback(args, onSuccess, onFail);
        }  

        protected void CompletedCallback(INotifyCompletedArgs args, Action onSuccess, Action<Exception> onFail)
        {
            if (null == args.Error)
            {
                if (null != onSuccess) onSuccess();
            }
            else
            {
                if (null != onFail)
                {
                    args.IsErrorHandled = true;
                    onFail(args.Error);
                }
            }            
        }

        private void SavingHandler(object sender, EntitySavingEventArgs args)
        {
            var manager = (EntityManager)sender;
            RemoveSavingHandler(manager);
            args.Cancel = OnSaving(manager, args.Entities, args.Cancel);
        }

        /// <summary>
        /// Performs the preprocessing of the entities to be saved [Internal Use Only]
        /// </summary>
        /// <param name="manager">The <see cref="EntityManager"/> that will do the saving</param>
        /// <param name="entities">Entities to save; presumed to be in the <see param="manager"/>.</param>
        /// <param name="isCanceled">True if the save should be canceled; default is false.</param>
        /// <returns>True if the save should be canceled.</returns>
        /// <remarks>
        /// This method is public only to facilitate testing of pre-save processing logic.
        /// </remarks>
        public virtual bool OnSaving(EntityManager manager, IList<object> entities, bool isCanceled=false )
        {
            if (isCanceled) return true;
            PreprocessChangeSet(manager, entities);
            var errs = ValidateChangeSet(manager, entities);
            if (errs.HasErrors)
            {
                NotifyValidationErrors(errs);
                isCanceled = true;
            }
            return isCanceled;
        }

        protected virtual void PreprocessChangeSet(EntityManager manager, IList<object> entities) { }

        protected virtual VerifierResultCollection ValidateChangeSet(EntityManager manager, IEnumerable<object> entities)
        {

            // Validation errors for all change-set entities
            var allValidationErrors = new VerifierResultCollection();
            var verifierEngine = manager.VerifierEngine;

            foreach (var entity in entities)
            {
                var entityAspect = EntityAspect.Wrap(entity);

                // DevForce Verification
                var validationResults =
                    entityAspect.EntityState.IsAddedOrModified()
                        ? verifierEngine.Execute(entity)
                        : new VerifierResultCollection();

                // Custom entity validation (if specified via an EntityManagerDelegate)
                foreach (var validator in EntityManagerDelegates)
                {
                    validator.Validate(entity, validationResults);
                }

                // Keep only the validation error results
                validationResults = validationResults.Errors;

                // Add non-duplicate errant VerifierResults to the entity's collection
                validationResults.Where(vr => !entityAspect.ValidationErrors.Contains(vr))
                    .ForEach(entityAspect.ValidationErrors.Add);

                // Add this entity's errors to the list of all change-set entity errors
                validationResults.ForEach(allValidationErrors.Add);
            }

            return allValidationErrors;
        }

        protected IEnumerable<EntityManagerDelegate> EntityManagerDelegates { get; set; }

        /// <summary>
        /// Invoke <see cref="ValidationErrorNotifiers"/> if there are save validation errors.
        /// </summary>
        /// <param name="errors">Save validation errors.</param>
        /// <remarks>
        /// If there are errors and no notification delegates in <see cref="ValidationErrorNotifiers"/>
        /// an exception is thrown with the validation error message(s).
        /// </remarks>
        protected virtual void NotifyValidationErrors(VerifierResultCollection errors)
        {
            if (!errors.HasErrors) return;
            if (!ValidationErrorNotifiers.Any())
                throw new ValidationException(errors.Select(v => v.Message).ToAggregateString("\n"));

            ValidationErrorNotifiers.ForEach(s => s.OnValidationError(errors));
        }

        protected IEnumerable<IValidationErrorNotification> ValidationErrorNotifiers { get; set; }
    }
}
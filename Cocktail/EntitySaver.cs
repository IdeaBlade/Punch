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
    public interface IEntitySaver
    {
        OperationResult SaveAsync(EntityManager manager, Action onSuccess = null, Action<Exception> onFail = null);
    }

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
                var nc = new NotifyCompletedImmediately(error);
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

            var entitySaverHandlers = CreateEntitySaverHandlers(manager, onSuccess, onFail);
            entitySaverHandlers.AddSavingHandler(manager);
            return manager.SaveChangesAsync(op =>
                                         {
                                             entitySaverHandlers.RemoveSavingHandler(manager);
                                             entitySaverHandlers.OnSaveCompleted(
                                                 ((IHasAsyncEventArgs)op).AsyncArgs, 
                                                 onSuccess, onFail, 
                                                 manager);
                                                                         
                                         }).AsOperationResult();
        }

        /// <summary>
        /// Create a new instance of <see cref="IEntitySaverHandlers"/>
        /// to assist in processing the save
        /// </summary>
        protected virtual IEntitySaverHandlers CreateEntitySaverHandlers(
            EntityManager manager, Action onSuccess, Action<Exception> onFail)
        {
            return new EntitySaverHandlers(EntityManagerDelegates, ValidationErrorNotifiers);
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
        void OnSaveCompleted(INotifyCompletedArgs args, Action onSuccess, Action<Exception> onFail, EntityManager manager);
    }

    /// <summary>
    /// Handlers involved in the process 
    /// of saving entity changes within an <see cref="EntitySaver"/>.
    /// </summary>
    public class EntitySaverHandlers : IEntitySaverHandlers
    {
        /// <summary>Create an <see cref="EntitySaverHandlers"/></summary>
        /// <param name="entityManagerDelegates">
        /// Delegate(s) that invoke custom validation of certain entities.
        /// </param>
        /// <param name="validationErrorNotifiers">
        /// Notification delegate(s) invoked when there are validation errors.
        /// </param>
        public EntitySaverHandlers(
            IEnumerable<EntityManagerDelegate> entityManagerDelegates = null,
            IEnumerable<IValidationErrorNotification> validationErrorNotifiers = null)
        {
            EntityManagerDelegates = entityManagerDelegates ?? new EntityManagerDelegate[0];
            ValidationErrorNotifiers = validationErrorNotifiers ?? new IValidationErrorNotification[0];
        }

        public virtual void AddSavingHandler(EntityManager manager)
        {
            RemoveSavingHandler(manager); // paranoia
            manager.Saving += SavingHandler;
        }

        public virtual void RemoveSavingHandler(EntityManager manager)
        {
            manager.Saving -= SavingHandler;
        }

        public virtual void OnSaveCompleted(INotifyCompletedArgs args, Action onSuccess, Action<Exception> onFail, EntityManager manager)
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
            manager.Saving -= SavingHandler;
            args.Cancel = OnSaving(manager, args.Entities, args.Cancel);
        }

        protected virtual bool OnSaving(EntityManager manager, IList<object> entities, bool isCanceled=false )
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

            foreach (var entity in entities)
            {
                var entityAspect = EntityAspect.Wrap(entity);

                // DevForce Verification
                var validationResults =
                    entityAspect.EntityState.IsAddedOrModified()
                        ? manager.VerifierEngine.Execute(entity)
                        : new VerifierResultCollection();

                // Custom entity validation (if specified via a EntityManagerDelegate)
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
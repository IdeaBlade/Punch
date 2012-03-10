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
using System.Linq;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>Manages and provides an EntityManager.</summary>
    /// <typeparam name="T">The type of the EntityManager</typeparam>
    public class EntityManagerProvider<T> : EntityManagerProviderCore<T>
        where T : EntityManager
    {
        private bool _isSaving;

        private T _manager;

        /// <summary>Returns the EntityManager managed by this provider.</summary>
        public override T Manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = CreateEntityManagerCore();
                    OnManagerCreated();
                }
                return _manager;
            }
        }

        /// <summary>
        /// Returns true if a save is in progress. A <see cref="InvalidOperationException"/> is thrown 
        /// if EntityManager.SaveChangesAsync is called while a previous SaveChangesAsync is still in progress.
        /// </summary>
        public override bool IsSaving
        {
            get { return _isSaving; }
        }

        /// <summary>
        /// Creates a new EntityManagerProvider from the current EntityManagerProvider and assigns the specified <see cref="ConnectionOptions"/> name.
        /// </summary>
        /// <param name="connectionOptionsName">The ConnectionOptions name to be assigned.</param>
        /// <returns>A new EntityManagerProvider instance.</returns>
        public new EntityManagerProvider<T> With(string connectionOptionsName)
        {
            return (EntityManagerProvider<T>) base.With(connectionOptionsName);
        }

        private T CreateEntityManagerCore()
        {
            Composition.BuildUp(this);
            T manager = CreateEntityManager();
            manager.Saving += new EventHandler<EntitySavingEventArgs>(OnSaving).MakeWeak(eh => manager.Saving -= eh);
            manager.Saved += new EventHandler<EntitySavedEventArgs>(OnSaved).MakeWeak(eh => manager.Saved -= eh);
            return manager;
        }

        private void OnSaved(object sender, EntitySavedEventArgs args)
        {
            _isSaving = false;
            OnDataChanged(EntityAspect.WrapAll(args.Entities).Select(e => e.EntityKey));
        }

        private void OnSaving(object sender, EntitySavingEventArgs args)
        {
            _isSaving = true;
        }
    }
}
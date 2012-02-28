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
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>Interface identifying an EntityManagerProvider.</summary>
    public interface IEntityManagerProvider : IHideObjectMembers
    {
        /// <summary>
        /// Returns true if the last save operation aborted due to a validation error.
        /// </summary>
        bool HasValidationError { get; }

        /// <summary>
        /// Returns true if a save is in progress. A <see cref="InvalidOperationException"/> is thrown 
        /// if EntityManager.SaveChangesAsync is called while a previous SaveChangesAsync is still in progress.
        /// </summary>
        bool IsSaving { get; }

        /// <summary>
        /// Specifies the ConnectionOptions used by the current EntityManagerProvider.
        /// </summary>
        ConnectionOptions ConnectionOptions { get; }

        /// <summary>
        /// Returns the EntityManager managed by this provider.
        /// </summary>
        EntityManager Manager { get; }

        /// <summary>
        /// Signals that a Save of at least one entity has been performed
        /// or changed entities have been imported from another entity manager.
        /// Clients may use this event to force a data refresh. 
        /// </summary>
        event EventHandler<DataChangedEventArgs> DataChanged;

        /// <summary>
        /// Event fired after the EntityManager got created.
        /// </summary>
        event EventHandler<EventArgs> ManagerCreated;
    }

    /// <summary>Generic interface identifying an EntityManagerProvider.</summary>
    /// <typeparam name="T">The type of the EntityManager</typeparam>
    public interface IEntityManagerProvider<out T> : IEntityManagerProvider
        where T : EntityManager
    {
        /// <summary>Returns the EntityManager managed by this provider.</summary>
        new T Manager { get; }
    }
}
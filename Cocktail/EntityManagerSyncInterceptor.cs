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

using IdeaBlade.Core;
using IdeaBlade.Core.Composition;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>
    /// Controls the syncing of Entities between multiple EntityManagers.
    /// By default no entities are being synced.
    /// 
    /// To sync entities, create a subclass of EntityManagerSyncInterceptor
    /// and override the inherited methods to control the import and export
    /// of entities.
    /// </summary>
    [InterfaceExport(typeof(IEntityManagerSyncInterceptor))]
    public abstract class EntityManagerSyncInterceptor : IEntityManagerSyncInterceptor, IHideObjectMembers
    {
        /// <summary>Provides access to the importing or exporting EntityManager.</summary>
        /// <value>The instance of the EntityManager currently in the process of exporting or importing changed entities.</value>
        protected internal EntityManager EntityManager { get; internal set; }

        /// <summary>
        /// Controls which entities should be imported to the EntityManager
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool ShouldImportEntity(object entity)
        {
            return false;
        }

        /// <summary>
        /// Controls which entities should be exported from an EntityManager
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool ShouldExportEntity(object entity)
        {
            return false;
        }
    }
}
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

using Cocktail;
using DomainModel;
using IdeaBlade.EntityModel;

namespace TempHire
{
    public class SyncInterceptor : EntityManagerSyncInterceptor
    {
        public override bool ShouldExportEntity(object entity)
        {
            return true;
        }

        public override bool ShouldImportEntity(object entity)
        {
            // Only import if the importing EntityManager holds a copy of the entity or its root aggregate in its cache
            if (EntityManager.FindEntity(EntityAspect.Wrap(entity).EntityKey) != null)
                return true;

            var hasRoot = entity as IHasRoot;
            if (hasRoot != null)
                return EntityManager.FindEntity(EntityAspect.Wrap(hasRoot.Root).EntityKey) != null;

            return false;
        }
    }
}
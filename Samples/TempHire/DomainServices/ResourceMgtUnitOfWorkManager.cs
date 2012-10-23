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
using System.ComponentModel.Composition;
using Cocktail;

namespace DomainServices
{
    /// <summary>
    ///   Used to share instances of the ResourceMgtUnitOfWork among composed view models.
    /// </summary>
    [Export(typeof(IResourceMgtUnitOfWorkManager<IResourceMgtUnitOfWork>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ResourceMgtUnitOfWorkManager : ObjectManager<Guid, IResourceMgtUnitOfWork>,
                                           IResourceMgtUnitOfWorkManager<IResourceMgtUnitOfWork>
    {
        #region IResourceMgtUnitOfWorkManager<IDomainUnitOfWork> Members

        public IResourceMgtUnitOfWork Get(Guid key)
        {
            return GetObject(key);
        }

        #endregion
    }
}
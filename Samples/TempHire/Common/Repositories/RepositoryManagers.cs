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
using System.ComponentModel.Composition;
using Cocktail;

namespace Common.Repositories
{
    public class RepositoryManager<T> : ObjectManager<Guid,T>, IRepositoryManager<T>
    {
        public T GetRepository(Guid key)
        {
            return GetObject(key);
        }
    }

    /// <summary>
    /// Used to share instances of the StaffingResourceRepository among composed view models.
    /// </summary>
    [Export(typeof(IRepositoryManager<IStaffingResourceRepository>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class StaffingResourceRepositoryManager : RepositoryManager<IStaffingResourceRepository>
    {
    }
}
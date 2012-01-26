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

using System.Collections.Generic;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>
    /// Internal use.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SyncDataMessage<T>
        where T : EntityManager
    {
        private readonly IEnumerable<EntityKey> _deletedEntityKeys;
        private readonly IEnumerable<object> _savedEntities;
        private readonly IEntityManagerProvider<T> _source;

        /// <summary>Internal use.</summary>
        public SyncDataMessage(IEntityManagerProvider<T> source, IEnumerable<object> savedEntities,
                        IEnumerable<EntityKey> deletedEntityKeys)
        {
            _source = source;
            _savedEntities = savedEntities ?? new List<object>();
            _deletedEntityKeys = deletedEntityKeys ?? new List<EntityKey>();
        }

        public IEntityManagerProvider<T> Source
        {
            get { return _source; }
        }

        /// <summary>Internal use.</summary>
        public IEnumerable<object> SavedEntities
        {
            get { return _savedEntities; }
        }

        /// <summary>Internal use.</summary>
        public IEnumerable<EntityKey> DeletedEntityKeys
        {
            get { return _deletedEntityKeys; }
        }

        /// <summary>Internal use.</summary>
        public bool IsSameProviderAs(IEntityManagerProvider<T> provider)
        {
            return ReferenceEquals(provider, _source);
        }
    }
}
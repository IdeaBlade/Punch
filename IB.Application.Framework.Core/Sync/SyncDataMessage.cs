//====================================================================================================================
//Copyright (c) 2011 IdeaBlade
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

using System.Collections.Generic;
using IdeaBlade.Application.Framework.Core.Persistence;
using IdeaBlade.EntityModel;

namespace IdeaBlade.Application.Framework.Core.Sync
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
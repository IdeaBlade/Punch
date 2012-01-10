//====================================================================================================================
//Copyright (c) 2012 IdeaBlade
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

using System;
using System.Collections.Generic;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>
    /// Event arguments for events signaling that entities have changed
    /// </summary>
    public class DataChangedEventArgs : EventArgs
    {
        private readonly EntityManager _manager;

        /// <summary>Constructs a new DataChangedEventArgs</summary>
        /// <param name="entityKeys">The list of entity keys that were added, modified or deleted</param>
        /// <param name="manager">The EntityManager that triggered the event. For internal use.</param>
        public DataChangedEventArgs(IEnumerable<EntityKey> entityKeys, EntityManager manager)
        {
            EntityKeys = entityKeys;
            _manager = manager;
        }

        /// <summary>Contains the list of entity keys that were added, modified or deleted</summary>
        public IEnumerable<EntityKey> EntityKeys { get; private set; }

        /// <summary>Determines if a matching entity for the provided key exists in the cache of the EntityManager that triggered the event.</summary>
        /// <param name="key">The entity's key.</param>
        /// <returns>True if the entity exists.</returns>
        public bool EntityExists(EntityKey key)
        {
            return _manager.FindEntity(key) != null;
        }

        /// <summary>Gets the complete entity from the EntityManager that triggered the event.</summary>
        /// <param name="key">The entities key.</param>
        /// <returns>An object representing the entity or null if it doesn't exist.</returns>
        public object GetEntity(EntityKey key)
        {
            return _manager.FindEntity(key);
        }
    }
}
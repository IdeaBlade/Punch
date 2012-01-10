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

using System.ComponentModel.Composition;

namespace Cocktail
{
    /// <summary>The ObjectManager allows for the creation and sharing of multiple object instances at runtime. It utilizes weak references to automatically free
    /// instances that are no longer used anywhere in the application. It automatically creates new instances for keys that are being encountered for the first time or
    /// after a previous instance with the same key has been released.</summary>
    /// <typeparam name="TKey">The type of the key used to retrieve a shared instance.</typeparam>
    /// <typeparam name="T">The type of the instance.</typeparam>
    public class ObjectManager<TKey, T>
    {
        private readonly WeakRefDictionary<TKey, T> _objects;

        /// <summary>Initializes a new ObjectManager</summary>
        protected ObjectManager()
        {
            _objects = new WeakRefDictionary<TKey, T>();
        }

#if SILVERLIGHT  // ExportFactory is only available in Silverlight
        /// <summary>Used internally to create new instances.</summary>
        [Import]
        public ExportFactory<T> ExportFactory { get; set; }
#endif

        /// <summary>Retrieves an object instance by key. If the key hasn't been encountered before, a new instance will be created.</summary>
        /// <param name="key">The key used to look up the instance. If the key is encountered for the first time, a new instance will be created.</param>
        /// <returns>An existing or new instance matching the key.</returns>
        public T GetObject(TKey key)
        {
            T obj;

            if (_objects.TryGetValue(key, out obj)) return obj;

            // No or dead repository for the given key. Create a new one.
            obj = Create();
            _objects.Add(key, obj);

            return obj;
        }

        /// <summary>Creates an object instance without adding it to the internal managed dictionary.</summary>
        /// <returns>Returns the new instance. Can later be added with <see cref="Add"/></returns>
        public T Create()
        {
#if SILVERLIGHT
            return ExportFactory.CreateExport().Value;
#else
            return CompositionHelper.GetInstance<T>(CreationPolicy.NonShared);
#endif
        }

        /// <summary>Removes all instances and keys from the manager.</summary>
        public void Clear()
        {
            _objects.Clear();
        }

        /// <summary>Allows to manually add an object instance to the ObjectManager.</summary>
        /// <param name="key">The key under which the instance should be filed away.</param>
        /// <param name="obj">The object instance to be added.</param>
        public void Add(TKey key, T obj)
        {
            _objects.Add(key, obj);
        }

        /// <summary>Allows to manually remove an object instance from the manager. A subsequent call to GetObject with the same key will create a new instance.</summary>
        /// <param name="key">The key of the object instance to be removed.</param>
        public void Remove(TKey key)
        {
            _objects.Remove(key);
        }

        /// <summary>Check if an instance with the given key exists.</summary>
        /// <param name="key">The key of the instance.</param>
        /// <returns>Returns true if the instance with the key exists.</returns>
        public bool Exists(TKey key)
        {
            return _objects.ContainsKey(key);
        }
    }
}

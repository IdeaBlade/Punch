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
using System.Collections.Generic;
using System.Linq;

#if WinRT
using IdeaBlade.Core;
#endif

namespace Cocktail
{

    /// <summary>
    /// Represents a dictionary which stores the values as weak references instead of strong
    /// references. Null values are supported.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class WeakRefDictionary<TKey, TValue>
    {
        /// <summary>
        /// Retrieves a value from the dictionary.
        /// </summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>The value in the dictionary.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the key does exist in the dictionary.
        /// Since the dictionary contains weak references, the key may have been removed by the
        /// garbage collection of the object.</exception>
        public TValue this[TKey key]
        {
            get
            {
                CleanIfNeeded();

                TValue result;

                if (TryGetValue(key, out result)) return result;

                throw new KeyNotFoundException();
            }
        }

        /// <summary>
        /// Returns a count of the number of items in the dictionary.
        /// </summary>
        /// <remarks>Since the items in the dictionary are held by weak reference, the count value
        /// cannot be relied upon to guarantee the number of objects that would be discovered via
        /// enumeration. Treat the Count as an estimate only.  This property also has the side effect 
        /// of clearing out any GC'd refs.</remarks>
        public int Count
        {
            get
            {
                CleanAbandonedItems();
                return _inner.Count;
            }
        }

        /// <summary>
        /// Adds a new item to the dictionary.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(TKey key, TValue value)
        {
            TValue dummy;

            CleanIfNeeded();

            if (TryGetValue(key, out dummy))
            {
                throw new ArgumentException(StringResources.KeyIsAlreadyPresentInThisDictionary);
            }

            _inner.Add(key, new WeakReference(EncodeNullObject(value)));
        }

        /// <summary>
        /// Determines if the dictionary contains a value for the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            TValue dummy;
            return TryGetValue(key, out dummy);
        }

        /// <summary>
        /// Gets an enumerator over the values in the dictionary.
        /// </summary>
        /// <returns>The enumerator.</returns>
        /// <remarks>As objects are discovered and returned from the enumerator, a strong reference
        /// is temporarily held on the object so that it will continue to exist for the duration of
        /// the enumeration. Once the enumeration of that object is over, the strong reference is
        /// removed. If you wish to keep values alive for use after enumeration, to ensure that they
        /// stay alive, you should store strong references to them during enumeration.</remarks>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (KeyValuePair<TKey, WeakReference> kvp in _inner)
            {
                object innerValue = kvp.Value.Target;

                if (innerValue != null)
                {
                    yield return new KeyValuePair<TKey, TValue>(kvp.Key, DecodeNullObject<TValue>(innerValue));
                }
            }
        }

        /// <summary>
        /// Removes an item from the dictionary.
        /// </summary>
        /// <param name="key">The key of the item to be removed.</param>
        /// <returns>Returns true if the key was in the dictionary; return false otherwise.</returns>
        public bool Remove(TKey key)
        {
            return _inner.Remove(key);
        }

        /// <summary>
        /// Attempts to get a value from the dictionary.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <returns>Returns true if the value was present; false otherwise.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            WeakReference wr;

            if (!_inner.TryGetValue(key, out wr)) return false;

            object result = wr.Target;

            if (result == null)
            {
                _inner.Remove(key);
                return false;
            }

            value = DecodeNullObject<TValue>(result);
            return true;
        }

        /// <summary>Removes all keys and values from the Dictionary.</summary>
        public void Clear()
        {
            _inner.Clear();
        }

        private object EncodeNullObject(object value)
        {
            if (value == null)
            {
                return typeof(NullObject);
            }
            return value;
        }

        private TObject DecodeNullObject<TObject>(object innerValue)
        {
            if (innerValue is Type && ((Type)innerValue) == typeof(NullObject))
            {
                return default(TObject);
            }
            return (TObject)innerValue;
        }


        private class NullObject
        {
        }


        /// <summary>
        /// Perform cleanup if GC occurred
        /// </summary>
        private void CleanIfNeeded()
        {
            if (_gcSentinal.Target == null)
            {
                CleanAbandonedItems();
                _gcSentinal = new WeakReference(new Object());
            }
        }

        private void CleanAbandonedItems()
        {
            //List<TKey> deadKeys = new List<TKey>();
            //foreach (KeyValuePair<TKey, WeakReference> kvp in _inner) {
            //  if (kvp.Value.Target == null) deadKeys.Add(kvp.Key);
            //}
            // foreach (TKey key in deadKeys) _inner.Remove(key);

            _inner.Where(kvp => kvp.Value.Target == null)
             .Select(kvp => kvp.Key)
             .ToList()
             .ForEach(k => _inner.Remove(k));
        }

        private readonly Dictionary<TKey, WeakReference> _inner = new Dictionary<TKey, WeakReference>();
        /// <summary>
        /// Serves as a simple "GC Monitor" that indicates whether cleanup is needed. 
        /// If _gcSentinal.IsAlive is false, GC has occurred and we should perform cleanup
        /// </summary>
        private WeakReference _gcSentinal = new WeakReference(new Object());
    }

}



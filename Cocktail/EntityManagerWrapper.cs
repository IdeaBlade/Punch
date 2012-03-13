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
using IdeaBlade.EntityModel;

namespace Cocktail
{
    internal class EntityManagerWrapper<T> where T : EntityManager
    {
        public EntityManagerWrapper(T manager)
        {
            Manager = manager;
            manager.Cleared += (sender, args) => PublishEvent(args);
            manager.EntityChanged += (sender, args) => PublishEvent(args);
            manager.EntityChanging += (sender, args) => PublishEvent(args);
            manager.EntityServerError += (sender, args) => PublishEvent(args);
            manager.Fetching += (sender, args) => PublishEvent(args);
            manager.Queried += (sender, args) => PublishEvent(args);
            manager.Querying += (sender, args) => PublishEvent(args);
            manager.Saving += (sender, args) => PublishEvent(args);
            manager.Saved += (sender, args) => PublishEvent(args);
        }

        public T Manager { get; private set; }

        private void PublishEvent(EventArgs eventArgs)
        {
            EventFns.Publish(new EntityManagerEventMessage<T>(Manager, eventArgs));
        }
    }
}
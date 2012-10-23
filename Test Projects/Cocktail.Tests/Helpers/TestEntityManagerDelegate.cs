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

using Test.Model;

#if !NETFX_CORE
using System.ComponentModel.Composition;
#else
using System.Composition;
#endif

namespace Cocktail.Tests.Helpers
{
    [PartNotDiscoverable]
    public class TestEntityManagerDelegate : EntityManagerDelegate<NorthwindIBEntities>
    {
        public int QueryingRaised;
        public int QueriedRaised;
        public int FetchingRaised;
        public int SavingRaised;
        public int SavedRaised;
        public int EntityChangingRaised;
        public int EntityChangedRaised;
        public int ClearedRaised;

        private int _counter;

        public override void OnQuerying(NorthwindIBEntities source, IdeaBlade.EntityModel.EntityQueryingEventArgs args)
        {
            QueryingRaised = ++_counter;
        }

        public override void OnQueried(NorthwindIBEntities source, IdeaBlade.EntityModel.EntityQueriedEventArgs args)
        {
            QueriedRaised = ++_counter;
        }

        public override void OnFetching(NorthwindIBEntities source, IdeaBlade.EntityModel.EntityFetchingEventArgs args)
        {
            FetchingRaised = ++_counter;
        }

        public override void OnSaving(NorthwindIBEntities source, IdeaBlade.EntityModel.EntitySavingEventArgs args)
        {
            SavingRaised = ++_counter;
        }

        public override void OnSaved(NorthwindIBEntities source, IdeaBlade.EntityModel.EntitySavedEventArgs args)
        {
            SavedRaised = ++_counter;
        }

        public override void OnEntityChanging(NorthwindIBEntities source, IdeaBlade.EntityModel.EntityChangingEventArgs args)
        {
            EntityChangingRaised = ++_counter;
        }

        public override void OnEntityChanged(NorthwindIBEntities source, IdeaBlade.EntityModel.EntityChangedEventArgs args)
        {
            EntityChangedRaised = ++_counter;
        }

        public override void OnCleared(NorthwindIBEntities source, IdeaBlade.EntityModel.EntityManagerClearedEventArgs args)
        {
            ClearedRaised = ++_counter;
        }
    }
}
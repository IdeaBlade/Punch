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
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;

namespace Common.Toolbar
{
    [Export(typeof (IToolbarManager)), PartCreationPolicy(CreationPolicy.Shared)]
    public class ToolbarManager : PropertyChangedBase, IToolbarManager
    {
        private readonly List<ToolbarGroup> _groups;

        public ToolbarManager()
        {
            _groups = new List<ToolbarGroup>();
        }

        #region IToolbarManager Members

        public IEnumerable<ToolbarAction> Actions
        {
            get { return Groups.SelectMany(g => g); }
        }

        public IEnumerable<ToolbarGroup> Groups
        {
            get { return _groups.OrderBy(g => g.Level); }
        }

        public void AddGroup(ToolbarGroup @group)
        {
            _groups.Add(@group);
            NotifyOfPropertyChange(() => Actions);
            NotifyOfPropertyChange(() => Groups);
        }

        public void RemoveGroup(ToolbarGroup @group)
        {
            _groups.Remove(@group);
            NotifyOfPropertyChange(() => Actions);
            NotifyOfPropertyChange(() => Groups);
        }

        public void Clear()
        {
            _groups.Clear();
        }

        #endregion
    }
}
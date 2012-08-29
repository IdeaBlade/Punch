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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Cocktail;

namespace TempHire.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class ItemSelectorViewModel : Screen
    {
        private string _displayMemberPath;
        private BindableCollection<object> _items;
        private string _label;
        private object _selectedItem;

        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                NotifyOfPropertyChange(() => Label);
            }
        }

        public string DisplayMemberPath
        {
            get { return _displayMemberPath; }
            set
            {
                _displayMemberPath = value;
                NotifyOfPropertyChange(() => DisplayMemberPath);
            }
        }

        public BindableCollection<object> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                NotifyOfPropertyChange(() => Items);
                SelectedItem = _items.FirstOrDefault();
            }
        }

        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }

        public ItemSelectorViewModel Start<T>(string label, string displayMemberPath,
                                              Func<OperationResult<IEnumerable<T>>> items)
        {
            Label = label;
            DisplayMemberPath = displayMemberPath;

            items().ContinueWith(op =>
                                     {
                                         if (op.CompletedSuccessfully)
                                             Items = new BindableCollection<object>(op.Result.Cast<object>());
                                     });

            return this;
        }
    }
}
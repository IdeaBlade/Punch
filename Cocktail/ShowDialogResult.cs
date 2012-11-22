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

namespace Cocktail
{
    /// <summary>
    /// Internal use.
    /// </summary>
    public class ShowDialogResult<T> : DialogOperationResult<T>
    {
        private readonly object _content;
        private readonly IEnumerable<IDialogUICommand<T>> _commands;
        private readonly PartLocator<DialogHostBase> _dialogHostLocator;
        private readonly string _title;
        private DialogHostBase _dialogHost;

        internal ShowDialogResult(object content, IEnumerable<IDialogUICommand<T>> commands, string title = null)
        {
            _title = title;
            _content = content;
            _commands = commands;
            _dialogHostLocator = new PartLocator<DialogHostBase>(CreationPolicy.NonShared)
                .WithDefaultGenerator(() => new DialogHostBase());
        }

            /// <summary>
        /// Internal use.
        /// </summary>
        [Import]
        public IWindowManager WindowManager { get; set; }

        /// <summary>
        /// Returns the user's response to a dialog or message box.
        /// </summary>
        public override T DialogResult
        {
            get
            {
                return _dialogHost == null || _dialogHost.DialogResult == null
                           ? default(T)
                           : (T) _dialogHost.DialogResult;
            }
        }

        /// <summary>Indicates whether the dialog or message box has been cancelled.</summary>
        /// <value>Cancelled is set to true, if the user clicked the designated cancel button in response to the dialog or message box.</value>
        public override bool Cancelled
        {
            get { return _dialogHost != null && _dialogHost.InvokedCommand != null && _dialogHost.InvokedCommand.IsCancelCommand; }
        }

        internal void Show()
        {
            _dialogHost = _dialogHostLocator.GetPart().Start(_title, _content, _commands.Cast<IUICommand>());
            _dialogHost.Completed += OnCompleted;
            WindowManager.ShowDialog(_dialogHost);
        }
    }
}
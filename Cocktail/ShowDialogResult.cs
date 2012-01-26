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
using Caliburn.Micro;

namespace Cocktail
{
    /// <summary>
    /// Internal use.
    /// </summary>
    public class ShowDialogResult<T> : DialogOperationResult<T>
    {
        private readonly T _cancelButton;
        private readonly object _content;
        private readonly IEnumerable<T> _dialogButtons;
        private readonly PartLocator<DialogHostBase> _dialogHostLocator;
        private readonly bool _hasCancelButton;
        private readonly string _title;
        private DialogHostBase _dialogHost;

        internal ShowDialogResult(object content, IEnumerable<T> dialogButtons, string title = null)
        {
            _title = title;
            _content = content;
            _dialogButtons = dialogButtons;
            _dialogHostLocator = new PartLocator<DialogHostBase>(CreationPolicy.NonShared)
                .WithDefaultGenerator(() => new DialogHostBase());
        }

        internal ShowDialogResult(object content, IEnumerable<T> dialogButtons, T cancelButton, string title = null)
            : this(content, dialogButtons, title)
        {
            _hasCancelButton = true;
            _cancelButton = cancelButton;
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
            get { return _dialogHost == null ? default(T) : (T)_dialogHost.DialogResult; }
        }

        /// <summary>Indicates whether the dialog or message box has been cancelled.</summary>
        /// <value>Cancelled is set to true, if the user clicked the designated cancel button in response to the dialog or message box.</value>
        public override bool Cancelled
        {
            get { return _hasCancelButton && DialogResult.Equals(_cancelButton); }
        }

        internal void Show()
        {
            _dialogHost = _dialogHostLocator.GetPart().Start(_title, _content, _dialogButtons);
            _dialogHost.Completed += OnCompleted;
            WindowManager.ShowDialog(_dialogHost);
        }
    }
}
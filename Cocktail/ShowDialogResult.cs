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
using Caliburn.Micro;

namespace Cocktail
{
    /// <summary>
    /// Internal use.
    /// </summary>
    public class ShowDialogResult : DialogOperationResult
    {
        private readonly object _content;
        private readonly DialogButtons _dialogButtons;
        private readonly string _title;
        private DialogHostBase _dialogHost;
        private readonly PartLocator<DialogHostBase> _dialogHostLocator;

        internal ShowDialogResult(object content, DialogButtons dialogButtons = DialogButtons.OkCancel,
                                  string title = null)
        {
            _title = title;
            _content = content;
            _dialogButtons = dialogButtons;
            _dialogHostLocator = new PartLocator<DialogHostBase>(CreationPolicy.NonShared)
                .WithDefaultGenerator(() => new DialogHostBase());
        }

        /// <summary>
        /// Internal use.
        /// </summary>
        [Import]
        public IWindowManager WindowManager { get; set; }

        /// <summary>
        /// Internal use.
        /// </summary>
        public override DialogResult DialogResult
        {
            get { return _dialogHost == null ? DialogResult.None : _dialogHost.DialogResult; }
        }

        internal void Show()
        {
            _dialogHost = _dialogHostLocator.GetPart().Start(_title, _content, _dialogButtons);
            _dialogHost.Completed += OnCompleted;
            WindowManager.ShowDialog(_dialogHost);
        }
    }
}
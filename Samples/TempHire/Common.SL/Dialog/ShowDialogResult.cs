using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Cocktail;

namespace Common.Dialog
{
    public class ShowDialogResult : IResult
    {
        private readonly object _content;
        private readonly bool _hideCancel;
        private readonly string _title;
        private ResultCompletionEventArgs _completionEventArgs;

        internal ShowDialogResult(object content, bool hideCancel = false, string title = null)
        {
            _title = title;
            _content = content;
            _hideCancel = hideCancel;

            IoC.BuildUp(this);
        }

        [Import]
        public IWindowManager WindowManager { get; set; }

        #region IResult Members

        void IResult.Execute(ActionExecutionContext context)
        {
        }

        event EventHandler<ResultCompletionEventArgs> IResult.Completed
        {
            add
            {
                if (_completionEventArgs != null)
                    value(this, _completionEventArgs);
                else
                    Completed += value;
            }
            remove { Completed -= value; }
        }

        #endregion

        public void Show()
        {
            DialogHostViewModel dialogHost = new DialogHostViewModel().Start(_title, _content, _hideCancel);
            dialogHost.Completed += OnCompleted;
            WindowManager.ShowDialog(dialogHost);
        }

        private void OnCompleted(object sender, ResultCompletionEventArgs e)
        {
            if (Completed != null)
                EventFns.RaiseOnce(ref Completed, this, e);
            _completionEventArgs = e;
        }

        private event EventHandler<ResultCompletionEventArgs> Completed;
    }
}
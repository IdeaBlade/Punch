using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Cocktail;

namespace Common.Dialog
{
    public class ShowDialogResult : IResult
    {
        private readonly object _content;
        private readonly string _title;
        private readonly bool _hideCancel;

        public ShowDialogResult(string title, object content, bool hideCancel = false)
        {
            _title = title;
            _content = content;
            _hideCancel = hideCancel;

            IoC.BuildUp(this);
        }

        [Import]
        public IWindowManager WindowManager { get; set; }

        #region IResult Members

        public void Execute(ActionExecutionContext context)
        {
            DialogHostViewModel dialogHost = new DialogHostViewModel().Start(_title, _content, _hideCancel);
            dialogHost.Completed += OnCompleted;
            WindowManager.ShowDialog(dialogHost);
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;

        #endregion

        private void OnCompleted(object sender, ResultCompletionEventArgs e)
        {
            if (Completed != null)
                EventFns.RaiseOnce(ref Completed, this, e);
        }
    }
}
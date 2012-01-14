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

        public ShowDialogResult(object content, bool hideCancel = false, string title = null)
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
            DialogHostViewModel dialogHost = new DialogHostViewModel().Start(_title, _content, _hideCancel);
            dialogHost.Completed += OnCompleted;
            WindowManager.ShowDialog(dialogHost);
        }

        event EventHandler<ResultCompletionEventArgs> IResult.Completed
        {
            add { Completed += value; }
            remove { Completed -= value; }
        }

        #endregion

        public void Show(Action<DialogResult> callback = null)
        {
            if (callback != null)
                this.Execute(args => callback(args.WasCancelled ? DialogResult.Cancel : DialogResult.Ok));
            else
                this.Execute();
        }

        private void OnCompleted(object sender, ResultCompletionEventArgs e)
        {
            if (Completed != null)
                EventFns.RaiseOnce(ref Completed, this, e);
        }

        private event EventHandler<ResultCompletionEventArgs> Completed;
    }
}
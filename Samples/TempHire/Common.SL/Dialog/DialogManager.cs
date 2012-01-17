using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Common.Dialog
{
    [Export(typeof (IDialogManager))]
    public class DialogManager : IDialogManager
    {
        #region IDialogManager Members

        public IResult ShowDialog(object content, bool hideCancel, Action<DialogResult> callback, string title)
        {
            var result = new ShowDialogResult(content, hideCancel, title);
            if (callback != null)
                ((IResult) result).Completed +=
                    (sender, args) => callback(args.WasCancelled ? DialogResult.Cancel : DialogResult.Ok);
            result.Show();
            return result;
        }

        public IResult ShowMessage(string message, bool hideCancel, Action<DialogResult> callback, string title)
        {
            var result = new ShowMessageResult(message, hideCancel, title);
            if (callback != null)
                ((IResult)result).Completed +=
                    (sender, args) => callback(args.WasCancelled ? DialogResult.Cancel : DialogResult.Ok);
            result.Show();
            return result;
        }

        #endregion
    }
}
using System;
using Caliburn.Micro;

namespace Common.Dialog
{
    public interface IDialogManager
    {
        IResult ShowDialog(object content, bool hideCancel = false, Action<DialogResult> callback = null,
                           string title = null);

        IResult ShowMessage(string message, bool hideCancel = true, Action<DialogResult> callback = null,
                            string title = null);
    }
}
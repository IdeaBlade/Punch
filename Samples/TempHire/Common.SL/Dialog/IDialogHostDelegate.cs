using System;

namespace Common.Dialog
{
    public enum DialogResult
    {
        Ok,
        Cancel
    };

    public interface IDialogHostDelegate
    {
        bool IsComplete { get; }
        DialogResult DialogResult { get; set; }

        event EventHandler CompleteChanged;
    }
}
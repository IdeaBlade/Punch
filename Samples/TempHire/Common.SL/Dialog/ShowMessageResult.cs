namespace Common.Dialog
{
    public class ShowMessageResult : ShowDialogResult
    {
        public ShowMessageResult(string title, string message, bool hideCancel = true)
            : base(title, new MessageViewModel(message), hideCancel)
        {
        }
    }
}
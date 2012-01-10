using Caliburn.Micro;

namespace Common.Dialog
{
    public class MessageViewModel : Screen
    {
        private readonly string _message;

        public MessageViewModel(string message)
        {
            _message = message;
        }

        public string Message
        {
            get { return _message; }
        }
    }
}
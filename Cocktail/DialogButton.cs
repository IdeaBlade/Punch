using Caliburn.Micro;

namespace Cocktail
{
    /// <summary>Logical definition of a dialog or message box button.</summary>
    public class DialogButton : PropertyChangedBase
    {
        private bool _enabled;

        /// <summary>Initializes a new instance of DialogButton.</summary>
        /// <param name="value">The user response value associated with this button.</param>
        public DialogButton(object value)
        {
            Value = value;
            Enabled = true;
        }

        internal object Value { get; private set; }

        /// <summary>The button content displayed in the view.</summary>
        public object Content
        {
            get
            {
                if (Value is DialogResult)
                    return StringResources.ResourceManager.GetString(Value.ToString());
                return Value;
            }
        }

        /// <summary>Indicates whether the button is currently enabled or disabled.</summary>
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                NotifyOfPropertyChange(() => Enabled);
            }
        }
    }
}
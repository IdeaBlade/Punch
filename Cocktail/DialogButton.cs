//====================================================================================================================
// Copyright (c) 2012 IdeaBlade
//====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
// USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
// http://cocktail.ideablade.com/licensing
//====================================================================================================================

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
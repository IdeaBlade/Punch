// ====================================================================================================================
//  Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//  WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//  OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//  OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//  USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//  http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Common.Behaviors
{
    [TypeConstraint(typeof (PasswordBox))]
    public class BindablePasswordBehavior : Behavior<PasswordBox>
    {
        public static DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof (string), typeof (BindablePasswordBehavior),
                                        new PropertyMetadata(OnPasswordChanged));

        public string Password
        {
            get { return (string) GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        private static void OnPasswordChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var behavior = (BindablePasswordBehavior) sender;
            PasswordBox associatedObject = behavior.AssociatedObject;

            if (associatedObject.Password == behavior.Password)
                return;

            associatedObject.Password = behavior.Password;
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += AssociatedObjectOnPasswordChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PasswordChanged -= AssociatedObjectOnPasswordChanged;
        }

        private void AssociatedObjectOnPasswordChanged(object sender, RoutedEventArgs routedEventArgs)
        {
            if (AssociatedObject.Password == Password)
                return;

            Password = AssociatedObject.Password;
        }
    }
}
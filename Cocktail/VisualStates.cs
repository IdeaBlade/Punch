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

using System;
using System.Windows;
using System.Windows.Controls;

namespace Cocktail
{
    /// <summary>Maintains a <see cref="VisualStateManager"/>'s 
    /// current state via a binding to the "CurrentState" attached property</summary>
    /// <remarks>Thanks to Alex van Beek for original "VisualStates" version at
    /// from http://blogs.infosupport.com/silverlight-4-using-the-visualstatemanager-for-state-animations-with-mvvm/
    /// <para>
    /// In the example, the Grid's VSM state is maintained by the "MoodState" property
    /// of the data bound source, typically a ViewModel.
    /// </para></remarks>
    /// <example>
    /// 	<code title="Example" description="" lang="XAML">
    /// &lt;Grid cocktail:VisualStates.CurrentState="{Binding MoodState}" /&gt;</code>
    /// </example>
    public static class VisualStates
    {
        /// <summary>Hold the intended current state of a control's <see cref="VisualStateManager"/></summary>
        public static readonly DependencyProperty CurrentStateProperty =
            DependencyProperty.RegisterAttached(
                "CurrentState",
                typeof(String),
                typeof(VisualStates),
                new PropertyMetadata(TransitionToState));

        /// <summary>Get the intended current state of a control's <see cref="VisualStateManager"/></summary>
        public static string GetCurrentState(DependencyObject obj)
        {
            return (string) obj.GetValue(CurrentStateProperty);
        }

        /// <summary>Set the intended current state of a control's <see cref="VisualStateManager"/></summary>
        public static void SetCurrentState(DependencyObject obj, string value)
        {
            obj.SetValue(CurrentStateProperty, value);
        }

        private static void TransitionToState(object sender, DependencyPropertyChangedEventArgs args)
        {
            var c = sender as Control;
            if (c != null)
            {
                VisualStateManager.GoToState(
                    c, ((string) args.NewValue) ?? string.Empty, true);
            }
            else
            {
                throw new ArgumentException(StringResources.CurrentStateOnlySupportedInControls);
            }
        }
    }
}
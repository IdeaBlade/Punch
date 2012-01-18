using System.Windows;
using System.Windows.Controls;

namespace HappyHour.Controls
{
    public partial class InebriationIndicator : UserControl
    {
        public InebriationIndicator()
        {
            InitializeComponent();
        }

        //public string InebriationState
        //{
        //    get { return (string)GetValue(InebriationStateProperty); }
        //    set { SetValue(InebriationStateProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for InebriationState.  
        //// enabling animation, styling, binding, etc...
        //public static readonly DependencyProperty InebriationStateProperty =
        //    DependencyProperty.Register("InebriationState",                               
        //    typeof(string),
        //    typeof(InebriationIndicator),
        //    new PropertyMetadata("Sober",                                 
        //        OnInebriationStateChanged));

        //static void OnInebriationStateChanged(object sender,                              
        //    DependencyPropertyChangedEventArgs args)
        //{  
        //    // Get reference to self  
        //    var source = (InebriationIndicator)sender;   
        //    // Handling the new value  
        //    var newState = (string)args.NewValue ?? "Sober";
        //    VisualStateManager.GoToState(source, newState, true);
        //} 
    }
}

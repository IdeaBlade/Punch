using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using HappyHour.ViewModels;

namespace HappyHour.Views
{
    public partial class MainPage : UserControl
    {
        private readonly MainPageViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            DataContext = _viewModel = new MainPageViewModel();
        }

        // Button click handler; not used when binding to button's Command property
        private void AskForIt_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AskForIt();
        }

        private void StartAnimation_Click(object sender, RoutedEventArgs e)
        {
            var sb = ((Storyboard) Resources["AnimateTarget"]);
            sb.Begin();
        } 
    }
}

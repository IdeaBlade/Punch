using System.Windows;

namespace TempHire.Views.Resource
{
    public partial class ResourceNameEditorView
    {
        public ResourceNameEditorView()
        {
            InitializeComponent();
        }

        private void FirstNameGotFocus(object sender, RoutedEventArgs e)
        {
            FirstName.SelectAll();
        }

        private void MiddleNameGotFocus(object sender, RoutedEventArgs e)
        {
            MiddleName.SelectAll();
        }

        private void LastNameGotFocus(object sender, RoutedEventArgs e)
        {
            LastName.SelectAll();
        }
    }
}
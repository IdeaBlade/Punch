using System.Windows;

namespace TempHire.Views.StaffingResource
{
    public partial class StaffingResourceNameEditorView
    {
        public StaffingResourceNameEditorView()
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
using System.Windows;
using HappyHour.ViewModels;

namespace HappyHour.Views
{
    public partial class MainPage : IMainPage
    {
        void IMainPage.ReadyForNewDrink()
        {
            Beverages.Focus();
        }
    }
}

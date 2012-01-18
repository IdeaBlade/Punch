using HappyHour.ViewModels;

namespace HappyHour.Views
{
    public partial class MainPage : IMainPage
    {
        void IMainPage.ReadyForNewDrink()
        {
            DrinkName.Text = string.Empty;
            DrinkName.Focus();
        }
    }
}

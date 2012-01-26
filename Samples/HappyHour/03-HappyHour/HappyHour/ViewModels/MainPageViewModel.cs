using System;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;

namespace HappyHour.ViewModels
{
    [Export]
    public class MainPageViewModel : Screen
    {
        private string _drinkName;
        public string DrinkName
        {
            get { return _drinkName; }
            set
            {
                _drinkName = value;
                NotifyOfPropertyChange("CanAskForIt");
            }
        }

        public bool CanAskForIt
        {
            get { return !String.IsNullOrWhiteSpace(DrinkName); }
        }

        public void AskForIt()
        {
            MessageBox.Show(string.Format("One {0}, coming right up!", DrinkName)); // don't do this in real app
        }
    }
}

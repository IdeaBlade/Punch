using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace HappyHour.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private string _drinkName;
        public string DrinkName
        {
            get { return _drinkName; }
            set
            {
                _drinkName = value;
                NotifyOfPropertyChange("CanAskForIt");
                AskForItCommand.RaiseCanExecuteChanged(); // When binding to AskForItCommand
            }
        }

        public bool CanAskForIt
        {
            get { return !String.IsNullOrWhiteSpace(DrinkName); }
        }

        public void AskForIt()
        {
            MessageBox.Show(
                string.Format(CultureInfo.CurrentCulture, 
                "One {0}, coming right up!", DrinkName)); // don't do this in real app
        }

        public Command AskForItCommand
        {
            get
            {
                if (null == _askForItCommand) _askForItCommand =
                    new Command(AskForIt, _ => CanAskForIt);
                return _askForItCommand;
            }
        }

        private Command _askForItCommand;

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyOfPropertyChange(string propertyName)
        {
            var handlers = PropertyChanged;
            if (null == handlers) return; // no one is listening
            handlers(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

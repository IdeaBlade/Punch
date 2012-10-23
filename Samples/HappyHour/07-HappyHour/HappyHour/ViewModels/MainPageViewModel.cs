using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Model;

namespace HappyHour.ViewModels
{
    [Export]
    public class MainPageViewModel : Screen
    {
        public MainPageViewModel()
        {
            DrinkOrders = new BindableCollection<DrinkOrder>();
        }

        public BindableCollection<DrinkOrder> DrinkOrders { get; set; }

        private string _drinkName;
        public string DrinkName
        {
            get { return _drinkName; }
            set
            {
                _drinkName = value;
                NotifyOfPropertyChange("CanAddDrinkOrder");
            }
        }

        private DrinkOrder _selectedDrinkOrder;
        public DrinkOrder SelectedDrinkOrder
        {
            get { return _selectedDrinkOrder; }
            set 
            {
                _selectedDrinkOrder = value;
                NotifyOfPropertyChange("SelectedDrinkOrder");
            }
        }

        public bool CanAddDrinkOrder
        {
            get { return !String.IsNullOrWhiteSpace(DrinkName); }
        }

        public void AddDrinkOrder()
        {
            var drink = new DrinkOrder {DrinkName = DrinkName};
            DrinkOrders.Add(drink);
            SelectedDrinkOrder = drink;
            ReadyForNewDrink();
        }

        private void ReadyForNewDrink()
        {
            if (null != _view) _view.ReadyForNewDrink();
        }

        protected override void OnViewAttached(object view, object context)
        {
            _view = view as IMainPage; 
        }

        private IMainPage _view;


    }
}

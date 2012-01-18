using System;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Model;

namespace HappyHour.ViewModels
{
    [Export]
    public class MainPageViewModel : Screen
    {
        public MainPageViewModel()
        {
            Repository = new BeverageRepository();
            DrinkOrders = new BindableCollection<DrinkOrder>();
            Beverages = new BindableCollection<Beverage>();
            PlaceHolderBeverage = new Beverage
                                      {
                                          BeverageName = "Please select a beverage",
                                          ImageFilename = "select_drink.png"
                                      };        
            Beverages.Add(PlaceHolderBeverage);
            SelectedBeverage = PlaceHolderBeverage;
        }

        private IBeverageRepository Repository { get; set; }
        public BindableCollection<DrinkOrder> DrinkOrders { get; private set; }
        public BindableCollection<Beverage> Beverages { get; private set; }
        private Beverage PlaceHolderBeverage { get; set; }

        protected override void OnInitialize()
        {
            Beverages.AddRange(Repository.FindAll()); // ToDo: Async version
            ReadyForNewDrink();
        }

        private Beverage _selectedBeverage;
        public Beverage SelectedBeverage
        {
            get { return _selectedBeverage; }
            set
            {
                if (_selectedBeverage == value) return;// short circuit if no change
                _selectedBeverage = value;
                NotifyOfPropertyChange("SelectedBeverage");
                NotifyOfPropertyChange(() => CanAddDrinkOrder);
                AddDrinkOrder();
            }
        }

        private DrinkOrder _selectedDrinkOrder;
        public DrinkOrder SelectedDrinkOrder
        {
            get { return _selectedDrinkOrder; }
            set 
            {
                if (_selectedDrinkOrder == value) return; // short circuit if no change
                _selectedDrinkOrder = value;

                // Ensure SelectedBeverage is same as beverage of the newly selected DrinkOrder
                if (null != _selectedDrinkOrder)
                {
                    var bev = _selectedDrinkOrder.Beverage;
                    SelectedBeverage = Beverages.Contains(bev) ? bev : PlaceHolderBeverage;
                }
                NotifyOfPropertyChange("SelectedDrinkOrder");
            }
        }

        public bool CanAddDrinkOrder
        {
            get { return SelectedBeverage != PlaceHolderBeverage; }
        }

        public void AddDrinkOrder()
        {
            if (!CanAddDrinkOrder) return; // for safety
            if (DrinkOrders.Count == 10)
            {
                DrinkOrders.Clear(); // Reset at 10 drinks of any kind
            }
            var drink = new DrinkOrder(SelectedBeverage);
            DrinkOrders.Add(drink);
            SelectedDrinkOrder = drink;
            SetInebriationState();
            ReadyForNewDrink();
        }
        
        private string _inebriationState;
        public string InebriationState
        {
            get { return _inebriationState; }
            set
            {
                if (_inebriationState == value) return;
                _inebriationState = value;
                NotifyOfPropertyChange("InebriationState");
            }
        }

        private void SetInebriationState()
        {
            var state = InebriationStates.Sober;

            var alkyDrinkCount = DrinkOrders.Count(d => d.HasAlcohol);
            if (alkyDrinkCount > 5) state = InebriationStates.Drunk;
            else if (alkyDrinkCount > 3) state = InebriationStates.Woozy;
            else if (alkyDrinkCount > 1) state = InebriationStates.Tipsy;

            InebriationState = state;
        }

        private void ReadyForNewDrink()
        {
            if (null != _view) _view.ReadyForNewDrink(); // Throw if null?
        }

        protected override void OnViewAttached(object view, object context)
        {
            _view = view as IMainPage; // Throw if not IMainPage?
        }

        private IMainPage _view;        
    }
    
    public static class InebriationStates
    {
        public const string Sober = "Sober";
        public const string Tipsy = "Tipsy";
        public const string Woozy = "Woozy";
        public const string Drunk = "Drunk";
    }
}

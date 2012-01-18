namespace HappyHour
{
    public class AppBootstrapper : Cocktail.FrameworkBootstrapper<ViewModels.MainPageViewModel>
    {        
        // Needed if the ListBox displays DrinkOrderEntityViews
        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            Caliburn.Micro.ViewLocator.NameTransformer
                // Map "Model" classes to views in "HappyHour.Views"
                .AddRule(@"^Model\.(\w*)", "HappyHour.Views.${1}EntityView");

            base.OnStartup(sender, e);
        }
    }
}

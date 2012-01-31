namespace HappyHour
{
    public class AppBootstrapper : Cocktail.FrameworkBootstrapper<ViewModels.MainPageViewModel>
    {       
        // Needed if the ListBox displays DrinkOrderEntityViews
        protected override void Configure()
        {
            base.Configure();
            Caliburn.Micro.ViewLocator.NameTransformer
                // Map "Model" classes to views in "HappyHour.Views"
                .AddRule(@"^Model\.(\w*)", "HappyHour.Views.${1}EntityView");
        }
    }
}

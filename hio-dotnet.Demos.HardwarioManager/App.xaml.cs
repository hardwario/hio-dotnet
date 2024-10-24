namespace hio_dotnet.Demos.HardwarioManager
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}

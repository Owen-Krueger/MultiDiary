namespace MultiDiary;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new MainPage();
	}

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);

        window.MinimumWidth= 800;
        window.MinimumHeight= 600;

        return window;
    }
}

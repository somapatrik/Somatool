using System.Runtime.CompilerServices;

namespace Veverka.Views;

public partial class DevelopmentPage : ContentPage
{
	public DevelopmentPage()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
        openlink();
    }

    private async void openlink()
    {
        try 
        { 
        Uri uri = new Uri("https://github.com/somapatrik/somatool-public/issues");
        await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch { }
    }
}
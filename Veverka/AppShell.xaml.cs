using Veverka.Services;

namespace Veverka;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		LoadDB();
    }

	public async void LoadDB()
	{
		await DBV.InitDB();
	}

}

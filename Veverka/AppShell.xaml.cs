using Veverka.Services;

namespace Veverka;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		//InitDB();
    }

	public async void InitDB()
	{
		await DBV.InitDB();
	}
}

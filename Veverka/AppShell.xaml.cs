using Veverka.Services;

namespace Veverka;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        DBV.InitDB();
    }
}

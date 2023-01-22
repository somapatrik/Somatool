using Veverka.ViewModels;

namespace Veverka;

public partial class MainPage : ContentPage
{
	MainViewModel _viewModel;

	public MainPage()
	{
		InitializeComponent();
		BindingContext = _viewModel = new MainViewModel();
	}
}


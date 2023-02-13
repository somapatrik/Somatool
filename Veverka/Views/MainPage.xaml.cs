using Veverka.ViewModels;

namespace Veverka.Views;

public partial class MainPage : ContentPage
{
	MainViewModel _viewModel;

	public MainPage()
	{
		InitializeComponent();
		BindingContext = _viewModel = new MainViewModel();
	}
}


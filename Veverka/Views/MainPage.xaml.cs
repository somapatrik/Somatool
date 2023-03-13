using Veverka.Services;
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

    protected override void OnAppearing()
    {
        base.OnAppearing();
		_viewModel.OnAppearing();
    }
}


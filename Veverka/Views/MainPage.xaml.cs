using Veverka.Services;
using Veverka.ViewModels;

namespace Veverka.Views;

public partial class MainPage : ContentPage
{
	MainViewModel _viewModel;

	public MainPage()
	{
		InitializeComponent();
		InitDB();
		BindingContext = _viewModel = new MainViewModel();
	}

	private async void InitDB()
	{
		await DBV.InitDB();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		_viewModel.OnAppearing();
    }
}


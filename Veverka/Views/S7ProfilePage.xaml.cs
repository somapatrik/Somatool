using Veverka.Models;
using Veverka.ViewModels;

namespace Veverka.Views;

public partial class S7ProfilePage : ContentPage
{
	S7ProfileViewModel viewModel;
	public S7ProfilePage(S7Plc plc)
	{
		InitializeComponent();
		BindingContext = viewModel = new S7ProfileViewModel(plc);
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		viewModel.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        viewModel.OnDisappearing();
    }

    private void SwipeItem_Clicked(object sender, EventArgs e)
    {
        
        viewModel.DeleteSignalHandler (sender);
    }
}
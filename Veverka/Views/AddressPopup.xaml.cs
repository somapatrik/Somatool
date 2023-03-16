using CommunityToolkit.Maui.Views;
using Veverka.ViewModels;

namespace Veverka.Views;

public partial class AddressPopup : Popup
{

	AddressPopupViewModel _viewModel;
	public AddressPopup()
	{
		InitializeComponent();
		BindingContext = _viewModel = new AddressPopupViewModel();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
		Close(_viewModel);
    }
}
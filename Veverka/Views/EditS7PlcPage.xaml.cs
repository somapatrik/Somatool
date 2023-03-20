using System.Collections.ObjectModel;
using Veverka.Models;
using Veverka.ViewModels;

namespace Veverka.Views;

public partial class EditS7PlcPage : ContentPage
{
    EditS7PlcViewModel _viewModel;

	public EditS7PlcPage()
	{
		InitializeComponent();
		BindingContext = _viewModel = new EditS7PlcViewModel();
	}

	public void SetEdit(S7Plc plc)
	{
		_viewModel.Edit = true;
		_viewModel.Plc = plc;
	}

}
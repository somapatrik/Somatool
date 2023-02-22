using System.Collections.ObjectModel;
using Veverka.ViewModels;

namespace Veverka.Views;

public partial class EditS7PlcPage : ContentPage
{
	public EditS7PlcPage()
	{
		InitializeComponent();
		BindingContext = new EditS7PlcViewModel();

	}
}
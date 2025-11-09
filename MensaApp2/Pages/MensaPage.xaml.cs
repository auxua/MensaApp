namespace MensaApp2.Pages;

public partial class MensaPage : ContentPage
{
	public MensaPage()
	{
		InitializeComponent();
	}

	private async void ToolbarItem_Clicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("config");
	}
}
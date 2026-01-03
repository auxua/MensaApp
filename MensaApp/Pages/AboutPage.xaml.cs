
namespace MensaApp.Pages
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage(AboutPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }

    }
}
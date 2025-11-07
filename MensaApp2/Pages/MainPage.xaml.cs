using MensaApp2.Models;
using MensaApp2.PageModels;

namespace MensaApp2.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}
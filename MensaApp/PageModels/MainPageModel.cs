using CommunityToolkit.Maui.PlatformConfiguration.AndroidSpecific;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MensaApp.Resources.Strings;
using MensaPortable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace MensaApp.PageModels
{
    public partial class MainPageModel : ObservableObject
    {

        [ObservableProperty]
        bool _isBusy;

        [ObservableProperty]
        bool _isRefreshing;

        [ObservableProperty]
        private ObservableCollection<Dish> _currentDishes = new();

        [ObservableProperty]
        private string _currentMensa = "Academica";

        [ObservableProperty]
        private Command _nextMensaCommand;

        [ObservableProperty]
        private Command _previousMensaCommand;

        [ObservableProperty]
        private Command _nextDateCommand;

        [ObservableProperty]
        private Command _previousDateCommand;

        [ObservableProperty]
        private Command _selectMensaCommand;

        [ObservableProperty]
        private DateTime _date = DateTime.Now.Date;


        private MensaAdapter mensaAdapter;

        private IDialogService dialog;

        public MainPageModel(IDialogService dialog, MensaAdapter mensaAdapter)
        {
            this.dialog = dialog;
            this.mensaAdapter = mensaAdapter;

            //LoadData();

            _nextMensaCommand = new Command(async () =>
            {
                //TODO: Next Mensa
                var next = mensaAdapter.getNextMensaName(this.CurrentMensa);
                //dialog.ShowSnackInfoAsync("Next Mensa: "+next);
                await MainThread.InvokeOnMainThreadAsync(() => CurrentMensa = next);
                this.RefreshData();
                await Task.Delay(1); // One Dispatch Loop
            });

            _previousMensaCommand = new Command(async () =>
            {
                // TODO: Previous Mensa
                var prev = mensaAdapter.getPreviousMensaName(this.CurrentMensa);
                //dialog.ShowSnackInfoAsync("Previous Mensa: "+prev);
                await MainThread.InvokeOnMainThreadAsync(() => CurrentMensa = prev);
                this.RefreshData();
            });

            _nextDateCommand = new Command(async () =>
            {
                //TODO
                var next = MenuDB.Instance.getNextAvailableDay(this.Date);
                //dialog.ShowSnackInfoAsync("Next Day: " + next);
                await MainThread.InvokeOnMainThreadAsync(() => this.Date = next);
                this.RefreshData();
            });

            _previousDateCommand = new Command(async () =>
            {
                //TODO
                var next = MenuDB.Instance.getPreviousAvailableDay(this.Date);
                //dialog.ShowSnackInfoAsync("Previous Day: " + next);
                await MainThread.InvokeOnMainThreadAsync(() => this.Date = next);
                this.RefreshData();
            });

            _selectMensaCommand = new Command(async () =>
            {
                //var mensen = mensaAdapter.Mensen.Keys.ToArray();
                var mensen = mensaAdapter.GetActiveMensen();
                var sel = await dialog.ShowSelectionAsync("Mensa Select", mensen.ToArray());
                if (sel == "Cancel") return; // No Selection

                await MainThread.InvokeOnMainThreadAsync(() => CurrentMensa = sel);
                this.RefreshData();
            });


        }

        private void RefreshData()
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            MenuDB.QueryBuilder qb;
            if (App.getConfig(App.Config_OnlyMainDishes))
                qb = new MenuDB.QueryBuilder().ByMensa(this.CurrentMensa).ByDate(this.Date).NoSideDishes();
            else
                qb = new MenuDB.QueryBuilder().ByMensa(this.CurrentMensa).ByDate(this.Date);

            var dishes = qb.ExecuteQuery();
            if (dishes.Count < 1)
                dishes.Add(FallbackDish);
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                this.CurrentDishes.Clear();
                foreach (var item in dishes) this.CurrentDishes.Add(item);
            });
            //this.CurrentDishes = dishes.ToList();
            //dialog.ShowSnackInfoAsync("updated");
            //sw.Stop();
            //dialog.ShowSnackInfoAsync("Time needed: " + sw.ElapsedMilliseconds);


        }


        private async Task LoadData()
        {
            try
            {
                //IsBusy = true;
                IsRefreshing = true;

                if (MenuDB.Instance.IsOutdated())
                {
                    await mensaAdapter.CatchMensaDataAsync();
                }
                
                DateTime next = MenuDB.Instance.getNextAvailableDay(this.Date);
                await MainThread.InvokeOnMainThreadAsync(() => this.Date = next );

                MenuDB.QueryBuilder qb;

                if (App.getConfig(App.Config_OnlyMainDishes))
                    qb = new MenuDB.QueryBuilder().ByMensa(this.CurrentMensa).ByDate(this.Date).NoSideDishes();
                else
                    qb = new MenuDB.QueryBuilder().ByMensa(this.CurrentMensa).ByDate(this.Date);
                
                var dishes = qb.ExecuteQuery();

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    this.CurrentDishes.Clear();
                    foreach (var item in dishes) this.CurrentDishes.Add(item);
                });
            }
            finally
            {
                //IsBusy = false;
                IsRefreshing = false;
            }
        }

        private List<Dish> CreateDummyDishes()
        {
            return new List<Dish>()
            {
                new Dish()
                {
                    Name = "Spaghetti Bolognese",
                    Price = "2.50Ä",
                    Kind = "Tellergericht",
                    Date = DateTime.Now,
                    Mensa = "Ahornstraﬂe",
                    Special = null,
                    Popup = null,
                    NutritionInfo = null,
                    Tags = new List<DishTag>() { new DishTag() { Name="Rind" }, new DishTag { Name = "Schwein" } }
                },
                new Dish()
                {
                    Name = "Vegetarische Lasagne",
                    Price = "2.00Ä",
                    Kind = "Tellergericht",
                    Date = DateTime.Now,
                    Mensa = "Ahornstraﬂe",
                    Special = null,
                    Popup = null,
                    NutritionInfo = null,
                    Tags = new List<DishTag>() { new DishTag() { Name="Vegetarisch" } }
                },
                new Dish()
                {
                    Name = "SChnitzel",
                    Price = "2.00Ä",
                    Kind = "Schnitzelgericht",
                    Date = DateTime.Now,
                    Mensa = "Ahornstraﬂe",
                    Special = null,
                    Popup = null,
                    NutritionInfo = null,
                    Tags = new List<DishTag>() { new DishTag() { Name = "Schwein" } }
                },
                new Dish()
                {
                    Name = "Gemischter Salat",
                    Price = null,
                    Kind = "Hauptbeilage",
                    Date = DateTime.Now,
                    Mensa = "Ahornstraﬂe",
                    Special = null,
                    Popup = null,
                    NutritionInfo = null,
                 },
                new Dish()
                {
                    Name = "SChnitzel ¸berbacken",
                    Price = "2.00Ä",
                    Kind = "Schnitzelgericht",
                    Date = DateTime.Now.AddDays(1),
                    Mensa = "Ahornstraﬂe",
                    Special = null,
                    Popup = null,
                    NutritionInfo = null,
                    Tags = new List<DishTag>() { new DishTag() { Name = "Schwein" } }
                },
                new Dish()
                {
                    Name = "Gemischter Salat",
                    Price = null,
                    Kind = "Hauptbeilage",
                    Date = DateTime.Now.AddDays(1),
                    Mensa = "Ahornstraﬂe",
                    Special = null,
                    Popup = null,
                    NutritionInfo = null,
                 }
            };

        }

        private Dish FallbackDish =
            new Dish()
            {
                Name = AppRessources.FallbackDishName,
                Kind = "Info",
            };

        

        [RelayCommand]
        private async Task Appearing()
        {
            LoadData();
            if (MenuDB.Instance.IsOutdated())
                dialog.ShowSnackInfoAsync(AppRessources.OutdatedLoading,10);
        }



    }
}
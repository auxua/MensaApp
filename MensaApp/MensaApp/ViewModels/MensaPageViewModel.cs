using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using System.Threading;
using Xamarin.Forms;
using System.Globalization;

namespace MensaApp.ViewModels
{
    class MensaPageViewModel : INotifyPropertyChanged
    {
        #region properties

        //private INavigation navigation;

        private bool isBusy;

        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }
            set
            {
                this.isBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }

        private bool isCreated;

        public bool IsCreated
        {
            get
            {
                return this.isCreated;
            }
            set
            {
                this.isCreated = value;
                RaisePropertyChanged("IsCreated");
                /*if (value)
                {
                    this.LoadAllDataCommand.Execute(null);
                }*/
            }
        }

        private bool needsUpdate;

        public bool NeedsUpdate
        {
            get
            {
                return this.needsUpdate;
            }
            set
            {
                this.needsUpdate = value;
                RaisePropertyChanged("NeedsUpdate");
                if (value)
                {
                    this.LoadAllDataCommand.Execute(null);
                }
                this.needsUpdate = false;
            }
        }

        private string mensaName;

        public string MensaName
        {
            get
            {
                return this.mensaName;
            }
            set
            {
                this.mensaName = value;
                RaisePropertyChanged("MensaName");
            }
        }

        private bool hasData;

        public bool HasData
        {
            get
            {
                return this.hasData;
            }
            set
            {
                this.hasData = value;
                RaisePropertyChanged("HasData");
            }
        }

        private bool dataAvailable;

        public bool DataAvailable
        {
            get
            {
                return this.dataAvailable;
            }
            set
            {
                this.dataAvailable = value;
                RaisePropertyChanged("DataAvailable");
                if (value)
                {
                    this.GetAllDataCommand.Execute(null);
                }
            }
        }

        private string status;

        public string Status
        {
            get
            {
                return this.status;
            }
            set
            {
                this.status = value;
                RaisePropertyChanged("Status");
            }
        }

        private ObservableCollection<Mensa.DataTypes.Dish> items;

        public ObservableCollection<Mensa.DataTypes.Dish> Items
        {
            get
            {
                return this.items;
            }
            // should never be set - should use the Collection itself for changes
            /*set
            {
                this.status = value;
                RaisePropertyChanged("Status");
            }*/
        }

        private DateTime date;

        public DateTime Date
        {
            get
            {
                return this.date;
            }
            set
            {
                this.date = value;
                RaisePropertyChanged("Date");
                var culture = new CultureInfo(Localization.Locale());
                var day = culture.DateTimeFormat.GetDayName(value.Date.DayOfWeek);
                this.DateAsString = day + "\n" + value.Date.ToShortDateString();
            }
        }

        private string dateAsString;

        public string DateAsString
        {
            get
            {
                return this.dateAsString;
            }
            set
            {
                this.dateAsString = value;
                RaisePropertyChanged("DateAsString");
            }
        }

        private string errorMessage;

        public string ErrorMessage
        {
            get
            {
                return this.errorMessage;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.errorMessage = "";
                }
                else
                {
                    this.errorMessage = string.Format("An error occured: {0}", value);
                }
                RaisePropertyChanged("ErrorMessage");
                RaisePropertyChanged("HasErrorMessage");
            }
        }

        public bool HasErrorMessage
        {
            get
            {
                return !string.IsNullOrEmpty(ErrorMessage);
            }
        }

        #endregion

        #region commands

        private ICommand loadAllDataCommand;

        public ICommand LoadAllDataCommand
        {
            get
            {
                return this.loadAllDataCommand;
            }
        }

        private ICommand getAllDataCommand;

        public ICommand GetAllDataCommand
        {
            get
            {
                return this.getAllDataCommand;
            }
        }

        private ICommand getNextDayCommand;

        public ICommand GetNextDayCommand
        {
            get
            {
                return this.getNextDayCommand;
            }
        }

        private ICommand getPrevDayCommand;

        public ICommand GetPrevDayCommand
        {
            get
            {
                return this.getPrevDayCommand;
            }
        }

        private ICommand getPrevMensaCommand;

        public ICommand GetPrevMensaCommand
        {
            get
            {
                return this.getPrevMensaCommand;
            }
        }

        private ICommand getNextMensaCommand;

        public ICommand GetNextMensaCommand
        {
            get
            {
                return this.getNextMensaCommand;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        #endregion

        #region Localization Strings

        private string dishesString;

        public string DishesString
        {
            get
            {
                return this.dishesString;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.dishesString = "";
                }
                else
                {
                    this.dishesString = value;
                }
                RaisePropertyChanged("DishesString");
            }
        }

        private string nextDayString;

        public string NextDayString
        {
            get
            {
                return this.nextDayString;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.nextDayString = ">";
                }
                else
                {
                    this.nextDayString = value;
                }
                RaisePropertyChanged("NextDayString");
            }
        }

        private string previousDayString;

        public string PreviousDayString
        {
            get
            {
                return this.previousDayString;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.previousDayString = "<";
                }
                else
                {
                    this.previousDayString = value;
                }
                RaisePropertyChanged("PreviousDayString");
            }
        }

        private string previousMensaString;

        public string PreviousMensaString
        {
            get
            {
                return this.previousMensaString;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.previousMensaString = "<";
                }
                else
                {
                    this.previousMensaString = value;
                }
                RaisePropertyChanged("PreviousMensaString");
            }
        }

        private string nextMensaString;

        public string NextMensaString
        {
            get
            {
                return this.nextMensaString;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.nextMensaString = ">";
                }
                else
                {
                    this.nextMensaString = value;
                }
                RaisePropertyChanged("NextMensaString");
            }
        }

        private string configString;

        public string ConfigString
        {
            get
            {
                return this.configString;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.configString = "Config";
                }
                else
                {
                    this.configString = value;
                }
                RaisePropertyChanged("ConfigString");
            }
        }

        private void getLocalizedStrings()
        {
            //this.DishesString = "DishTest";
            this.DishesString = Localization.Localize("DishesString");
            //this.PreviousDayString = Localization.Localize("PreviousDayString");
            //this.NextDayString = Localization.Localize("NextDayString");

            this.PreviousDayString = "<";
            this.NextDayString = ">";
            this.PreviousMensaString = "<";
            this.NextMensaString = ">";

            this.ConfigString = Localization.Localize("ConfigString");
        }

        #endregion

        public MensaPageViewModel(string MensaName, DateTime dt)
        {
            this.MensaName = MensaName;
            this.Date = dt.Date;
            this.Status = "Creating Page";
            this.getLocalizedStrings();
            this.items = new ObservableCollection<Mensa.DataTypes.Dish>();
            this.IsBusy = false;

            // Trigger the MensaDB to get the Mensa Data
            this.loadAllDataCommand = new Command(async () => 
                {
                    IsBusy = true;
                    this.Status = "Get Data";
                    bool done = true;
                    if (Mensa.MenuDB.Instance.isOutdated())
                    {
                        done = await MensaAdapter.CatchMensaDataAsync();
                    }
                    if (!done)
                    {
                        this.ErrorMessage = "Could not Load Mensa Data";
                    }
                    else
                    {
                        this.DataAvailable = true;
                    }
                    IsBusy = false;
                });

            // query the Mensa Data from the MensaDB
            this.getAllDataCommand = new Command(async () =>
            {
                IsBusy = true;
                Status = "Populating Data";

                var query = new Mensa.MenuDB.QueryBuilder().ByDate(this.Date).ByMensa(this.MensaName);

                if (App.getConfig("VegieOnly"))
                    query = query.ByKind("Vegetarisch");

                if (App.getConfig("MainDishesOnly"))
                    query = query.NoSideDishes();

                var queryData = query.ExecuteQuery();
                
                this.Items.Clear();
                foreach (var dish in queryData)
                {
                    this.Items.Add(dish);
                }
                this.HasData = true;
                IsBusy = false;
            });

            this.getNextDayCommand = new Command(async () =>
            {
                IsBusy = true;
                DateTime next = Mensa.MenuDB.Instance.getNextAvailableDay(this.Date);
                this.Date = next;
                this.NeedsUpdate = true;
                IsBusy = false;
            });

            this.getPrevDayCommand = new Command(async () =>
            {
                IsBusy = true;
                DateTime next = Mensa.MenuDB.Instance.getPreviousAvailableDay(this.Date);
                this.Date = next;
                this.NeedsUpdate = true;
                IsBusy = false;
            });

            this.getNextMensaCommand = new Command(async () =>
            {
                IsBusy = true;
                string next = MensaAdapter.getNextMensaName(this.MensaName);
                this.MensaName = next;
                this.NeedsUpdate = true;
                IsBusy = false;
            });

            this.getPrevMensaCommand = new Command(async () =>
            {
                IsBusy = true;
                string next = MensaAdapter.getPreviousMensaName(this.MensaName);
                this.MensaName = next;
                this.NeedsUpdate = true;
                IsBusy = false;
            });

            this.IsCreated = true;
            this.NeedsUpdate = true;
        }

    }
}

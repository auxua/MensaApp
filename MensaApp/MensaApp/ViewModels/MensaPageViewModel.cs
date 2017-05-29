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

        private bool busy;

        public bool Busy
        {
            get
            {
                return this.busy;
            }
            set
            {
                this.busy = value;
                RaisePropertyChanged("Busy");
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
                Busy = true;
                this.needsUpdate = value;
                RaisePropertyChanged("NeedsUpdate");
                if (value)
                {
                    this.LoadAllDataCommand.Execute(null);
                }
                else
                {
                    var query = new MensaPortable.MenuDB.QueryBuilder().ByDate(this.Date).ByMensa(this.MensaName);
                    var dishes = query.ExecuteQuery();
                    if (dishes.Count < 1)
                    {
                        this.GetNextDayCommand.Execute(null);
                    }
                }
                this.needsUpdate = false;
                //Busy = false;
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

        private ObservableCollection<MensaPortable.DataTypes.Dish> items;

        public ObservableCollection<MensaPortable.DataTypes.Dish> Items
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
                CultureInfo cult;
                try
                {
                    cult = new CultureInfo(Localization.Locale());
				}
                catch
                {
                    cult = new CultureInfo("en-US");
                }
                var day = cult.DateTimeFormat.GetDayName(value.Date.DayOfWeek);
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

        internal MensaPortable.DataTypes.Dish Closed = new MensaPortable.DataTypes.Dish(Localization.Localize("Closed"), Localization.Localize("ClosedSubtext"),DateTime.Now, "");

        public MensaPageViewModel(string MensaName, DateTime dt)
        {
            this.MensaName = MensaName;
            this.Date = dt.Date;
            this.Status = "Starting...";
            this.getLocalizedStrings();
            this.items = new ObservableCollection<MensaPortable.DataTypes.Dish>();
            this.Busy = true;

            // Trigger the MensaDB to get the Mensa Data
            this.loadAllDataCommand = new Command(async () => 
                {
                    Busy = true;
                    this.Status = Localization.Localize("GetData");
                    bool done = true;
                    // Outdated without error? -> Refresh!
                    if (MensaPortable.MenuDB.Instance.isOutdated() && !MensaAdapter.DownloadError)
                    {
                        done = await MensaAdapter.CatchMensaDataAsync();
                    }
                    if (!done)
                    {
                        this.ErrorMessage = Localization.Localize("LoadFail");
                    }
                    else
                    {
                        this.DataAvailable = true;
                    }
                    //IsBusy = false;
                });

            // query the Mensa Data from the MensaDB
            this.getAllDataCommand = new Command(() =>
            {
                //Debugging only!
                //this.Date = Date.AddDays(-2);

                Busy = true;
                Status = "Populating Data";

                IList<MensaPortable.DataTypes.Dish> queryData = new List<MensaPortable.DataTypes.Dish>();
                bool searchNextDay = App.getConfig("searchNextDay");

                if (searchNextDay)
                {
                    // This whole looping is just for selecting a day for this canteen with dishes available. It automatically stops after 5 subsequent days without dishes
                    bool found = false;
                    int counter = 5;
                    while (!found && counter > 0)
                    {
                        var query = new MensaPortable.MenuDB.QueryBuilder().ByDate(this.Date).ByMensa(this.MensaName);

                        if (App.getConfig("VegieOnly"))
                            query = query.ByKind("Vegetarisch");

                        if (App.getConfig("MainDishesOnly"))
                            query = query.NoSideDishes();

                        queryData = query.ExecuteQuery();

                        if (queryData.Count > 0)
                            found = true;
                        else
                            this.Date = Date.AddDays(1);
                        counter--;
                    }
                    if (counter == 0 && !found)
                    {
                        // did not find any dishes! Reset Date
                        this.Date = Date.AddDays(-5);
                    }
                }
                else
                {
                    // old behavior: just use current day - independently from any dishes existing
                    var query = new MensaPortable.MenuDB.QueryBuilder().ByDate(this.Date).ByMensa(this.MensaName);

                    if (App.getConfig("VegieOnly"))
                        query = query.ByKind("Vegetarisch");

                    if (App.getConfig("MainDishesOnly"))
                        query = query.NoSideDishes();

                    queryData = query.ExecuteQuery();
                }
                
                
                this.Items.Clear();
                foreach (var dish in queryData)
                {
                    this.Items.Add(dish);
                }
                // No data? Show default Closed info
                if (this.Items.Count == 0)
                {
                    this.Items.Add(this.Closed);
                }
                this.HasData = true;
                Busy = false;
            });

            this.getNextDayCommand = new Command(() =>
            {
                Busy = true;
                DateTime next = MensaPortable.MenuDB.Instance.getNextAvailableDay(this.Date);
                // Optimization: No better day available? prevent reloading data and re-download in some cases
                if (next == this.Date)
                {
                    Busy = false;
                    return;
                }
                this.Date = next;
                this.NeedsUpdate = true;
                Busy = false;
            });

            this.getPrevDayCommand = new Command(() =>
            {
                Busy = true;
                DateTime next = MensaPortable.MenuDB.Instance.getPreviousAvailableDay(this.Date);
                // Optimization: No better day available? prevent reloading data and re-download in some cases
                if (next == this.Date)
                {
                    Busy = false;
                    return;
                }
                this.Date = next;
                this.NeedsUpdate = true;
                Busy = false;
            });

            this.getNextMensaCommand = new Command(() =>
            {
                Busy = true;
                string next = MensaAdapter.getNextMensaName(this.MensaName);
                this.MensaName = next;
                this.NeedsUpdate = true;
                Busy = false;
            });

            this.getPrevMensaCommand = new Command(() =>
            {
                Busy = true;
                string next = MensaAdapter.getPreviousMensaName(this.MensaName);
                this.MensaName = next;
                this.NeedsUpdate = true;
                Busy = false;
            });

            this.IsCreated = true;
            this.NeedsUpdate = true;
        }

    }
}

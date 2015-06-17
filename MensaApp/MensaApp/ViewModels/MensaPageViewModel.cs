using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using System.Threading;
using Xamarin.Forms;

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
                if (value)
                {
                    this.LoadAllDataCommand.Execute(null);
                }
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
                this.DateAsString = value.Date.DayOfWeek.ToString() + "\n" + value.Date.ToShortDateString();
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


        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        #endregion

        public MensaPageViewModel(string MensaName, DateTime dt)
        {
            this.MensaName = MensaName;
            this.Date = dt.Date;
            this.Status = "Creating Page";
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
                var queryData = new Mensa.MenuDB.QueryBuilder().ByDate(this.Date).ByMensa(this.MensaName).ExecuteQuery();
                //TODO: not querying weekends and stuff...
                foreach (var dish in queryData)
                {
                    this.Items.Add(dish);
                }
                this.HasData = true;
                IsBusy = false;
            });

            this.IsCreated = true;
        }

    }
}

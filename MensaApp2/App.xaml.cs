namespace MensaApp2
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        #region Settings/Config

        public static bool MensaActive(string name)
        {
            if (Preferences.ContainsKey(name))
            {
                string value = Preferences.Get(name,"False");
                return (value == "True");
            }
            Preferences.Set(name,"True");
            return true;
        }

        public static void MensaActivate(string name)
        {
            Preferences.Set(name, "True");
        }

        public static void MensaDeactivate(string name)
        {
            Preferences.Set(name, "False");
        }

        public static bool getConfig(string name)
        {
            if (Preferences.ContainsKey(name))
            {
                string value = Preferences.Get(name, "False");
                return (value == "True");
            }
            Preferences.Set(name, "False");
            return false;
        }

        public static void setConfig(string name, bool value)
        {
            string v;
            if (value)
            {
                v = "True";
            }
            else
            {
                v = "False";
            }

            Preferences.Set(name, v);
        }

        #endregion
    }
}
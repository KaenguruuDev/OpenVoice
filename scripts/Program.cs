using OpenVoice;
using OpenVoice.Forms;

namespace OpenVoiceApp
{
    class App
    {
        private User? ActiveUser = null;
        new private Form ActiveForm = null;
        private ThemeControl? ThemeControl = null;

        public App()
        {
            Form HomePage = new HomeScreen(SignUpPage, LogInPage, SettingsPage);
            SetThemeControl(new ThemeControl());
            ThemeControl.LoadTheme("E:/OpenVoice/assets/themes/blue_skies.json");

            ActiveForm = HomePage;

            Object[] Params = { ThemeControl.GetTheme() };
            SignalHandler.ConnectSignalToObj(ThemeControl.GetThemeChangedSignal(), HomePage, "UpdateTheme", Params);
            ThemeControl.UpdateTheme();

            Application.EnableVisualStyles();
            Application.Run(HomePage);
        }

        private Form? LogInForm;
        private Form? SignUpForm;
        private Form? SettingsForm;
        private Form? MainWindowForm;

        private void ChangePage(Form NewPage, SignalInfo[] SignalsToBeConnected)
        {
            if (NewPage == null || ThemeControl == null) { return; }

            if (ActiveForm != null) ActiveForm.Hide();

            foreach (SignalInfo SignalInfo in SignalsToBeConnected)
            {
                if (SignalInfo == null) continue;
                SignalHandler.ConnectSignalToObj(SignalInfo.GetSignal(), SignalInfo.GetObject(), SignalInfo.GetMethod(), SignalInfo.GetParams());
                if (SignalInfo.GetEmitOnConnect() && SignalInfo.GetSignal() != null) ThemeControl.UpdateTheme();
            }

            NewPage.Show();
            ActiveForm = NewPage;
        }

        public void HomePage(object sender, EventArgs e)
        {
            Form HomePage = new HomeScreen(SignUpPage, LogInPage, SettingsPage);
            Object[] Params = { ThemeControl.GetTheme() };
            SignalHandler.ConnectSignalToObj(ThemeControl.GetThemeChangedSignal(), HomePage, "UpdateTheme", Params);
            
            ThemeControl.UpdateTheme();
            ChangePage(HomePage, new SignalInfo[0]);
        }

        public void SettingsPage(object sender, EventArgs e)
        {
            if (ThemeControl == null) return;
            SettingsForm = new Settings(HomePage, ThemeControl);

            Theme? CurrentTheme = ThemeControl.GetTheme();
            if (CurrentTheme == null) return;
            Object[] Params = { CurrentTheme };

            Signal? ThemeChanged = ThemeControl.GetThemeChangedSignal();
            if (ThemeChanged == null) { return; }
            SignalInfo[] Signals = { new SignalInfo(ThemeChanged, SettingsForm, "UpdateTheme", Params, true) };
            
            ThemeControl.UpdateTheme();
            ChangePage(SettingsForm, Signals);
        }

        public void SignUpPage(object sender, EventArgs e)
        {
            if (ThemeControl == null) return;
            SignUpForm = new SignUp(SignUpPage, HomePage);

            Theme? CurrentTheme = ThemeControl.GetTheme();
            if (CurrentTheme == null) return;
            Object[] Params = { CurrentTheme };

            Signal? ThemeChanged = ThemeControl.GetThemeChangedSignal();
            if (ThemeChanged == null) { return; }
            SignalInfo[] Signals = { new SignalInfo(ThemeChanged, SignUpForm, "UpdateTheme", Params, true) };

            ThemeControl.UpdateTheme();
            ChangePage(SignUpForm, Signals);
        }

        public void LogInPage(object sender, EventArgs e)
        {
            if (ThemeControl == null) return;
            LogInForm = new LogIn(LogIn, HomePage);

            Theme? CurrentTheme = ThemeControl.GetTheme();
            if (CurrentTheme == null) return;
            Object[] Params = { CurrentTheme };

            Signal? ThemeChanged = ThemeControl.GetThemeChangedSignal();
            if (ThemeChanged == null) { return; }
            SignalInfo[] Signals = { new SignalInfo(ThemeChanged, LogInForm, "UpdateTheme", Params, true) };

            ThemeControl.UpdateTheme();
            ChangePage(LogInForm, Signals);
        }



        public void SignUp(object sender, EventArgs e)
        {
            //currentUser.SaveData(username, password);
        }

        public void LogIn(object sender, EventArgs e)
        {
            if (!(ActiveForm is LogIn)) return;
            if (User.VerifyLogin(((LogIn)ActiveForm).RequestUsername(), ((LogIn)ActiveForm).RequestPassword()))
            {
                ActiveUser = new User(((LogIn)ActiveForm).RequestUsername());

                if(ThemeControl == null) return;
                MainWindowForm = new MainWindow(ThemeControl);

                Theme? CurrentTheme = ThemeControl.GetTheme();
                if (CurrentTheme == null) return;
                Object[] Params = { CurrentTheme };

                Signal? ThemeChanged = ThemeControl.GetThemeChangedSignal();
                if (ThemeChanged == null) { return; }
                SignalInfo[] Signals = { new SignalInfo(ThemeChanged, MainWindowForm, "UpdateTheme", Params, true) };

                ThemeControl.UpdateTheme();
                ChangePage(MainWindowForm, Signals);
            }
        }

        public ThemeControl? GetThemeControl()
        { return ThemeControl; }
        public User? GetActiveUser()
        { return ActiveUser; }
        public Form? GetActiveForm()
        { return ActiveForm; }
        public void SetActiveForm(Form ActiveForm)
        { this.ActiveForm = ActiveForm; }
        public void SetThemeControl(ThemeControl ThemeControl)
        { this.ThemeControl = ThemeControl; }
    }

    class Program
    {
        static App? CurrentApp = null;


        static void Main(string[] args)
        {
            CurrentApp = new App();
        }
    }
}
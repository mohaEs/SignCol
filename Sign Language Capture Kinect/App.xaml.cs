using DbModel.Extensions;
using Sign_Language_Capture_Kinnect.Pages;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using StructureMap;
using DbModel.Context;
using DbModel.Services.Interfaces;
using DbModel.Services;
using System.Data.Entity;
using DbModel.Context.Migrations;
using System.ServiceProcess;

namespace Sign_Language_Capture_Kinnect
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const int MINIMUM_SPLASH_TIME = 1500; // Miliseconds
        public App()
        {
            Cultures.InitializePersianCulture();
            InitializeComponent();

        }

        private static void initStructureMap()
        {
            ObjectFactory.Initialize(x =>
            {
                x.For<IUnitOfWork>().CacheBy(InstanceScope.Hybrid).Use<MyDbContext>();
                x.For<IWords>().Use<WordsService>();
                x.For<ILanguages>().Use<LanguageServise>();
                x.For<IOptionService>().Use<OptionService>();
                x.For<IUser>().Use<UserService>();
                x.For<IVideo>().Use<VideoService>();

                x.SetAllProperties(y =>
                {
                    y.OfType<IUnitOfWork>();
                    y.OfType<IWords>();
                    y.OfType<ILanguages>();
                    y.OfType<IOptionService>();
                    y.OfType<IUser>();
                    y.OfType<IVideo>();
                });
            });
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            //METHOD ONE STARTS

            MainWindow main = new MainWindow();

            Startup splash = new Startup();// SplashScreen();
            splash.Show();
            // Step 2 - Start a stop watch
            Stopwatch timer = new Stopwatch();
            timer.Start();

            base.OnStartup(e);
            AppDomain.CurrentDomain.SetData("DataDirectory",
                 AppDomain.CurrentDomain.BaseDirectory);



            int remainingTimeToShowSplash = MINIMUM_SPLASH_TIME - (int)timer.ElapsedMilliseconds;
            if (remainingTimeToShowSplash > 0)
                Thread.Sleep(remainingTimeToShowSplash);

            splash.Close();


            initStructureMap();
        }
    }
}

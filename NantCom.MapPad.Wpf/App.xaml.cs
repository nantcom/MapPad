using NantCom.MapPad.Core;
using NantCom.MapPad.Core.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NantCom.MapPad.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Any( s => s == "setupapp" ))
            {
                MetroAppSetup.Setup();

                Application.Current.Shutdown();

                return;
            }

            MapPadServer.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            MapPadServer.Stop();
            GamePadHub.StopGamePad();

            base.OnExit(e);
        }
    }
}

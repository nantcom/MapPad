using NantCom.MapPad.Core;
using NantCom.MapPad.Core.Hubs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NantCom.MapPad.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private CancellationTokenSource _Cancel = new CancellationTokenSource();

        ///<summary>
        ///Get or set the value of GamePadEnabled
        ///</summary>
        public bool GamePadEnabled
        {
            get
            {
                return Gamepad.IsPolling;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;
            this.Visibility = System.Windows.Visibility.Collapsed;

            GamePadHub.DeviceError += () =>
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.TaskBarIcon.ShowBalloonTip("NC MapPad",
                        "Device disconnected",
                        Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Error);

                    this.PropertyChanged(this, new PropertyChangedEventArgs("GamePadEnabled"));
                }));
            };
        }

        private bool _IsRun;
        protected override void OnActivated(EventArgs e)
        {
            if (_IsRun == true)
            {
                return;
            }

            _IsRun = true;

            this.EnableMap_Click(null, null);
            MapPadProfile.StartProfileByProcessMapping(_Cancel.Token);

        }

        private void DownloadStoreApp_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {

            MetroAppSetup.Setup();
        }

        private void EnableMap_Click(object sender, RoutedEventArgs e)
        {

            if (Gamepad.IsPolling == false)
            {
                GamePadHub.StartGamePad();
                if (Gamepad.CurrentDevice != null)
                {
                    this.TaskBarIcon.ShowBalloonTip("NC MapPad", 
                        "Connected to: " + Gamepad.CurrentDevice.ProductName, 
                        Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
                }
                else
                {
                    this.TaskBarIcon.ShowBalloonTip("NC MapPad", 
                        "No Device Found!", 
                        Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Error);
                }

            }
            else
            {
                GamePadHub.StopGamePad();

                this.TaskBarIcon.ShowBalloonTip("NC MapPad",
                    "Disconnected",
                    Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
            }

            this.PropertyChanged(this, new PropertyChangedEventArgs("GamePadEnabled"));

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (Gamepad.IsPolling)
            {
                var answer = MessageBox.Show("Your Game Pad will stop being mapped. Sure about that? ", "Stop MapPad",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (answer == MessageBoxResult.Yes)
                {
                    GamePadHub.StopGamePad();
                    _Cancel.Cancel();
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }

        }

    }
}

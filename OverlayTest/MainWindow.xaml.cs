using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace OverlayTest
{
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Policy;
    using System.Threading;
    using System.Windows.Interop;
    using System.Windows.Automation;
    using System.Windows.Automation;
    using FileWatcherAPI.Models;
    using FileWatcherAPICLI;
    using OverlayTest.WIndowsModules;
    using Busylight;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool UseLights { get; set; } = true;

        public List<ComboboxItems> StateList = new List<ComboboxItems>();
        public static Busylight.SDK _busylight = new Busylight.SDK();

        public FileWatcherAPI Watcher = new FileWatcherAPI(@"C:\Users\" + Environment.UserName +
                                                           @"\AppData\Roaming\Intelecom Group AS\Intelecom Connect\Logs\all.txt")
            ;

        private IntPtr? _windowHandle;
        private object locker = new object();
        private IObservable<IntPtr?> test;
        public MainWindow MainWindowInstance { get; set; }

        public IntPtr? WindowHandle
        {
            get
            {
                lock (locker)
                {
                    return _windowHandle;
                }
            }
            set
            {
                lock (locker)
                {
                    _windowHandle = value;
                }
            }
        }

        public CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();
        public ConnectHandler ConnectHandler { get; set; } = new ConnectHandler();
        public bool Running { get; set; } = false;

        public MainWindow()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            Watcher.Events.Setup += SetupLight;
            Watcher.Events.Alerting += AlertingLight;
            Watcher.Events.Connected += ConnectedLight;
            Watcher.Events.Hangup += HangupLight;
            Watcher.Events.NoAnswer += NoAnswerLight;
            Watcher.Events.PauseOn += PauseOnLight;
            Watcher.Events.PauseOff += PauseOffLight;
            this.Watcher.Events.Aborted += AbortedLight;
            //turn background solid
            this.Background = new SolidColorBrush(Colors.Transparent);
            this.AllowsTransparency = true;

            MainWindowInstance = this;
            MainWindowInstance.Hide();

            StateBox.ItemsSource = new ComboboxItems().ComboboxItemList;
            this.StateBox.SelectedIndex = 0;
            StateBox.DisplayMemberPath = "Text";
            PeriodicTask.Run(CheckConnectHandler, new TimeSpan(0, 0, 0, 0, 200));

            ConnectHandler.PointerEvent += PointerChanged;
            ConnectHandler.RunningEvent += ConnectRunningStateChanged;
        }

        private void PointerChanged(object source, IntPtr e)
        {
            CancellationTokenSource.Cancel();
            Thread.Sleep(100);
            CancellationTokenSource = new CancellationTokenSource();
            User32.SetWindowOwner(e);
            //Thread.Sleep(200);

            var t2 = new Thread(delegate() { TrackWindow(e, CancellationTokenSource); });
            t2.Start();
        }

        private void ConnectRunningStateChanged(object source, bool running)
        {
            if (running) return;
            MainWindowInstance.Hide();
            CancellationTokenSource.Cancel();
        }


        private void CheckConnectHandler()
        {
            var temp = User32.GetWindowHandle("Intelecom.Connect");
            if (temp.HasValue)
            {
                ConnectHandler.Pointer = temp.Value;
                ConnectHandler.ProgramIsRunning = true;

                return;
            }
            ConnectHandler.ProgramIsRunning = false;
        }

        


        private void TrackWindow(IntPtr windowHandleIntPtr, CancellationTokenSource token)
        {
            var mainwindowLaunched = false;
            RECT rect = new RECT();
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                if (!mainwindowLaunched)
                {
                    mainwindowLaunched = CheckMainwindowLaunched(windowHandleIntPtr);
                }

                GetWindowRect(windowHandleIntPtr, ref rect);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.Left = rect.Left + 350;
                    this.Top = rect.Top + 25;
                });
            }
        }


        private bool CheckMainwindowLaunched(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                return false;
            }
            try
            {
                AutomationElement targetApp =
                    AutomationElement.FromHandle(handle);


                var allEditors = targetApp.FindAll(TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text));
                if (allEditors.Cast<AutomationElement>().Any(element => element.Current.Name == "TELEFON"))
                {
                    Thread.Sleep(500);
                    User32.ShowWindow(handle, 6);
                    //Thread.Sleep(50);
                    User32.ShowWindow(handle, 9);
                    Dispatcher.Invoke(() => MainWindowInstance.Show());

                    return true;
                }
            }
            catch (ElementNotAvailableException ex)
            {
            }
            return false;
        }


        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private void StateBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboboxItem = ((sender as ComboBox).SelectedItem as ComboboxItem);
            UseLights = comboboxItem.LightsOn;

            if (comboboxItem.Text == "Avslutt")
            {
                if (MessageBox.Show("Avslutt BuzzyLight?", "Er du sikker på du vil avslutte?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {                   
                    CancellationTokenSource.Cancel();
                    MainWindowInstance.Close();
                }
            }

            if (UseLights)
            {
                this.Watcher.Start();
                _busylight.Light(BusylightColor.Green);
            }
            else
            {
                this.Watcher.Stop();
                _busylight.Light(BusylightColor.Off);
            }
        }

        private static void PauseOffLight(object source, bool e)
        {
            //#ff0066 Lilla
            _busylight.Light(BusylightColor.Green);
            Console.WriteLine("PauseOFF");
        }

        private static void PauseOnLight(object source, bool e)
        {
            //#ff0066 Lilla
            _busylight.Light(BusylightColor.Red);
            Console.WriteLine("PauseON");
        }

        private static void SetupLight(object source, Call e)
        {
            //#ff0066 Lilla
            _busylight.Light(BusylightColor.Yellow);
            Console.WriteLine(e.system_call_progress);
        }

        private static void AlertingLight(object source, Call e)
        {
            //#ffff00 gul
            _busylight.Pulse(new BusylightColor() {BlueRgbValue = 255, GreenRgbValue = 255, RedRgbValue = 0});
            Console.WriteLine(e.system_call_progress);
        }

        private static void ConnectedLight(object source, Call e)
        {
            //#ff3300 rød
            _busylight.Light(BusylightColor.Red);
            Console.WriteLine(e.system_call_progress);
        }

        private static void HangupLight(object source, Call e)
        {
            //#33cc33 Grønn
            _busylight.Light(BusylightColor.Green);
            Console.WriteLine(e.system_call_progress);
        }

        private static void NoAnswerLight(object source, Call e)
        {
            //#33cc33 Grønn
            _busylight.Light(BusylightColor.Green);
            Console.WriteLine(e.system_call_progress);
        }

        private static void AbortedLight(object source, Call e)
        {
            //#33cc33 Grønn
            _busylight.Light(BusylightColor.Blue);
            Console.WriteLine(e.system_call_progress);
        }
    }
}
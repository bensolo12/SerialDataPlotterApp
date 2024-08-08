using System.Windows;
using System.IO.Ports;
using System.Windows.Threading;
using System.IO;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace DataPlotterApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPortFactory port6;
        BackgroundWorker DataListener = new BackgroundWorker();
        public ObservableCollection<string> Current { get; private set; }
        public ObservableCollection<string> Voltage { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            port6 = new SerialPortFactory("COM6");            
            DataListener.DoWork += new DoWorkEventHandler(CheckForUpdate);
            DataListener.RunWorkerAsync();
        }

        private async void CheckForUpdate(object sender, DoWorkEventArgs e)
        {
            while (string.IsNullOrWhiteSpace(port6.Current)) await Task.Delay(10);
            Voltage.Add("Latest Voltage: " + port6.Voltage);
            Current.Add("Latest Current: " + port6.Current);
        }

        //private void DataListener_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    Current.Content = port6.Current;
        //    Voltage.Content = port6.Voltage;
        //}

        private void Close_Form(object sender, RoutedEventArgs e)
        {
            port6.closePort();
            Close();
        }
    }
}
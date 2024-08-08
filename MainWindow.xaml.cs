using System.Windows;
using System.IO.Ports;
using System.Windows.Threading;
using System.IO;

namespace DataPlotterApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort serial = new SerialPort();
        string received_data;
        IEnumerable<string> allData = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            setUpSerialPort();
        }
        private void setUpSerialPort()
        {
            serial.PortName = "COM6";
            serial.BaudRate = 9600;
            serial.Handshake = Handshake.None;
            serial.Parity = Parity.None;
            serial.DataBits = 8;
            serial.StopBits = StopBits.Two;
            serial.ReadTimeout = 200;
            serial.WriteTimeout = 50;
            serial.Open();
            serial.DataReceived += new SerialDataReceivedEventHandler(Recieve);
        }
        private void Recieve(object sender, SerialDataReceivedEventArgs e)
        {
            received_data = serial.ReadExisting();
            Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteData), received_data);
        }

        private delegate void UpdateUiTextDelegate(string text);

        private void WriteData(string text)
        {
            File.AppendAllText("data.txt", text);
            allData.Append(text);
        }

        private void Close_Form(object sender, RoutedEventArgs e)
        {
            if (serial.IsOpen) serial.Close();
            Close();
        }
    }
}
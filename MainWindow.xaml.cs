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
        SerialPortFactory port6;

        public MainWindow()
        {
            InitializeComponent();
            port6 = new SerialPortFactory("COM6");
            if(port6.connect() == 1)
            {
                MockSerialPort mock = new MockSerialPort();
                DataContext = mock;
            }
            DataContext = port6;
        }        

        private void Close_Form(object sender, RoutedEventArgs e)
        {
            port6.closePort();
            Close();
        }
    }
}
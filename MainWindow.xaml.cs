using System.Windows;
using System.IO.Ports;
using System.Windows.Threading;
using System.IO;
using OxyPlot;

namespace DataPlotterApp
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string initialcurrent;
        private string current;
        //public string DisplayCurrent
        //{
        //    get { return initialcurrent; }
        //    set
        //    {
        //        if (current != value)
        //        {
        //            current = value;
        //            OnPropertyChanged("DisplayCurrent");
        //        }
        //    }
        //}

        private void OnPropertyChanged(string v)
        {
            throw new NotImplementedException();
        }

        SerialPortFactory port6;
        MockSerialPort mock;

        public MainWindow()
        {
            InitializeComponent();
            port6 = new SerialPortFactory("COM6");
            mock = new MockSerialPort();
            if (port6.ConnectionState == 0)
            {                
                DataContext = port6;
            }
            else {
                mock.connectMock();
                DataContext = mock;
                update();
            }            
        }

        private void update()
        {            
            string initialCurrent = mock.Current;
            string initialVoltage = mock.Voltage;
            while (mock.ConnectionState == 1)
            {                
                //Points.Add(new DataPoint(Convert.ToDouble(mock.Current), Convert.ToDouble(mock.Voltage)));
                if(initialCurrent != mock.Current)
                {
                    isMock.Content = mock.Current;
                    initialVoltage = mock.Voltage;
                }
            }
        }
        public string Title { get; private set; }

        public IList<DataPoint> Points { get; private set; }

        private void Close_Form(object sender, RoutedEventArgs e)
        {
            port6.closePort();
            Close();
        }
    }
}
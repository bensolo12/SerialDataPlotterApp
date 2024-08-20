using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.IO;

namespace DataPlotterApp
{
    internal class MockSerialPort : SerialPortFactory
    {
        string received_data;
        IEnumerable<string> allData = new List<string>();
        private string current;
        private string voltage;
        private string isMock;
        private int randCurrent;
        private int randVoltage;
        private string allCurrentData;
        private string DispPortname;

        public string IsMock
        {
            get { return DispPortname; }
            set
            {
                if (DispPortname != value)
                {
                    DispPortname = value;
                    OnPropertyChanged("IsMock");
                }
            }
        }
        public string Current
        {
            get { return current; }
            set
            {
                if (current != value)
                {
                    current = value;
                    OnPropertyChanged("Current");
                }
            }
        }
        public string Voltage
        {
            get { return voltage; }
            set
            {
                if (voltage != value)
                {
                    voltage = value;
                    OnPropertyChanged("Voltage");
                }
            }
        }
        public MockSerialPort()
        {
            //DispPortname = "Mock Connected";            
            connectMock();            
        }
        private async void connectMock()
        {
            Console.WriteLine("Mock connecting");
            isMock = "Mock Connected";
            Current = "0";
            Voltage = "0";
            DispPortname = "Connected to port: Mock";
            while (isMock == "Mock Connected")
            {
            await Task.Run(() => runBackgroundWorker());

            }
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteData), allCurrentData);

        }
        private async void runBackgroundWorker()
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(MockData);
            backgroundWorker.RunWorkerAsync();
        }
        private void MockData(object? sender, DoWorkEventArgs e)
        {
            while (isMock == "Mock Connected")
            {
                randCurrent = new Random().Next(0, 100);
                randVoltage = new Random().Next(0, 24);
                allCurrentData = randCurrent.ToString() + "," + randVoltage.ToString();
                //Application.Current.Dispatcher.Invoke(() =>
                //{
                    WriteData(allCurrentData);
                //});
                
                //Thread.Sleep(1000);
            }
        }
        private delegate void UpdateUiTextDelegate(string text);

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnDataChanged(string data)
        {
            if(Application.Current.Dispatcher.CheckAccess())
            {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(data));
            }
            else             {
                Application.Current.Dispatcher.Invoke(() => { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(data)); });
            }
        }

        private void WriteData(string text)
        {
            List<string> allLines = text.Split("\n").ToList();
            var topLine = allLines.First().Split(",");
            Current = topLine.First();
            Voltage = topLine.Last();
            //WriteToFile(text);
            allData.Append(text);
        }
        private void WriteToFile(string text)
        {
            List<string> allLines = text.Split("\n").ToList();
            foreach (string line in allLines)
            {
                string timeStampedData = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + line + "\n";
                File.AppendAllText("data.txt", timeStampedData);
            }
            
        }
    }
}

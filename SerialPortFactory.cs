using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

namespace DataPlotterApp
{
    internal class SerialPortFactory
    {
        SerialPort serial = new SerialPort();
        string received_data;
        IEnumerable<string> allData = new List<string>();
        private string PortName;
        private int BaudRate;
        private string current;
        private string voltage;

        public string Current
        {
            get { return current; }
            set { current = value; }
        }
        public string Voltage
        {
            get { return voltage; }
            set { voltage = value; }
        }

        public SerialPortFactory(string portName, int baudRate = 9600)
        {
            PortName = portName;
            BaudRate = baudRate;
            connect();
        }
        private void connect()
        {
            serial.PortName = PortName;
            serial.BaudRate = BaudRate;
            serial.Handshake = Handshake.None;
            serial.Parity = Parity.None;
            serial.DataBits = 8;
            serial.StopBits = StopBits.Two;
            serial.ReadTimeout = 200;
            serial.WriteTimeout = 50;
            serial.Open();
            serial.DataReceived += new SerialDataReceivedEventHandler(Recieve);
        }
        private delegate void UpdateUiTextDelegate(string text);
        private void Recieve(object sender, SerialDataReceivedEventArgs e)
        {
            string? allCurrentData = serial.ReadExisting();            
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteData), allCurrentData);
        }

        private void WriteData(string text)
        {
            List<string> allLines = text.Split("\n").ToList();
            var topLine = allLines.First().Split(",");
            Current = topLine.First();
            Voltage = topLine.Last();
            WriteToFile(text);
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
        public void closePort()
        {
            if (serial.IsOpen) serial.Close();
        }
    }
}

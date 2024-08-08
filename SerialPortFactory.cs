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
            received_data = serial.ReadExisting();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteData), received_data);
        }

        private void WriteData(string text)
        {
            string timeStampedData = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + text;
            File.AppendAllText("data.txt", timeStampedData);
            allData.Append(text);
        }
        public void closePort()
        {
            if (serial.IsOpen) serial.Close();
        }
    }
}

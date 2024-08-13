﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;
using System.ComponentModel;

namespace DataPlotterApp
{
    internal class SerialPortFactory : INotifyPropertyChanged
    {
        SerialPort serial = new SerialPort();
        string received_data;
        IEnumerable<string> allData = new List<string>();
        private string PortName;
        private int BaudRate;
        private string current;
        private string voltage;     
        private string DispPortname;
        private int randCurrent;
        private int randVoltage;
        private string allCurrentData;
        private int connectionState;

        
        public int ConnectionState
        {
            get { return connect(); }
            set
            {
                if (connectionState != value)
                {
                    connectionState = value;
                    OnPropertyChanged("ConnectionState");
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

        public SerialPortFactory(string portName, int baudRate = 9600)
        {
            PortName = portName;
            BaudRate = baudRate;            
        }
        public SerialPortFactory() { }
        public int connect()
        {
            serial.PortName = PortName;
            serial.BaudRate = BaudRate;
            serial.Handshake = Handshake.None;
            serial.Parity = Parity.None;
            serial.DataBits = 8;
            serial.StopBits = StopBits.Two;
            serial.ReadTimeout = 200;
            serial.WriteTimeout = 50;
            try
            {
                DispPortname = "Connected to port: " + PortName;
                Console.WriteLine("Connected to port: " + PortName);
                serial.Open();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Port not found, connecting mock");
                return RadioConnectionReturnCode.CONNECTION_FAILURE;
            }
            serial.DataReceived += new SerialDataReceivedEventHandler(Recieve);
            return RadioConnectionReturnCode.SUCCESS;
        }
        
        private delegate void UpdateUiTextDelegate(string text);

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

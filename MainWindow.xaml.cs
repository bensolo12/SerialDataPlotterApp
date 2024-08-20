﻿using System.Windows;
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
        MockSerialPort mock;

        public MainWindow()
        {
            InitializeComponent();
            port6 = new SerialPortFactory("COM6");
            if (port6.ConnectionState == 0)
            {                
                DataContext = port6;
            }
            else {
                mock = new MockSerialPort();
                DataContext = mock;
            }            
        }        

        private void Close_Form(object sender, RoutedEventArgs e)
        {
            port6.closePort();
            Close();
        }
    }
}
using System.Diagnostics;
using System.IO.Ports;
using System.Windows;


namespace GridSim.Model
{
    public class SerialModel
    {
        private SerialPort? serialPort;
        public event Action<string>? DataReceived;
        public bool isConnected = false;

        public bool Connect(string portName, int baudRate)
        {
            bool success = false;
            App.Current.Dispatcher.Invoke(() =>
            {
                Disconnect();
            try
            {
                serialPort = new SerialPort(portName, baudRate)
                {
                    ReadTimeout = 500,
                    WriteTimeout = 500
                };
                serialPort.DataReceived += SerialPort_DataReceived;
                serialPort.Open();
                isConnected = true;
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                success = false;
            }
            });
            return success;
            }

        public void Send(String data)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.WriteLine(data);
                }
            });
        }
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

                App.Current.Dispatcher.Invoke(() =>
                {
            // Läs inkommande data och skicka vidare via event
            try
            {
            string? data = serialPort?.ReadTo("\n");
            DataReceived?.Invoke(data);
            }
            catch (Exception ex)
            {
                    //Send(e.ToString());
                    //Debug.WriteLine("Error in SerialPort_DataReceived\n");
                    MessageBox.Show(ex.Message);
            }
                });
        }

        public void Disconnect()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.DataReceived -= SerialPort_DataReceived;
                    if (serialPort.IsOpen)
                    {
                        isConnected = false;
                        serialPort.Close();
                    }
                }
            });
        }

    }
}

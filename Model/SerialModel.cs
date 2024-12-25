using System.Diagnostics;
using System.IO.Ports;
using System.Windows;
using System.Threading.Tasks;



namespace GridSim.Model
{
    public class SerialModel
    {
        private SerialPort? serialPort;
        private CancellationTokenSource? cancellationTokenSource;
        public event Action<string>? DataReceived;
        public bool IsConnected = false;

        public bool Connect(string portName, int baudRate)
        {
            Disconnect(); // Ensure cleanup before connecting

            try
            {
                serialPort = new SerialPort(portName, baudRate)
                {
                    ReadTimeout = 500,
                    WriteTimeout = 500
                };
                serialPort.Open();
                IsConnected = true;

                // Start background task for reading data
                cancellationTokenSource = new CancellationTokenSource();
                Task.Run(() => ListenForData(cancellationTokenSource.Token));

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error connecting to {portName}: {e.Message}");
                return false;
            }
        }

        public void Send(string data)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    serialPort.WriteLine(data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending data: {ex.Message}");
                }
            }
        }

        private void ListenForData(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested && serialPort != null && serialPort.IsOpen)
                {
                    string? data = serialPort.ReadTo("\n");
                    if (!string.IsNullOrWhiteSpace(data))
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            DataReceived?.Invoke(data);
                        });
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when the token is canceled
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading data: {ex.Message}");
            }
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
            if (serialPort != null && serialPort.IsOpen)
            {
                cancellationTokenSource?.Cancel();
                cancellationTokenSource?.Dispose();
                serialPort.Close();
                IsConnected = false;
            }
        }

    }
}

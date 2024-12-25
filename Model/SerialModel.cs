using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GridSim.Model
{
    public class SerialModel
    {
        private SerialPort? serialPort;
        private CancellationTokenSource? cancellationTokenSource;
        public event Action<string>? DataReceived;
        public bool IsConnected { get; private set; } = false;

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
                    try
                    {
                        string? data = serialPort.ReadLine(); // Changed to ReadLine for simplicity
                        if (!string.IsNullOrWhiteSpace(data))
                        {
                            App.Current.Dispatcher.Invoke(() => DataReceived?.Invoke(data));
                        }
                    }
                    catch (TimeoutException)
                    {
                        // Ignore timeout exceptions to allow continuation
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading data: {ex.Message}");
            }
        }

        public void Disconnect()
        {
            if (serialPort != null)
            {
                try
                {
                    cancellationTokenSource?.Cancel();
                    cancellationTokenSource?.Dispose();
                    if (serialPort.IsOpen)
                    {
                        serialPort.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during disconnect: {ex.Message}");
                }
                finally
                {
                    IsConnected = false;
                    serialPort = null;
                }
            }
        }
    }
}

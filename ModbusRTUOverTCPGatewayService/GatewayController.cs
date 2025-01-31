using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModbusRTUOverTCPGatewayService
{
	public class GatewayController
	{
		private int _port;
		private string _comPort;
		private SerialPortLayer _commLayer;
		private Listener _listener;
		private int _incomingPacketsDelay;
		private readonly object _modbusDeviceLock = new object();
		private volatile int _activeClients;
		public Action<string> UpdateStatusDelegate { get; set; }

		public GatewayController(string comport, int baudRate, int port, Action<string> updateStatusDelegate, int incomingPacketsDelayMs)
		{
			_comPort = comport;
			_port = port;

			_commLayer = new SerialPortLayer(_comPort, baudRate, Parity.None, 8, StopBits.One);
			_commLayer.ReceiveTimeout = 5000;
			_commLayer.SendTimeout = 5000;

			_listener = new Listener(_port, Timeout.Infinite);
			_incomingPacketsDelay = incomingPacketsDelayMs;

			UpdateStatusDelegate = updateStatusDelegate;
			_activeClients = 0;
		}

		private byte[] OnReceivedData(byte[] received)
		{
			if (received.Any())
			{
				byte[] response;
				lock (_modbusDeviceLock)
				{
					_commLayer.SendSynced(received);
					Thread.Sleep(200);
					response = _commLayer.ReceiveSynced();
				}
				UpdateStatusDelegate?.Invoke($"Connected modbus device responded the following data: {Utilities.ByteArrayToString(response)}");
				return response;
			}
			else
			{
				UpdateStatusDelegate?.Invoke("No data found.");
				return new byte[] { };
			}
		}


		private void HandleClient(object tcpClient)
		{
			TcpClient client = (TcpClient)tcpClient;
			_activeClients++;
			UpdateStatusDelegate?.Invoke($"The number of active clients is: {_activeClients}.");
			try 
			{
				while (client.Client.IsConnected())
				{
					if (client.Available > 0)
					{
						NetworkStream stream = client.GetStream();

						byte[] receivedData = new byte[client.Available];
						stream.Read(receivedData, 0, client.Available);

						UpdateStatusDelegate?.Invoke($"Received the following data from client {client.Client.RemoteEndPoint}: {Utilities.ByteArrayToString(receivedData)}");

						byte[] response = OnReceivedData(receivedData);
						client.Client.Send(response);

						UpdateStatusDelegate?.Invoke($"The following data was sent to the client {client.Client.RemoteEndPoint}: {Utilities.ByteArrayToString(response)}");
					}
					Thread.Sleep(1000);
				}

				UpdateStatusDelegate?.Invoke($"Client {client.Client.RemoteEndPoint} is no longer active.");
				client.Close();
				UpdateStatusDelegate?.Invoke("The client socket was closed.");

				_activeClients--;
				UpdateStatusDelegate?.Invoke($"The number of active clients is: {_activeClients}.");
				return;
			}
			catch 
			{
				client?.Close();
				UpdateStatusDelegate?.Invoke("The client socket was closed.");

				_activeClients--;
				UpdateStatusDelegate?.Invoke($"The number of active clients is: {_activeClients}.");
				return;
			}
		}

		private async Task ServerAsync()
		{
			TcpClient TCPClient = null;
			_listener.Start();
			UpdateStatusDelegate?.Invoke($"The server is now listening on port {_port}.");
			while (true)
			{
				try
				{
					TCPClient = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);

					UpdateStatusDelegate?.Invoke($"Client {TCPClient.Client.RemoteEndPoint} accepted.");
					Thread.Sleep(_incomingPacketsDelay);
					ThreadPool.QueueUserWorkItem(HandleClient, TCPClient);
				}
				catch (Exception e) { UpdateStatusDelegate?.Invoke($"An exception occurred: {e.Message}\nStack trace: {e.StackTrace}"); }
			}
		}

		public async void StartProcessAsync()
		{
			if (!_commLayer.Connected)
				_commLayer.Connect();
			await Task.Run(() => ServerAsync().ConfigureAwait(false)).ConfigureAwait(false);
		}

		public void StopProcess()
			=> _commLayer?.Close();
	}
}

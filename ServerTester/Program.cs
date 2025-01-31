using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerTester
{
	class Program
	{
        public static string IP { get; set; }
        public static int Port { get; set; }

		static void Main(string[] args)
		{
            GetInfoFromFile();
            Connect(IP, new byte[] { 0x01, 0x03, 0x00, 0x04, 0x00, 0x01,0xc5, 0xcb }, Port);
		}

        static void GetInfoFromFile()
		{
            Port = AppSettings.Get("port", 8082);
            IP = AppSettings.Get("ip", "127.0.0.1");
        }

        static void Connect(String server, byte[] message, int port)
        {
            try
            {
				// Create a TcpClient.
				// Note, for this client to work you need to have a TcpServer
				// connected to the same address as specified by the server, port
				// combination.
				//TcpClient client = new TcpClient(server, port);
				TcpClient client = new TcpClient
				{
					//ExclusiveAddressUse = true,
					//LingerState = new LingerOption(true, 0),
					//NoDelay = true,
					ReceiveTimeout = 5000,
					SendTimeout = 5000
				};

                Console.WriteLine($"Connecting with {IP}:{Port}...");
                client.Connect(new IPEndPoint(IPAddress.Parse(server), port));

				// Translate the passed message into ASCII and store it as a Byte array.
				byte[] dataSend = message;
                byte[] dataReceived = new byte[7];

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                Console.WriteLine($"Is client connected? {client.Connected}");
                NetworkStream stream = client.GetStream();

                Thread.Sleep(1000);

                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine("Press any key to read temperature.");
                    Console.ReadKey();

                    stream.Write(dataSend, 0, dataSend.Length);
                    Console.WriteLine("Sent: {0}", Utilities.ByteArrayToString(dataSend));
                    
                    Int32 bytes = stream.Read(dataReceived, 0, dataReceived.Length);
                    string responseData = Utilities.ByteArrayToString(dataReceived);
                    Console.WriteLine("Received: {0}", responseData);

                    Console.WriteLine($"Temperature: {Utilities.ConvertTemperature(dataReceived)}");
                }
                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusRTUOverTCPGatewayService
{
	static class CommandLineUI
	{
		public static void ProcessCommands(string[] args)
		{
			Console.WriteLine("Starting as application...");

			if (args[0] == "-console" || args[0] == "-c")
				StartAsConsoleApplication();
			else if (args[0] == "-help" || args[0] == "-h")
				DisplayHelpMessage();
			else
			{
				Console.WriteLine($"Bad argument: '{args[0]}'.\n");
				DisplayHelpMessage();
				throw new ArgumentException("Invalid option", args[0]);
			}
		}

		public static void DisplayHelpMessage()
			=> Console.WriteLine("Valid options are: " + 
				"\tnone: Run as service.\n" +
				"\t-console ou -c: Run as command line app.\n" +
				"\t-help ou -h: Shows this help message.\n");

		public static void UpdateStatus(string message)
			=> Console.WriteLine(message);

		public static void StartAsConsoleApplication()
		{
			GatewayController controller = new GatewayController(
				AppSettings.Get("comport", "COM16"),
				AppSettings.Get("baudrate", 9600),
				AppSettings.Get("port", 8082),
				((data) => UpdateStatus(data)),
				AppSettings.Get("timeoutAfterClientAccept", 8082));

			controller.StartProcessAsync();

			while (true);
		}
	}
}

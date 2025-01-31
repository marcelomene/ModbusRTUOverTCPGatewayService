using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ModbusRTUOverTCPGatewayService
{
	static class Program
	{
		internal const int SUCCESS = 0;
		internal const int ERROR_GENERAL = -1;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static int Main(string[] args)
		{
			if (args.Length > 0)
			{
				try { CommandLineUI.ProcessCommands(args); }
				catch(Exception e) 
				{
					Console.WriteLine("There was an error while running the as command line app. " +
						$"Error message: {e.Message}");
					return ERROR_GENERAL;
				}
				return SUCCESS;
			}
			else
			{
				ServiceBase.Run(new ModbusRTUOverTCPGatewayService());
				return SUCCESS;
			}
		}
	}
}

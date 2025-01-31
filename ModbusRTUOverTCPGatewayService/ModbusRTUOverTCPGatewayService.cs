using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ModbusRTUOverTCPGatewayService
{
	public partial class ModbusRTUOverTCPGatewayService : ServiceBase
	{
		private GatewayController _controller;

		public ModbusRTUOverTCPGatewayService()
		{
			InitializeComponent();

			//if (!EventLog.SourceExists("ModbusRTUOverTCPGatewayServiceSource"))
			//	EventLog.CreateEventSource("ModbusRTUOverTCPGatewayServiceSource", "ModbusRTUOverTCPGatewayServiceLog");
			//EventLog.Source = "ModbusRTUOverTCPGatewayServiceSource";
			//EventLog.Log = "ModbusRTUOverTCPGatewayServiceLog";

			_controller = new GatewayController(
				AppSettings.Get("comport", "COM16"),
				AppSettings.Get("baudrate", 9600),
				AppSettings.Get("port", 8082),
				null,
				AppSettings.Get("timeoutAfterClientAccept", 2000));
		}

		protected override void OnStart(string[] args)
		{
			//EventLog.WriteEntry("Starting as a service...");
			_controller.StartProcessAsync(); 
		}

		protected override void OnStop()
		{
			//EventLog.WriteEntry("Stopping service...");
			_controller.StopProcess(); 
		}
	}
}

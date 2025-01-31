using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusRTUOverTCPGatewayService
{
	public class SerialPortLayer
	{
        readonly SerialPort serialPort;

        public int SendTimeout { get => serialPort.WriteTimeout; set => serialPort.WriteTimeout = value; }
        public int ReceiveTimeout { get => serialPort.ReadTimeout; set => serialPort.ReadTimeout = value; }
        public bool Connected => serialPort?.IsOpen ?? false;

        #region Constructors
        public SerialPortLayer(string portName)
            => serialPort = new SerialPort(portName);

        public SerialPortLayer(string portName, int baudRate)
            => serialPort = new SerialPort(portName, baudRate);

        public SerialPortLayer(string portName, int baudRate, Parity parity)
            => serialPort = new SerialPort(portName, baudRate, parity);

        public SerialPortLayer(string portName, int baudRate, Parity parity, int dataBits)
            => serialPort = new SerialPort(portName, baudRate, parity, dataBits);

        public SerialPortLayer(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
            => serialPort = new SerialPort(portName, baudRate, parity, dataBits);

        public SerialPortLayer(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, int? endmodbus)
            => serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);

        public SerialPortLayer(SerialPort SP)
            => serialPort = SP;
        #endregion

        internal void Close()
            => serialPort?.Close();

        public void Connect()
        {
            if (!serialPort.IsOpen)
                serialPort.Open();
        }

        internal Task ConnectAsync()
            => throw new NotImplementedException();

        internal bool IsEquivalent<T>(T other)
            => (other as SerialPortLayer).serialPort.PortName == serialPort.PortName
            && (other as SerialPortLayer).serialPort.Parity == serialPort.Parity
            && (other as SerialPortLayer).serialPort.DataBits == serialPort.DataBits
            && (other as SerialPortLayer).serialPort.StopBits == serialPort.StopBits
            && (other as SerialPortLayer).serialPort.BaudRate == serialPort.BaudRate;

        internal bool Blocks<T>(T other)
            => (other as SerialPortLayer).serialPort.PortName == serialPort.PortName;

        internal byte[] ReceiveSynced()
        {
            int length = serialPort.BytesToRead;
            var response = new byte[length];
            int readBytes = 0;
            while (readBytes < length)
                readBytes += serialPort.BaseStream.Read(response, readBytes, length - readBytes);
            return response;
        }

        internal void SendSynced(byte[] data)
        {
            if (!serialPort.IsOpen) serialPort.Open();
            serialPort.DiscardInBuffer();
            serialPort.Write(data, 0, data.Length);
        }
    }
}

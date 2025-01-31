using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModbusRTUOverTCPGatewayService
{
    public class Listener : IDisposable
    {
        /// <summary>
        /// A porta TCP que este objeto escuta.
        /// </summary>
        public int Port { get; }

        volatile bool _isListening;
        /// <summary>
        /// Indica se a porta TCP está sendo escutada no momento.
        /// </summary>
        public bool IsListening => _isListening;

        private readonly TcpListener listener;

        private System.Timers.Timer timeoutTimer;
        readonly int _timeout;

        public Listener(int port, int timeout)
        {
            Port = port;
            _timeout = timeout;
            listener = new TcpListener(IPAddress.Any, port);
            //listener.ExclusiveAddressUse = true;
            //listener.Server.ReceiveTimeout = 5000;
            //listener.Server.SendTimeout = 5000;
            //listener.Server.Blocking = true;
            //listener.Server.NoDelay = true;
        }

        private void TimeoutTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!_isListening) return;

            /// Se o timer associado a este evento não é o mesmo guardado nesta classe, é porque
            /// ele já foi descartado e substituído.
            if (!ReferenceEquals(timeoutTimer, sender))
                return;

            Stop();
            _isListening = false;
        }

        /// <summary>
        /// Inicia a escuta da porta TCP.
        /// </summary>
        internal void Start()
        {
            listener.Start();
            _isListening = true;

            if (_timeout > 0)
            {
                if (timeoutTimer != null) timeoutTimer.Dispose();
                timeoutTimer.Elapsed += TimeoutTimer_Elapsed;
                timeoutTimer.AutoReset = false;
                timeoutTimer.Start();
            }
        }

        /// <summary>
        /// Para a escuta da porta TCP.
        /// </summary>
        internal void Stop()
        {
            listener.Stop();
            _isListening = false;
        }

        internal TcpClient AcceptTcpClient()
            => listener.AcceptTcpClient();

        internal Task<TcpClient> AcceptTcpClientAsync()
            => listener.AcceptTcpClientAsync();

        public void Dispose()
        {
            listener?.Stop();
            timeoutTimer?.Dispose();
        } 
	}
}

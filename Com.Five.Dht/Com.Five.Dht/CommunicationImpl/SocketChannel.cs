namespace Com.Five.Dht.CommunicationImpl
{
    using Communication;
    using Communication.Requests;
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class SocketChannel : IChannel, IDisposable
    {
        ILog _l = LogManager.GetLogger(typeof(SocketChannel));

        AutoResetEvent _connectionComplete = new AutoResetEvent(false),
            _shutdown = new AutoResetEvent(false);

        Socket _socket;

        BinaryFormatter _formatter = new BinaryFormatter();

        public State State
        {
            get;
            private set;
        }

        public Uri Url
        {
            get;
            private set;
        }

        public SocketChannel(Uri url)
        {
            if(null == url)
            {
                throw new ArgumentNullException(nameof(url));
            }
            Url = url;
        }

        public void Open()
        {
            ThreadPool.QueueUserWorkItem(OpenSocketAndAcceptCalls);   
        }

        void OpenSocketAndAcceptCalls(object p)
        {
            _l.InfoFormat("Opening endpoint @{0}", Url);

            IPHostEntry entry = Dns.GetHostEntry(Url.Host);
            IPAddress ipAddress = entry.AddressList[0];
            IPEndPoint endpoint = new IPEndPoint(ipAddress, Url.Port);

            _socket = new Socket(AddressFamily.InterNetworkV6
                , SocketType.Stream, ProtocolType.IPv6);

            _socket.Bind(endpoint);
            _socket.Listen(100);

            _l.Info("Connection open and listening.");

            while(true)
            {
                _socket.BeginAccept(AcceptConnectionCallback,
                    _socket);

                int index = WaitHandle.WaitAny(
                    new[] { _connectionComplete, _shutdown });

                if (index == 1)
                {
                    _l.Info("Server shutting down.");
                    break;
                }
            }
        }

        async void AcceptConnectionCallback(IAsyncResult result)
        {
            Socket serverSocket = (Socket)result.AsyncState;

            Socket callerSocket = serverSocket.EndAccept(result);

            _l.InfoFormat("Accepted connection from {0}"
                , callerSocket.RemoteEndPoint);

            await Task.Factory.StartNew(() =>
            {
                NetworkStream stream = new NetworkStream(callerSocket);

                try
                {
                    Request req = (Request)_formatter.Deserialize(stream);
                    _l.InfoFormat("Request: {0}", req);
                }
                catch
                {
                    _l.Info("Invalid request type.");
                }

            });
        }

        #region IDisposable Support

        bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                _socket.Dispose();

                disposedValue = true;
            }
        }

         ~SocketChannel()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
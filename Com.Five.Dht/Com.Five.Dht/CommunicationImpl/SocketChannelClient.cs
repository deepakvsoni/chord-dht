namespace Com.Five.Dht.CommunicationImpl
{
    using Communication;
    using Communication.Requests;
    using Communication.Responses;
    using log4net;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Threading.Tasks;

    public class SocketChannelClient : IChannelClient, IDisposable
    {
        ILog _l = LogManager.GetLogger(typeof(SocketChannelClient));

        AutoResetEvent _clientConnected = new AutoResetEvent(false),
            _responseReceived = new AutoResetEvent(false);

        //TODO: Refactor message format type and handler.
        BinaryFormatter _formatter = new BinaryFormatter();

        Uri _serverUri;

        IPEndPoint _serverEndpoint;

        Socket _clientSocket;

        public SocketChannelClient(Uri serverUri)
        {
            _serverUri = serverUri;
        }

        public ConnectionState State
        {
            get;
            private set;
        }

        public bool Connect()
        {
            IPHostEntry entry = Dns.GetHostEntry(_serverUri.Host);
            IPAddress ipAddress = entry.AddressList[0];
            _serverEndpoint = new IPEndPoint(ipAddress, _serverUri.Port);

            _clientSocket = new Socket(AddressFamily.InterNetworkV6
                , SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs ea = new SocketAsyncEventArgs();
            ea.Completed += ClientConnected;
            ea.RemoteEndPoint = _serverEndpoint;

            if(!_clientSocket.ConnectAsync(ea))
            {
                ClientConnected(ea);
            }

            _clientConnected.WaitOne();
            return _clientSocket.Connected;
        }

        void ClientConnected(object sender, SocketAsyncEventArgs connectedEa)
        {
            ClientConnected(connectedEa);
        }
        
        void ClientConnected(SocketAsyncEventArgs connectedEa)
        {
            if(connectedEa.SocketError != SocketError.Success)
            {
                _clientConnected.Set();
                //TODO: Handle
                return;
            }
            State = ConnectionState.Connected;

            _clientConnected.Set();
        }

        public Task<Response> SendRequest(Request message)
        {
            if(State != ConnectionState.Connected)
            {
                throw new InvalidOperationException("Client not connected.");
            }
            return Task.Factory.StartNew<Response>(() =>
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    _formatter.Serialize(ms, message);

                    ms.Seek(0, SeekOrigin.Begin);

                    byte[] buffer = (byte[])ms.GetBuffer().Clone();

                    SocketAsyncEventArgs sendEa = new SocketAsyncEventArgs();
                    sendEa.SetBuffer(buffer, 0, buffer.Length);
                    sendEa.UserToken = _clientSocket;
                    sendEa.RemoteEndPoint = _serverEndpoint;
                    sendEa.Completed += SendRequestCompleted;

                    if(!_clientSocket.SendAsync(sendEa))
                    {
                        SendRequestCompleted(sendEa);
                    }

                    _responseReceived.WaitOne();
                }

                return null;
            });
        }

        void SendRequestCompleted(object sender, SocketAsyncEventArgs sendEa)
        {
            SendRequestCompleted(sendEa);
        }

        void SendRequestCompleted(SocketAsyncEventArgs sendEa)
        {
            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
            _clientSocket.Dispose();
            _clientSocket = null;
            _responseReceived.Set();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                if(null != _clientSocket)
                {
                    _clientSocket.Shutdown(SocketShutdown.Both);
                    _clientSocket.Close();
                    _clientSocket.Dispose();
                }

                disposedValue = true;
            }
        }

        ~SocketChannelClient()
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

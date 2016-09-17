namespace Com.Five.Dht.CommunicationImpl
{
    using Communication;
    using log4net;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    public class SocketChannelClient : IChannelClient, IDisposable
    {
        ILog _l = LogManager.GetLogger(typeof(SocketChannelClient));

        ManualResetEventSlim _clientConnected = new ManualResetEventSlim(false),
            _responseReceived = new ManualResetEventSlim(false);

        IChannelClientListener _listener;

        ConnectionState _state;

        Uri _serverUri;

        IPEndPoint _serverEndpoint;

        Socket _clientSocket;

        public ConnectionState State
        {
            get
            {
                return _state;
            }
            private set
            {
                _state = value;
                if(null != _listener)
                {
                    _listener.StateChange(value);
                }
            }
        }

        public SocketChannelClient(Uri serverUri)
        {
            if(null == serverUri)
            {
                throw new ArgumentNullException(nameof(serverUri));
            }
            _serverUri = serverUri;
        }

        public void RegisterChannelClientListener(
            IChannelClientListener listener)
        {
            if (null == listener)
            {
                throw new ArgumentNullException(nameof(listener));
            }
            _listener = listener;
        }
        
        public bool Connect()
        {
            _l.InfoFormat("Attempting to connect to {0}."
                , _serverUri);

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

            _clientConnected.Wait();
            _clientConnected.Reset();
            return _clientSocket.Connected;
        }

        void ClientConnected(object sender, SocketAsyncEventArgs connectedEa)
        {
            ClientConnected(connectedEa);
        }
        
        void ClientConnected(SocketAsyncEventArgs connectedEa)
        {
            if (connectedEa.SocketError != SocketError.Success)
            {
                _l.ErrorFormat("Error connecting to endpoint {0}.",
                    _serverUri);
                _clientConnected.Set();
                //TODO: Handle
                return;
            }
            State = ConnectionState.Connected;
            _l.InfoFormat("Connected to {0}, my Uri {1}.", _serverUri
                , _clientSocket.LocalEndPoint);

            _clientConnected.Set();
        }

        public Task<byte[]> SendRequest(byte[] message)
        {
            if(State != ConnectionState.Connected)
            {
                throw new InvalidOperationException("Client not connected.");
            }
            if(null == message)
            {
                throw new ArgumentNullException(nameof(message));
            }
            if(0 == message.Length)
            {
                throw new ArgumentException("Invalid message length.");
            }
            
            _l.InfoFormat("Sending request of length: {0}", message.Length);

            return Task.Factory.StartNew(() =>
            {
                Token token = new Token();
                token.Socket = _clientSocket;

                SocketAsyncEventArgs sendEa = new SocketAsyncEventArgs();
                sendEa.SetBuffer(message, 0, message.Length);
                sendEa.UserToken = token;
                sendEa.RemoteEndPoint = _serverEndpoint;
                sendEa.Completed += SendReceiveCompleted;

                if (!_clientSocket.SendAsync(sendEa))
                {
                    StartReceive(sendEa);
                }

                _responseReceived.Wait();
                _responseReceived.Reset();
                return token.Data;
            });
        }

        void SendReceiveCompleted(object sender, SocketAsyncEventArgs sendEa)
        {
            switch(sendEa.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessResponse(sendEa);
                    break;
                case SocketAsyncOperation.Send:
                    StartReceive(sendEa);
                    break;
            }
        }

        void StartReceive(SocketAsyncEventArgs sendEa)
        {
            _l.Info("Send request completed.");
            _l.Info("Receiving response from server.");
            if (sendEa.SocketError == SocketError.Success)
            {
                if (!_clientSocket.ReceiveAsync(sendEa))
                {
                    ProcessResponse(sendEa);
                }
            }
            else
            {
                ProcessError(sendEa);
            }
        }

        void ProcessResponse(SocketAsyncEventArgs readEa)
        {
            Token token = (Token)readEa.UserToken;

            if (readEa.SocketError == SocketError.Success)
            {
                if (readEa.BytesTransferred > 0)
                {
                    _l.InfoFormat("Received {0} bytes from {1}.",
                        readEa.BytesTransferred, token.Socket.LocalEndPoint);
                    token.TotalBytes += readEa.BytesTransferred;

                    byte[] bytes = new byte[readEa.BytesTransferred];
                    Array.Copy(readEa.Buffer, bytes, readEa.BytesTransferred);

                    token.BufferList.Add(new ArraySegment<byte>(
                        bytes, readEa.Offset, readEa.BytesTransferred));

                    if (token.Socket.Available == 0)
                    {
                        HandleResponse(token);
                    }
                    else if (!token.Socket.ReceiveAsync(readEa))
                    {
                        ProcessResponse(readEa);
                    }
                }
            }
            else
            {
                ProcessError(readEa);
            }
        }

        void HandleResponse(Token token)
        {
            _l.Info("Response received. Closing socket.");
            
            using(MemoryStream stream = new MemoryStream(token.TotalBytes))
            {
                foreach(ArraySegment<byte> segment in token.BufferList)
                {
                    stream.Write(segment.Array, segment.Offset, segment.Count);
                }

                stream.Seek(0, SeekOrigin.Begin);

                token.Data = stream.GetBuffer();
            }

            token.Socket.Shutdown(SocketShutdown.Both);
            token.Socket.Close();

            _responseReceived.Set();
        }

        void ProcessError(SocketAsyncEventArgs erroredEa)
        {
            _l.ErrorFormat("Error on socket: {0}", erroredEa.SocketError);

            Token token = erroredEa.UserToken as Token;

            IPEndPoint localEp = token.Socket.LocalEndPoint as IPEndPoint;

            _l.ErrorFormat("Socket error {0} on endpoint {1} during {2}."
                , erroredEa.SocketError, localEp, erroredEa.LastOperation);

            token.Socket.Shutdown(SocketShutdown.Both);
            token.Socket.Close();
        }

        #region IDisposable Support
        bool disposedValue ;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (null != _clientSocket)
                {
                    if (disposing)
                    {
                        try
                        {
                            _clientSocket.Shutdown(SocketShutdown.Both);
                        }
                        catch (Exception e)
                        {
                            _l.Error(e);
                        }
                        _clientSocket.Close();
                        _clientSocket.Dispose();
                    }
                    _clientConnected.Dispose();
                    _responseReceived.Dispose();
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

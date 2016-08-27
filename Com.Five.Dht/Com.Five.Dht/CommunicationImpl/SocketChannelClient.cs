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
            _l.InfoFormat("Sending request: {0}", message);

            return Task.Factory.StartNew<Response>(() =>
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    _formatter.Serialize(ms, message);

                    ms.Seek(0, SeekOrigin.Begin);

                    byte[] buffer = (byte[])ms.GetBuffer().Clone();

                    Token token = new Token();
                    token.Socket = _clientSocket;

                    SocketAsyncEventArgs sendEa = new SocketAsyncEventArgs();
                    sendEa.SetBuffer(buffer, 0, buffer.Length);
                    sendEa.UserToken = token;
                    sendEa.RemoteEndPoint = _serverEndpoint;
                    sendEa.Completed += SendReceiveCompleted;

                    if(!_clientSocket.SendAsync(sendEa))
                    {
                        StartReceive(sendEa);
                    }

                    _responseReceived.WaitOne();
                }
                return null;
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
            _l.Info("Sending request completed.");
            _l.Info("Received response from server.");
            if (sendEa.SocketError == SocketError.Success)
            {
                if (!_clientSocket.ReceiveAsync(sendEa))
                {
                    ProcessResponse(sendEa);
                }
            } else
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
                    token.BufferList.Add(new ArraySegment<byte>(readEa.Buffer, 0, readEa.BytesTransferred));
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
            token.Socket.Shutdown(SocketShutdown.Both);
            token.Socket.Close();
            _responseReceived.Set();
        }

        void ProcessError(SocketAsyncEventArgs erroredEa)
        {
            _l.InfoFormat("Error on socket: {0}", erroredEa.SocketError);

            Token token = erroredEa.UserToken as Token;

            IPEndPoint localEp = token.Socket.LocalEndPoint as IPEndPoint;

            _l.InfoFormat("Socket error {0} on endpoint {1} during {2}."
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
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                if(null != _clientSocket)
                {
                    try
                    {
                        _clientSocket.Shutdown(SocketShutdown.Both);
                        _clientSocket.Close();
                        _clientSocket.Dispose();
                    }
                    catch
                    {
                        //NOP.
                    }
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

namespace Com.Five.Dht.CommunicationImpl
{
    using Communication;
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    public class SocketChannel : IChannel, IDisposable
    {
        ILog _l = LogManager.GetLogger(typeof(SocketChannel));

        int _maxConnections = 15;

        AutoResetEvent _acceptComplete = new AutoResetEvent(false);
        CountdownEvent _countdownEvent = new CountdownEvent(1);

        Socket _socket;

        SocketAsyncEventArgsPool _pool;

        CancellationTokenSource _stopAcceptingToken
            = new CancellationTokenSource();

        IChannelListener _listener;

        object _lock = new object();

        List<Socket> _openSockets
            = new List<Socket>();

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
            if (null == url)
            {
                throw new ArgumentNullException(nameof(url));
            }
            Url = url;
            _pool = new SocketAsyncEventArgsPool(url.AbsolutePath
                , _maxConnections, ReceiveSendCompleted);
            _pool.Initialize();
        }

        public void RegisterChannelListener(IChannelListener listener)
        {
            if (null == listener)
            {
                throw new ArgumentNullException(nameof(listener));
            }
            _listener = listener;
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
                , SocketType.Stream, ProtocolType.Tcp);

            _socket.Bind(endpoint);
            State = State.Open;

            _socket.Listen(_maxConnections);
            State = State.Listening;

            _l.Info("Connection open and listening.");

            SocketAsyncEventArgs acceptEa = new SocketAsyncEventArgs();
            acceptEa.Completed += AcceptCompleted;

            while (!_stopAcceptingToken.IsCancellationRequested)
            {
                acceptEa.AcceptSocket = null;

                if (!_socket.AcceptAsync(acceptEa))
                {
                    ProcessAccept(acceptEa);
                }

                State = State.Accepting;

                WaitHandle.WaitAny(new[] { _stopAcceptingToken.Token.WaitHandle
                , _acceptComplete }, -1);
            }

            State = State.NotOpen;
            _l.InfoFormat("Shutting down channel {0}."
                , Url.AbsolutePath);

            _socket.Close();
            _socket.Dispose();
            _socket = null;

            _countdownEvent.Signal();

            _l.Info("Waiting for other open sockets to close.");

            if(!_countdownEvent.Wait(10000))
            {
                _l.Info("Wait for other open sockets timed out.");
            }

            ForceCloseOpenSockets();

            _l.Info("All connections closed. Bye!!.");
        }

        void AcceptCompleted(object sender, SocketAsyncEventArgs acceptEa)
        {
            ProcessAccept(acceptEa);
        }

        void ProcessAccept(SocketAsyncEventArgs acceptEa)
        {
            if (acceptEa.SocketError == SocketError.Success)
            {
                _l.InfoFormat("Accepting Socket connection from {0}.",
                    acceptEa.AcceptSocket.LocalEndPoint);

                SocketAsyncEventArgs readEa = _pool.Pop();
                ((Token)readEa.UserToken).Socket = acceptEa.AcceptSocket;

                lock (_lock)
                {
                    _openSockets.Add(acceptEa.AcceptSocket);
                }

                _countdownEvent.AddCount(1);

                if (!acceptEa.AcceptSocket.ReceiveAsync(readEa))
                {
                    ProcessReceive(readEa);
                }

                _acceptComplete.Set();
            }
            else
            {
                _l.ErrorFormat("Error accepting connection. SocketError: {0}"
                    , acceptEa.SocketError);
            }
        }

        void ReceiveSendCompleted(object sender, SocketAsyncEventArgs ioEa)
        {
            switch (ioEa.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(ioEa);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(ioEa);
                    break;
            }
        }

        void ProcessReceive(SocketAsyncEventArgs readEa)
        {
            Token token = (Token)readEa.UserToken;

            if (readEa.SocketError == SocketError.Success)
            {
                if (readEa.BytesTransferred > 0)
                {
                    _l.InfoFormat("Received {0} bytes from {1}."
                        , readEa.BytesTransferred, token.Socket.LocalEndPoint);
                    token.TotalBytes += readEa.BytesTransferred;
                    token.BufferList.Add(new ArraySegment<byte>(readEa.Buffer
                        , 0, readEa.BytesTransferred));
                    if (token.Socket.Available == 0)
                    {
                        HandleReceive(token);
                        _pool.Push(readEa);
                    }
                    else if (!token.Socket.ReceiveAsync(readEa))
                    {
                        ProcessReceive(readEa);
                    }
                }
            }
            else
            {
                _l.InfoFormat("Error receiving data from Socket {0}."
                    , token.Socket.LocalEndPoint);
                ProcessError(readEa);
            }
        }

        void HandleReceive(Token token)
        {
            _l.InfoFormat("Receive complete. Handling request from {0}."
                , token.Socket.LocalEndPoint);
            Task<byte[]> t = _listener.HandleRequest(this, token.TotalBytes,
                token.BufferList);

            byte[] response = t.Result;
            Send(token, response);
            return;
        }

        void Send(Token token, byte[] response)
        {
            _l.InfoFormat("Sending response to {0}.",
                token.Socket.LocalEndPoint);

            SocketAsyncEventArgs responseEa = _pool.Pop();
            responseEa.SetBuffer(response, 0, response.Length);
            responseEa.UserToken = token;

            token.Socket.SendAsync(responseEa);
        }

        void ProcessSend(SocketAsyncEventArgs sendEa)
        {
            Token token = sendEa.UserToken as Token;

            if (sendEa.SocketError == SocketError.Success)
            {
                _l.InfoFormat("Send response to {0} completed."
                    , token.Socket.LocalEndPoint);
                CloseSocket(token, sendEa);
            }
            else
            {
                _l.InfoFormat("Send response to {0} errored."
                    , token.Socket.LocalEndPoint);
                ProcessError(sendEa);
            }
        }

        void ProcessError(SocketAsyncEventArgs erroredEa)
        {
            Token token = erroredEa.UserToken as Token;

            IPEndPoint localEp = token.Socket.LocalEndPoint as IPEndPoint;

            _l.InfoFormat("Socket error {0} on endpoint {1} during {2}."
                , erroredEa.SocketError, localEp, erroredEa.LastOperation);

            CloseSocket(token, erroredEa);
        }

        void CloseSocket(Token token, SocketAsyncEventArgs ea)
        {
            _l.InfoFormat("Closing connection {0}."
                , token.Socket.LocalEndPoint);
            try
            {
                if (token.Socket.Connected)
                {
                    token.Socket.Shutdown(SocketShutdown.Both);
                }
            }
            catch (Exception e)
            {
                _l.Error("Error shutting down socket.", e);
            }
            token.Socket.Close();

            lock (_lock)
            {
                _openSockets.Remove(token.Socket);
            }

            _pool.Push(ea);

            _countdownEvent.Signal();
        }

        public void RequestClose()
        {
            _l.Info("Request recieved to shutdown channel.");
            if (!_stopAcceptingToken.IsCancellationRequested)
            {
                _stopAcceptingToken.Cancel();
            }
        }

        void ForceCloseOpenSockets()
        {
            foreach (var socket in _openSockets)
            {
                try
                {
                    _l.Info("Forcing close of socket.");

                    if (socket.Connected)
                    {
                        socket.Shutdown(SocketShutdown.Both);
                    }
                    socket.Close();
                }
                catch (Exception e)
                {
                    _l.Error("Error closing connection.", e);
                }
            }
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
                if (null != _socket)
                {
                    _socket.Close();
                    _socket.Dispose();
                }

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
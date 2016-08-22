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

    public class SocketChannel : IChannel, IDisposable
    {
        ILog _l = LogManager.GetLogger(typeof(SocketChannel));

        AutoResetEvent _acceptComplete = new AutoResetEvent(false);

        Socket _socket;

        SocketAsyncEventArgsPool _pool;

        //TODO: Refactor message format type and handler.
        BinaryFormatter _formatter = new BinaryFormatter();

        CancellationTokenSource _stopAcceptingToken 
            = new CancellationTokenSource();

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
            _pool = new SocketAsyncEventArgsPool(url.AbsolutePath
                , 15, ReceiveSendCompleted);
            _pool.Initialize();
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

            _socket.Listen(100);
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

            //TODO: Socket might be receiveing data. 
            //Handle scenario and support graceful shutdown.
            
            State = State.NotOpen;

            _l.InfoFormat("Shutting down endpoint socket {0}"
                , Url.AbsolutePath);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
            _socket.Dispose();
        }

        void AcceptCompleted(object sender, SocketAsyncEventArgs acceptEa)
        {
            ProcessAccept(acceptEa);
        }

        void ProcessAccept(SocketAsyncEventArgs acceptEa)
        {
            SocketAsyncEventArgs readEa = _pool.Pop();
            ((Token)readEa.UserToken).Socket = acceptEa.AcceptSocket;

            if(!acceptEa.AcceptSocket.ReceiveAsync(readEa))
            {
                ProcessReceive(readEa);
            }

            _acceptComplete.Set();
        }

        void ReceiveSendCompleted(object sender, SocketAsyncEventArgs readEa)
        {
            switch (readEa.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(readEa);
                    break;
                case SocketAsyncOperation.Send:
                    break;
            }
        }

        void ProcessReceive(SocketAsyncEventArgs readEa)
        {
            if(readEa.BytesTransferred > 0)
            {
                if(readEa.SocketError == SocketError.Success)
                {
                    Token token = (Token)readEa.UserToken;
                    token.TotalBytes += readEa.BytesTransferred;
                    token.BufferList.Add(new ArraySegment<byte>(readEa.Buffer, 0, readEa.BytesTransferred));
                    if(token.Socket.Available == 0)
                    {
                        HandleReceive(token);
                    }
                    else if(!token.Socket.ReceiveAsync(readEa))
                    {
                        ProcessReceive(readEa);
                    }
                }
            }
        }

        void HandleReceive(Token token)
        {
            Request req = null;
            using (MemoryStream ms = new MemoryStream(token.TotalBytes))
            {
                foreach (ArraySegment<byte> segment in token.BufferList)
                {
                    ms.Write(segment.Array, segment.Offset, segment.Count);
                }
                ms.Seek(0, SeekOrigin.Begin);
                req = (Request)_formatter.Deserialize(ms);
                _l.InfoFormat("Received qequest: {0}", req);
            }

            //TODO: Move to handler?
            if(req is Shutdown)
            {
                token.Socket.Shutdown(SocketShutdown.Both);
                token.Socket.Close();

                _stopAcceptingToken.Cancel();
            }
            //TODO: Handle request
        }

        void Send(Token token, Response response)
        {

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
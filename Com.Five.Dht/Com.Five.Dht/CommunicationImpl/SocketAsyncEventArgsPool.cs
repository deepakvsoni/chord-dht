namespace Com.Five.Dht.CommunicationImpl
{
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;

    public sealed class SocketAsyncEventArgsPool
    {
        ILog _l = LogManager.GetLogger(typeof(SocketAsyncEventArgsPool));

        object _lock = new object();

        Stack<SocketAsyncEventArgs> _pool;

        bool _initialized;

        int _capacity;

        EventHandler<SocketAsyncEventArgs> _completed;

        public string Id
        {
            get;
            private set;
        }

        public int Count
        {
            get
            {
                return _pool.Count;
            }
        }

        public SocketAsyncEventArgsPool(string id, int capacity
            , EventHandler<SocketAsyncEventArgs> completed)
        {
            Id = id;
            _capacity = capacity;
            _completed = completed;
            _pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        public void Initialize()
        {
            if (_initialized)
            {
                _l.DebugFormat("SocketAsyncEventArgs Pool {0} already initialized.", Id);
                return;
            }
            lock (_lock)
            {
                if (_initialized)
                {
                    _l.DebugFormat("SocketAsyncEventArgs Pool {0} already initialized.", Id);
                    return;
                }

                _l.InfoFormat("Initializing SocketAsyncEventArgs Pool {0}.", Id);

                _initialized = true;
                for (int i = 0; i < _capacity; ++i)
                {
                    SocketAsyncEventArgs ea = new SocketAsyncEventArgs();
                    ea.Completed += _completed;
                    ea.UserToken = new Token();
                    ea.SetBuffer(new byte[1024], 0, 1024);
                    _pool.Push(ea);
                }
                _l.InfoFormat("SocketAsyncEventArgs Pool {0} initialized.", Id);
            }
        }

        public SocketAsyncEventArgs Pop()
        {
            lock (_lock)
            {
                if (_pool.Count > 0)
                {
                    return _pool.Pop();
                }
            }
            _l.InfoFormat("SocketAsyncEventArgs Pool {0} has not available args object.", Id);
            return null;
        }

        public void Push(SocketAsyncEventArgs args)
        {
            if (null == args)
            {
                _l.ErrorFormat("Trying to push SocketAsyncEventArgs object into {0}.", Id);
                throw new ArgumentNullException(nameof(args));
            }
            lock (_lock)
            {
                _pool.Push(args);
            }
        }
    }
}

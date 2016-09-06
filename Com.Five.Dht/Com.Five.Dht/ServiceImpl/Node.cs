namespace Com.Five.Dht.ServiceImpl
{
    using Service;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Communication;
    using Data;

    public class Node : INode, IChannelListener, IDisposable
    {
        Id _id;
        IChannel _channel;
        IRequestHandler _requestHandler;
                
        public Node(Id id, IChannel channel
            , IDataEntries entries, IRequestHandler requestHandler)
        {
            if(null == id)
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (null == channel)
            {
                throw new ArgumentNullException(nameof(channel));
            }
            if (null == entries)
            {
                throw new ArgumentNullException(nameof(entries));
            }
            if(null == requestHandler)
            {
                throw new ArgumentNullException(nameof(requestHandler));
            }
            _id = id;
            _channel = channel;
            Entries = entries;
            _requestHandler = requestHandler;
            _channel.RegisterChannelListener(this);
        }

        public IChannel Channel
        {
            get
            {
                return _channel;
            }
        }

        public Id Id
        {
            get
            {
                return _id;
            }
        }

        public IDataEntries Entries
        {
            get;
            private set;
        }

        public INode Predecessor
        {
            get;
            set;
        }

        public ICollection<INode> Successors
        {
            get;
            private set;
        }

        public Task<byte[]> HandleRequest(int totalBytes
            , IList<ArraySegment<byte>> req)
        {
            return _requestHandler.Handle(totalBytes, req);
        }

        public void StateChange(State newState)
        {
            //TBD
        }

        public void HandleError(int errorCode)
        {
            //TBD
        }

        public void RequestShutdown()
        {
            _channel.RequestClose();
        }

        public void JoinRing(Uri uri)
        {
            throw new NotImplementedException();
        }

        public void CreateRing()
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support

        bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Channel.RequestClose();
                }

                disposedValue = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}

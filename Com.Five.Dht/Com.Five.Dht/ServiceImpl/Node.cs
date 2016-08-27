namespace Com.Five.Dht.ServiceImpl
{
    using Service;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Communication;
    using Data;

    public class Node : INode, IChannelListener
    {
        Id _id;
        IChannel _endpoint;
        IDataEntries _entries;
        IRequestHandler _requestHandler;
                
        public Node(Id id, IChannel endpoint, IDataEntries entries
            , IRequestHandler requestHandler)
        {
            if(null == id)
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (null == endpoint)
            {
                throw new ArgumentNullException(nameof(endpoint));
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
            _endpoint = endpoint;
            _entries = entries;
            _requestHandler = requestHandler;

            _endpoint.RegisterChannelListener(this);
        }

        public IChannel Endpoint
        {
            get
            {
                return _endpoint;
            }
        }

        public Id Id
        {
            get
            {
                return _id;
            }
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

        public Task<byte[]> HandleRequest(IChannel channel, int totalBytes
            , IList<ArraySegment<byte>> req)
        {
            return _requestHandler.Handle(channel, totalBytes, req);
        }
    }
}

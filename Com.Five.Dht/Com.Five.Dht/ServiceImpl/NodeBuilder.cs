namespace Com.Five.Dht.ServiceImpl
{
    using Communication;
    using CommunicationImpl;
    using Data;
    using DataImpl;
    using System;
    using System.Text;

    public class NodeBuilder
    {
        IDataEntries _dataEntries;

        IChannel _channel;

        IRequestHandler _requestHandler;

        IRequestResponseFormatter _requestResponseFormatter;

        Uri _uri, _bootstrapUri;

        public NodeBuilder SetUri(Uri uri)
        {
            _uri = uri;
            return this;
        }

        public NodeBuilder SetBootstrapUri(Uri bootstrapUri)
        {
            _bootstrapUri = bootstrapUri;
            return this;
        }

        public NodeBuilder SetRequestHandler(IRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
            return this;
        }

        public NodeBuilder SetRequestResponseFormatter(
            IRequestResponseFormatter requestResponseFormatter)
        {
            _requestResponseFormatter = requestResponseFormatter;
            return this;
        }


        public NodeBuilder SetDataEntries(IDataEntries dataEntries)
        {
            _dataEntries = dataEntries;
            return this;
        }

        public NodeBuilder SetChannel(IChannel channel)
        {
            _channel = channel;
            return this;
        }

        public Node Build()
        {
            if(null == _uri)
            {
                throw new InvalidOperationException("Uri not set.");
            }

            IDataEntries dataEntries = _dataEntries ?? new DataEntries();
            IChannel channel = _channel ?? new SocketChannel(_uri);
            IRequestResponseFormatter formatter = _requestResponseFormatter ??
                new RequestResponseBinaryFormatter();
            IRequestHandler requestHandler = _requestHandler ??
                new RequestHandler(formatter);

            Id id = new Id(Encoding.UTF8.GetBytes(_uri.AbsolutePath + ":"
                + _uri.Port));

            Node node = new Node(id, _bootstrapUri, channel, dataEntries
                , requestHandler);
            return node;
        }
    }
}

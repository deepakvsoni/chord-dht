namespace Com.Five.Dht.ServiceImpl
{
    using Communication;
    using Data;
    using Service;
    using Factory;
    using System;

    public class NodeBuilder
    {
        IDataEntries _dataEntries;

        IChannel _channel;

        IRequestHandler _requestHandler;

        IRequestResponseFormatter _requestResponseFormatter;

        IHashFunction _hashFunction;

        IdGenerator _idGenerator;

        IRingFactory _ringFactory;

        Uri _uri;

        public NodeBuilder SetUrl(Uri url)
        {
            _uri = url;
            return this;
        }

        public NodeBuilder SetHashFunction(IHashFunction hashFunction)
        {
            _hashFunction = hashFunction;
            return this;
        }

        public NodeBuilder SetRequestHandler(IRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
            return this;
        }

        public NodeBuilder SetRingFactory(IRingFactory ringFactory)
        {
            _ringFactory = ringFactory;
            return this;
        }

        public NodeBuilder SetRequestResponseFormatter(
            IRequestResponseFormatter requestResponseFormatter)
        {
            _requestResponseFormatter = requestResponseFormatter;
            return this;
        }

        public NodeBuilder SetIdGenerator(IdGenerator idGenerator)
        {
            _idGenerator = idGenerator;
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
            if(null==_ringFactory)
            {
                throw new InvalidOperationException("Factory not set.");
            }
            
            IHashFunction hashFunction = _hashFunction 
                ?? _ringFactory.HashFunction;

            IdGenerator idGenerator = _idGenerator ??
                _ringFactory.IdGenerator;
            IDataEntries dataEntries = _dataEntries ?? 
                _ringFactory.CreateDataEntries();
            IChannel channel = _channel ?? _ringFactory.CreateChannel(_uri);
            IRequestResponseFormatter formatter = _requestResponseFormatter ??
                _ringFactory.Formatter;
            IRequestHandler requestHandler = _requestHandler ??
                _ringFactory.CreateRequestHandler(formatter, idGenerator);

            Id id = idGenerator.Generate(_uri.AbsolutePath + ":"
                + _uri.Port);

            Node node = new Node(id, _ringFactory, channel, dataEntries
                , requestHandler);
            requestHandler.Node = node;
            return node;
        }
    }
}

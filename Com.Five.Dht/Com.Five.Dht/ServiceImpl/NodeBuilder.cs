namespace Com.Five.Dht.ServiceImpl
{
    using Communication;
    using CommunicationImpl;
    using Data;
    using DataImpl;
    using Service;
    using System;

    public class NodeBuilder
    {
        byte _maxNoOfBits;

        IDataEntries _dataEntries;

        IChannel _channel;

        IRequestHandler _requestHandler;

        IRequestResponseFormatter _requestResponseFormatter;

        IHashFunction _hashFunction;

        Uri _uri;

        public NodeBuilder SetMaxNoOfBits(byte maxNoOfBits)
        {
            _maxNoOfBits = maxNoOfBits;
            return this;
        }

        public NodeBuilder SetUri(Uri uri)
        {
            _uri = uri;
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
            byte maxNoOfBits = _maxNoOfBits <= 0 ?
               (byte)64 : _maxNoOfBits;

            IHashFunction hashFunction = _hashFunction 
                ?? new SHA1HashFunction();

            IdGenerator _idGenerator = new IdGenerator(maxNoOfBits,
                hashFunction);
            IDataEntries dataEntries = _dataEntries ?? new DataEntries();
            IChannel channel = _channel ?? new SocketChannel(_uri);
            IRequestResponseFormatter formatter = _requestResponseFormatter ??
                new RequestResponseBinaryFormatter();
            IRequestHandler requestHandler = _requestHandler ??
                new RequestHandler(formatter);

            Id id = _idGenerator.Generate(_uri.AbsolutePath + ":"
                + _uri.Port);

            Node node = new Node(id, channel, dataEntries
                , requestHandler);
            return node;
        }
    }
}

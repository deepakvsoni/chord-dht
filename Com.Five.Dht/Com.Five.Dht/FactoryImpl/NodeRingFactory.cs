namespace Com.Five.Dht.FactoryImpl
{
    using Communication;
    using CommunicationImpl;
    using Data;
    using DataImpl;
    using Service;
    using Factory;
    using System;
    using ServiceImpl;

    public class NodeRingFactory : IRingFactory
    {
        NodeClientBuilder _builder = new NodeClientBuilder();

        public NodeRingFactory()
            :this(Ring.Default.MaxNoOfBits)
        {
        }

        public NodeRingFactory(byte maxNoOfBits)
        {
            if(0 == maxNoOfBits)
            {
                throw new ArgumentException(
                    "maxNoOfBits should be greater than zero.");
            }
            Formatter = new RequestResponseBinaryFormatter();
            HashFunction = new SHA1HashFunction();
            IdGenerator = new IdGenerator(maxNoOfBits, HashFunction);
        }

        public IRequestResponseFormatter Formatter
        {
            get;
            private set;
        }

        public IHashFunction HashFunction
        {
            get;
            private set;
        }

        public IdGenerator IdGenerator
        {
            get;
            private set;
        }

        public INode CreateNode(Uri uri)
        {
            NodeBuilder builder = new NodeBuilder();
            builder.SetUrl(uri).SetRingFactory(this);

            INode node = builder.Build();

            return node;
        }

        public INodeClient CreateNodeClient(Uri serverUrl)
        {
            _builder.SetServerUrl(serverUrl).SetRingFactory(this);

            return _builder.Build();
        }

        public IChannel CreateChannel(Uri uri)
        {
            return new SocketChannel(uri);
        }

        public IDataEntries CreateDataEntries()
        {
            return new DataEntries();
        }

        public IRequestHandler CreateRequestHandler(
            IRequestResponseFormatter formatter
            , IdGenerator idGenerator)
        {
            return new RequestHandler(formatter, idGenerator);
        }

        public IChannelClient CreateChannelClient(Uri serverUrl)
        {
            return new SocketChannelClient(serverUrl);
        }
    }
}
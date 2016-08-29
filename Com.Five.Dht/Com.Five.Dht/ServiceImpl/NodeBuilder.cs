namespace Com.Five.Dht.ServiceImpl
{
    using Data;
    using Communication;
    using System;

    public class NodeBuilder
    {
        IDataEntries _dataEntries;

        IChannel _channel;

        Uri _uri;

        public NodeBuilder SetUri(Uri uri)
        {
            _uri = uri;
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
            return null;
        }
    }
}

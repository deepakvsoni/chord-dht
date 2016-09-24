namespace Com.Five.Dht.Factory
{
    using Communication;
    using Data;
    using Service;
    using System;

    public interface IRingFactory
    {
        IHashFunction HashFunction
        {
            get;
        }

        IdGenerator IdGenerator
        {
            get;
        }

        IRequestResponseFormatter Formatter
        {
            get;
        }

        IChannel CreateChannel(Uri url);

        IChannelClient CreateChannelClient(Uri serverUrl);

        IRequestHandler CreateRequestHandler(
            IRequestResponseFormatter formatter
            , IdGenerator idGenerator);

        IDataEntries CreateDataEntries();

        INode CreateNode(Uri url);

        INodeClient CreateNodeClient(Uri serverUrl);
    }
}

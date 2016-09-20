namespace Com.Five.Dht.ServiceImpl
{
    using Communication;
    using CommunicationImpl;
    using Service;
    using System;

    //TODO: Factory for the communication module would be better?
    public class NodeClientBuilder
    {
        Uri _serverUri;
        IChannelClient _channelClient;

        public NodeClientBuilder SetChannelClient(IChannelClient channelClient)
        {
            _channelClient = channelClient;
            return this;
        }

        public NodeClientBuilder SetServerUri(Uri serverUri)
        {
            _serverUri = serverUri;
            return this;
        }

        public NodeClient Build()
        {
            if(null == _serverUri)
            {
                throw new InvalidOperationException(
                    "Channel Client or Server Uri required.");
            }
            IChannelClient channelClient = _channelClient ??
                new SocketChannelClient(_serverUri);

            //TODO: Use in builder.
            IRequestResponseFormatter formatter
                = new RequestResponseBinaryFormatter();

            NodeClient client = new NodeClient(channelClient, formatter);

            return client;
        }
    }
}

namespace Com.Five.Dht.ServiceImpl
{
    using Communication;
    using CommunicationImpl;
    using System;

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
            if(null == _channelClient && null == _serverUri)
            {
                throw new InvalidOperationException(
                    "Channel Client or Server Uri required.");
            }
            IChannelClient channelClient = _channelClient ??
                new SocketChannelClient(_serverUri);

            NodeClient client = new NodeClient(channelClient);

            return client;
        }
    }
}

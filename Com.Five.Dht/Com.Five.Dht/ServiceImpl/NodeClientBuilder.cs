namespace Com.Five.Dht.ServiceImpl
{
    using Communication;
    using CommunicationImpl;
    using Service;
    using Service.Factory;
    using System;

    public class NodeClientBuilder
    {
        Uri _serverUrl;
        IRequestResponseFormatter _formatter;
        IChannelClient _channelClient;
        IRingFactory _ringFactory;

        public NodeClientBuilder SetChannelClient(IChannelClient channelClient)
        {
            _channelClient = channelClient;
            return this;
        }

        public NodeClientBuilder SetServerUrl(Uri serverUrl)
        {
            _serverUrl = serverUrl;
            return this;
        }

        public NodeClientBuilder SetRingFactory(IRingFactory ringFactory)
        {
            _ringFactory = ringFactory;
            return this;
        }

        public NodeClientBuilder SetRequestResponseFormatter(
            IRequestResponseFormatter formatter)
        {
            _formatter = formatter;
            return this;
        }

        public NodeClient Build()
        {
            if(null == _serverUrl)
            {
                throw new InvalidOperationException(
                    "Channel Client or Server Url required.");
            }
            if((null == _channelClient || null == _formatter) 
                && null == _ringFactory)
            {
                throw new InvalidOperationException("Ring Factory required.");
            }
            IChannelClient channelClient = _channelClient ??
                _ringFactory.CreateChannelClient(_serverUrl);

            IRequestResponseFormatter formatter = _formatter ??
                _ringFactory.Formatter;

            NodeClient client = new NodeClient(channelClient, formatter);

            return client;
        }
    }
}

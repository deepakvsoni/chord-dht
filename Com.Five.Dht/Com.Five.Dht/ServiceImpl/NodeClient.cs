namespace Com.Five.Dht.ServiceImpl
{
    using Communication;
    using Data;
    using Service;
    using System;
    using System.Threading.Tasks;

    public class NodeClient : INodeClient
    {
        public NodeClient(IChannelClient channelClient)
        {
            ChannelClient = channelClient;
        }

        public IChannelClient ChannelClient
        {
            get;
            private set;
        }

        public Id Id
        {
            get;
            private set;
        }

        public Task<object> Get(string key)
        {
            throw new NotImplementedException();
        }

        public Task<INode> GetSuccessor(Id id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Ping()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Put(string key, object val)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Remove(string key)
        {
            throw new NotImplementedException();
        }
    }
}

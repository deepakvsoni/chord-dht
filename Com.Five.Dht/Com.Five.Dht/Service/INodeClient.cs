namespace Com.Five.Dht.Service
{
    using Communication;
    using Data;
    using System;
    using System.Threading.Tasks;

    public interface INodeClient
    {
        IChannelClient ChannelClient { get; }
        
        Task<INodeInfo> GetSuccessor(Id id, Uri url);

        Task<bool> Put(string key, object val);

        Task<object> Get(string key);

        Task<bool> Remove(string key);

        Task<bool> Ping();

        Task<bool> Notify(Id id, Uri url);
    }
}

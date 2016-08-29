namespace Com.Five.Dht.Service
{
    using Communication;
    using Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface INodeClient
    {
        Id Id { get; }

        IChannelClient Endpoint { get; }

        Task<INode> GetSuccessor(Id id);

        Task<bool> Insert(Id id, string key, object val);

        Task<object> Lookup(Id id, string key);

        Task Remove(Id id, string key);
    }
}

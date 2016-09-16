﻿namespace Com.Five.Dht.Service
{
    using Communication;
    using Data;
    using System;
    using System.Threading.Tasks;

    public interface INodeClient
    {
        Id Id { get; }

        IChannelClient ChannelClient { get; }
        
        Task<INodeInfo> GetSuccessor(Id id, Uri url);

        Task<bool> Put(string key, object val);

        Task<object> Get(string key);

        Task<bool> Remove(string key);

        Task<bool> Ping();
    }
}

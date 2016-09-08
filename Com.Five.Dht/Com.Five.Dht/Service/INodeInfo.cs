namespace Com.Five.Dht.Service
{
    using Data;
    using System;
    
    public interface INodeInfo
    {
        Id Id
        {
            get;
        }

        Uri Uri
        {
            get;
        }
    }
}

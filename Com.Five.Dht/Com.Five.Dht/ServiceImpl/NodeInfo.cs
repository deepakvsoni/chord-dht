namespace Com.Five.Dht.ServiceImpl
{
    using System;
    using Data;
    using Service;

    [Serializable]
    public class NodeInfo : INodeInfo
    {
        public Id Id
        {
            get;
            set;
        }

        public Uri Uri
        {
            get;
            set;
        }
    }
}

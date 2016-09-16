namespace Com.Five.Dht.Communication.Responses
{
    using Service;
    using System;

    [Serializable]
    public class JoinResponse : Response
    {
        INodeInfo[] Nodes
        {
            get;
            set;
        }
    }
}
namespace Com.Five.Dht.Service.Responses
{
    using Service;
    using System;

    [Serializable]
    public class GetSuccessorResponse : Response
    {
        public INodeInfo NodeInfo
        {
            get;
            set;
        }
    }
}

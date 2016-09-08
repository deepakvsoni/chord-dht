namespace Com.Five.Dht.Communication.Responses
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

namespace Com.Five.Dht.Service.Requests
{
    using Data;
    using System;

    [Serializable]
    public class GetSuccessor : Request
    {
        public Id Id
        {
            get;
            set;
        }

        public Uri Url
        {
            get;
            set;
        }
    }
}

namespace Com.Five.Dht.Communication.Requests
{
    using Data;
    using System;

    [Serializable]
    public class Notify : Request
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

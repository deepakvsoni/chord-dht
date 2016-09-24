namespace Com.Five.Dht.Service.Requests
{
    using System;

    [Serializable]
    public class Remove : Request
    {
        public string Key
        {
            get;
            set;
        }
    }
}

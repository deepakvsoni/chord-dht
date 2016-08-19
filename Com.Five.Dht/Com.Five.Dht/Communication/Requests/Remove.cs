namespace Com.Five.Dht.Communication.Requests
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

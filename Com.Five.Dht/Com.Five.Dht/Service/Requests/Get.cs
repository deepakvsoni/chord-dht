namespace Com.Five.Dht.Service.Requests
{
    using System;

    [Serializable]
    public class Get : Request
    {
        public string Key
        {
            get;
            set;
        }
    }
}
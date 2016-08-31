namespace Com.Five.Dht.Communication.Requests
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
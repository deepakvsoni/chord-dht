namespace Com.Five.Dht.Communication.Requests
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class Put : Request
    {
        public string Key
        {
            get;
            set;
        }

        public object Value
        {
            get;
            set;
        }
    }
}

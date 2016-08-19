namespace Com.Five.Dht.Communication.Requests
{
    using System;

    [Serializable]
    public class Lookup : Request
    {
        string Key
        {
            get;
            set;
        }
    }
}
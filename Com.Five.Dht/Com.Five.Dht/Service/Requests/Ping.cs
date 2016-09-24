namespace Com.Five.Dht.Service.Requests
{
    using System;

    [Serializable]
    public class Ping : Request
    {
        public static readonly Ping I = new Ping();
    }
}

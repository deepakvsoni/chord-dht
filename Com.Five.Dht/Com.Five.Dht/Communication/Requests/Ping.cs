namespace Com.Five.Dht.Communication.Requests
{
    using System;

    [Serializable]
    public class Ping : Request
    {
        public static readonly Ping I = new Ping();
    }
}

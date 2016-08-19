namespace Com.Five.Dht.Communication.Requests
{
    using Data;
    using System;

    [Serializable]
    public abstract class Request
    {
        public Id Id { get; set; }
    }
}
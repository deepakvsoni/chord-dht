namespace Com.Five.Dht.Service.Responses
{
    using System;

    public enum Status
    {
        Ok,
        BadRequest,
        Failed
    }

    [Serializable]
    public abstract class Response
    {
        public Status Status
        {
            get;
            set;
        }
    }
}

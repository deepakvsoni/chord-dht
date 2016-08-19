namespace Com.Five.Dht.Communication.Responses
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

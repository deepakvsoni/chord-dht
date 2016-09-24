namespace Com.Five.Dht.Service.Responses
{
    using System;
    
    [Serializable]
    public class NotifyResponse : Response
    {
        public static readonly NotifyResponse I = new NotifyResponse
        {
            Status = Status.Ok
        };
    }
}

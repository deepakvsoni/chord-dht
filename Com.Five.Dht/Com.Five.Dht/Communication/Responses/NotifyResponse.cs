namespace Com.Five.Dht.Communication.Responses
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

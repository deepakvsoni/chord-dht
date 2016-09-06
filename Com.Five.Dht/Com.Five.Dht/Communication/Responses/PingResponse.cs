namespace Com.Five.Dht.Communication.Responses
{
    using System;
    [Serializable]
    public class PingResponse : Response
    {
        public static readonly PingResponse Alive = new PingResponse
        {
            Status = Status.Ok
        };
    }
}
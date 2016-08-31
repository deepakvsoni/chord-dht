namespace Com.Five.Dht.Communication.Responses
{
    using System;

    [Serializable]
    public class PutResponse : Response
    {
        public static readonly PutResponse Success 
            = new PutResponse { Status = Status.Ok };

        public static readonly PutResponse Failed
            = new PutResponse { Status = Status.Failed };
    }
}

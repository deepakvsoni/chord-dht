namespace Com.Five.Dht.Service.Responses
{
    using System;

    [Serializable]
    public class RemoveResponse : Response
    {
        public static readonly RemoveResponse Success
            = new RemoveResponse { Status = Status.Ok };

        public static readonly RemoveResponse Failed
            = new RemoveResponse { Status = Status.Failed };
    }
}

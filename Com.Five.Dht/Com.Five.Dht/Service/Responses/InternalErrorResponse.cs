namespace Com.Five.Dht.Service.Responses
{
    using System;

    [Serializable]
    public class InternalErrorResponse : Response
    {
        public static readonly InternalErrorResponse I = new InternalErrorResponse();
    }
}

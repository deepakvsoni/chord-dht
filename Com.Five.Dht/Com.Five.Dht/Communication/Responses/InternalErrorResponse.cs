namespace Com.Five.Dht.Communication.Responses
{
    using System;

    [Serializable]
    public class InternalErrorResponse : Response
    {
        public static readonly InternalErrorResponse I = new InternalErrorResponse();
    }
}

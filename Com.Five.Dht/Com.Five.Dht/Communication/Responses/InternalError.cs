namespace Com.Five.Dht.Communication.Responses
{
    using System;

    [Serializable]
    public class InternalError : Response
    {
        public static readonly InternalError I = new InternalError();
    }
}

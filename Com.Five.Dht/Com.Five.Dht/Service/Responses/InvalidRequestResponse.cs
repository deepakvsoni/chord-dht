namespace Com.Five.Dht.Service.Responses
{
    using System;

    [Serializable]
    public class InvalidRequestResponse : Response
    {
        public static readonly InvalidRequestResponse I = new InvalidRequestResponse();
    }
}

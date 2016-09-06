namespace Com.Five.Dht.Communication.Responses
{
    using System;

    [Serializable]
    public class InvalidRequestResponse : Response
    {
        public static readonly InvalidRequestResponse I = new InvalidRequestResponse();
    }
}

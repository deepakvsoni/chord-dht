namespace Com.Five.Dht.Communication.Responses
{
    using System;

    [Serializable]
    public class InvalidRequest : Response
    {
        public static readonly InvalidRequest I = new InvalidRequest();
    }
}

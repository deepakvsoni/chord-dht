namespace Com.Five.Dht.Communication.Responses
{
    using System;

    [Serializable]
    public class GetResponse : Response
    {
        public object Value
        {
            get;
            set;
        }
    }
}

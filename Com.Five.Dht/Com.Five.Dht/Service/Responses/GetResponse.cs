namespace Com.Five.Dht.Service.Responses
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

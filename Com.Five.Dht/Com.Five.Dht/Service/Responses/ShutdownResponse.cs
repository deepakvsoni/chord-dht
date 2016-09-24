namespace Com.Five.Dht.Service.Responses
{
    using System;

    [Serializable]
    public class ShutdownResponse : Response
    {
        public bool ShutdownAccepted
        {
            get;
            set;
        }
    }
}

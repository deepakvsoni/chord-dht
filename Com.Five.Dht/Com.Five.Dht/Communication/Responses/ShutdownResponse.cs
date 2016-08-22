namespace Com.Five.Dht.Communication.Responses
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

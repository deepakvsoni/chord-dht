namespace Com.Five.Dht.CommunicationImpl
{
    using Communication;
    using System;
    using System.Threading.Tasks;
    using Communication.Requests;
    using Communication.Responses;

    public class SocketChannelClient : IChannelClient
    {
        public ConnectionState State
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool Connect()
        {
            throw new NotImplementedException();
        }

        public Task<Response> SendRequest(Request message)
        {
            throw new NotImplementedException();
        }
    }
}

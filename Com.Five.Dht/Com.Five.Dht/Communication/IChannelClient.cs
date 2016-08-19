namespace Com.Five.Dht.Communication
{
    using Requests;
    using Responses;
    using System.Threading.Tasks;

    public enum ConnectionState
    {
        NotConnected,
        Connected,
        ConnectFailed
    }

    public interface IChannelClient
    {
        ConnectionState State { get; }

        bool Connect();

        Task<Response> SendRequest(Request message);
    }
}

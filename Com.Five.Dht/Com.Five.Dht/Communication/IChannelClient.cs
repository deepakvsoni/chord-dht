namespace Com.Five.Dht.Communication
{
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

        Task<byte[]> SendRequest(byte[] message);
    }
}

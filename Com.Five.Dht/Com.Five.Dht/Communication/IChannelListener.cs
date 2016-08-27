namespace Com.Five.Dht.Communication
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IChannelListener
    {
        Task<byte[]> HandleRequest(IChannel channel, int totalBytes
            , IList<ArraySegment<byte>> req);
    }
}

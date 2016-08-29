namespace Com.Five.Dht.Communication
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IChannelListener
    {
        void StateChange(State newState);

        void HandleError(int errorCode);
        
        Task<byte[]> HandleRequest(IChannel channel, int totalBytes
            , IList<ArraySegment<byte>> req);
    }
}

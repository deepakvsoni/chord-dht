namespace Com.Five.Dht.Communication
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IChannelClientListener
    {
        void StateChange(ConnectionState newState);

        void HandleError(int errorCode);
    }
}

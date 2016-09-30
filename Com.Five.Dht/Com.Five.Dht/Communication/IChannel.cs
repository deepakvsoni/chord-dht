namespace Com.Five.Dht.Communication
{
    using Service;
    using System;

    public enum State
    {
        NotOpen,
        Open,
        Listening,
        Accepting,
        NotAccepting,
        Error
    }

    public interface IChannel
    {
        Uri Url { get; }

        State State { get; }

        void Open();

        void RequestClose();

        void RegisterChannelListener(IChannelListener listener);
    }
}
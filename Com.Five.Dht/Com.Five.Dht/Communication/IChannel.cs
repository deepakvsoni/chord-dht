namespace Com.Five.Dht.Communication
{
    using System;

    public enum State
    {
        NotOpen,
        Open,
        Listening
    }

    public interface IChannel
    {
        Uri Url { get; }

        State State { get; }

        void Open();
    }
}
namespace Com.Five.Dht.Service
{
    using Communication;
    using Data;
    using System;
    using System.Collections.Generic;

    public interface INode
    {
        Id Id { get; }

        IChannel Channel { get; }

        IDataEntries Entries { get; }

        INode Predecessor { get; set; }

        ICollection<INode> Successors { get; }

        void JoinRing(Uri uri);

        void CreateRing();

        void RequestShutdown();
    }
}

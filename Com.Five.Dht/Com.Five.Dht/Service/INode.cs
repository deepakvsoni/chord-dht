namespace Com.Five.Dht.Service
{
    using Communication;
    using Data;
    using System.Collections.Generic;

    public interface INode
    {
        Id Id { get; }

        IChannel Channel { get; }

        IDataEntries Entries { get; }

        INode Predecessor { get; set; }

        ICollection<INode> Successors { get; }

        void RequestShutdown();
    }
}

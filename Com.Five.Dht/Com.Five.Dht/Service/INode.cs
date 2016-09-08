namespace Com.Five.Dht.Service
{
    using Communication;
    using Data;
    using System;
    using System.Collections.Generic;

    public interface INode
    {
        INodeInfo Info
        {
            get;
        }

        Id Id { get; }

        IChannel Channel { get; }

        IDataEntries Entries { get; }

        INodeInfo Predecessor { get; set; }

        SortedList<Id, INodeInfo> Successors { get; }

        void JoinRing(Uri uri);

        void Start();

        void RequestShutdown();
    }
}
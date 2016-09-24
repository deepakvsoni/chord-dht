namespace Com.Five.Dht.Service
{
    using Communication;
    using Data;
    using Factory;
    using System;
    using System.Collections.Generic;

    public interface INode
    {
        INodeInfo Info
        {
            get;
        }

        IRingFactory Factory { get; }

        Id Id { get; }

        IChannel Channel { get; }

        IDataEntries Entries { get; }

        INodeInfo Predecessor { get; set; }

        FingerTable FingerTable { get; }

        SortedList<Id, INodeInfo> Successors { get; }

        void JoinRing(Uri url);

        void Start();

        void RequestShutdown();
    }
}
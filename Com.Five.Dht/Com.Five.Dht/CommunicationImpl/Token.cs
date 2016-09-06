namespace Com.Five.Dht.CommunicationImpl
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;

    public sealed class Token
    {
        public Token()
        {
            BufferList = new List<ArraySegment<byte>>();
        }

        public int TotalBytes
        {
            get;
            set;
        }

        public IList<ArraySegment<byte>> BufferList
        {
            get;
            set;
        }

        public byte[] Data
        {
            get;
            set;
        }

        public Socket Socket
        {
            get;
            set;
        }
    }
}
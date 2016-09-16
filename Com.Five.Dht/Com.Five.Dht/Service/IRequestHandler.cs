namespace Com.Five.Dht.Service
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRequestHandler
    {
        INode Node { get; set; }

        Task<byte[]> Handle(int totalBytes
            , IList<ArraySegment<byte>> req);
    }
}

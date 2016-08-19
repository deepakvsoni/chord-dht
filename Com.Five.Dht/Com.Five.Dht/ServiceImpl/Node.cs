namespace Com.Five.Dht.ServiceImpl
{
    using Service;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Communication;
    using Data;

    public class Node : INode
    {
        Id _id;
        IChannel _endpoint;
        IDataEntries _entries;

        public Node(Id id, IChannel endpoint, IDataEntries entries)
        {
            if(null == id)
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (null == endpoint)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }
            if (null == entries)
            {
                throw new ArgumentNullException(nameof(entries));
            }
            _id = id;
            _endpoint = endpoint;
            _entries = entries;
        }

        public IChannel Endpoint
        {
            get
            {
                return _endpoint;
            }
        }

        public Id Id
        {
            get
            {
                return _id;
            }
        }

        public INode Predecessor
        {
            get;
            set;
        }
    }
}

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

    public class NodeClient : INodeClient
    {
        public IChannelClient Endpoint
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Id Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Task<bool> Insert(Id id, string key, object val)
        {
            throw new NotImplementedException();
        }

        public Task<object> Lookup(Id id, string key)
        {
            throw new NotImplementedException();
        }

        public Task Remove(Id id, string key)
        {
            throw new NotImplementedException();
        }
    }
}

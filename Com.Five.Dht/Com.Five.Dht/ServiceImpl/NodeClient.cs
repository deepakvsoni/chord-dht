namespace Com.Five.Dht.ServiceImpl
{
    using Communication;
    using Communication.Responses;
    using Data;
    using Service;
    using System;
    using System.Threading.Tasks;

    public class NodeClient : INodeClient
    {
        public NodeClient(IChannelClient channelClient,
            IRequestResponseFormatter formatter)
        {
            ChannelClient = channelClient;
            Formatter = formatter;
        }
        

        public IChannelClient ChannelClient
        {
            get;
            private set;
        }

        public IRequestResponseFormatter Formatter
        {
            get;
            private set;
        }

        public Task<object> Get(string key)
        {
            throw new NotImplementedException();
        }

        public async Task<INodeInfo> GetSuccessor(Id id, Uri url)
        {
            if (ChannelClient.Connect())
            {
                Communication.Requests.GetSuccessor getSuccessor
                    = new Communication.Requests.GetSuccessor
                    {
                        Id = id,
                        Url = url
                    };

                byte[] request = Formatter.GetBytes(getSuccessor);
                byte[] responseBytes 
                    = await ChannelClient.SendRequest(request);

                GetSuccessorResponse response 
                    = (GetSuccessorResponse)Formatter.GetObject(responseBytes);

                if(response.Status == Status.Ok)
                {
                    return response.NodeInfo;
                }
                return null;
            }
            return null;
        }

        public async Task<bool> Notify(Id id, Uri url)
        {
            if (ChannelClient.Connect())
            {
                Communication.Requests.Notify notify
                    = new Communication.Requests.Notify
                    {
                        Id = id,
                        Url = url
                    };

                byte[] request = Formatter.GetBytes(notify);
                byte[] responseBytes
                    = await ChannelClient.SendRequest(request);

                NotifyResponse response
                    = (NotifyResponse)Formatter.GetObject(responseBytes);

                return (response.Status == Status.Ok);
            }
            return false;
        }

        public Task<bool> Ping()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Put(string key, object val)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Remove(string key)
        {
            throw new NotImplementedException();
        }
    }
}

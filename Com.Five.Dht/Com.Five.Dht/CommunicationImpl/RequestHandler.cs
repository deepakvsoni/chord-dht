namespace Com.Five.Dht.CommunicationImpl
{
    using Communication;
    using Communication.Requests;
    using Communication.Responses;
    using log4net;
    using Service;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class RequestHandler : IRequestHandler
    {
        delegate Task<Response> Handler(Request req);

        ILog _l = LogManager.GetLogger(typeof(RequestHandler));

        IRequestResponseFormatter _formatter;

        Dictionary<Type, Handler> _handlers 
            = new Dictionary<Type, Handler>();

        public INode Node
        {
            get;
            set;
        }

        public RequestHandler(IRequestResponseFormatter formatter)
        {
            if(null == formatter)
            {
                throw new ArgumentNullException(nameof(formatter));
            }
            _formatter = formatter;

            _handlers[typeof(Shutdown)] = HandleShutdown;
            _handlers[typeof(Put)] = HandlePut;
            _handlers[typeof(Get)] = HandleGet;
            _handlers[typeof(Remove)] = HandleRemove;
        }

        public async Task<byte[]> Handle(int totalBytes
            , IList<ArraySegment<byte>> req)
        {
            Request reqObj = null;
            try
            {
                reqObj = (Request)_formatter.GetObject(totalBytes
                    , req);
                _l.InfoFormat("Received request: {0}", req);
            }
            catch (InvalidCastException e)
            {
                _l.Error("Invalid type of request received.", e);
            }
            Response res = null;
            if (null == reqObj)
            {
                res = InvalidRequest.I;
                return _formatter.GetBytes(res);
            }

            Handler handler;
            if (!_handlers.TryGetValue(reqObj.GetType(), out handler))
            {
                res = InvalidRequest.I;
                return _formatter.GetBytes(res);
            }

            res = await handler(reqObj);

            return _formatter.GetBytes(res);
        }

        Task<Response> HandleShutdown(Request request)
        {
            return Task.Factory.StartNew<Response>(() =>
            {
                Node.RequestShutdown();
                return new ShutdownResponse();
            });
        }

        async Task<Response> HandlePut(Request request)
        {
            //TODO: This should find the actual node.
            Put put = (Put)request;
            bool putSuccess = await Node.Entries.Put(put.Key, put.Value);

            if(putSuccess)
            {
                return PutResponse.Success;
            }
            return PutResponse.Failed;
        }

        async Task<Response> HandleGet(Request request)
        {
            Get get = (Get)request;
            object val = await Node.Entries.Get(get.Key);
            return new GetResponse { Status = Status.Ok, Value = val };
        }

        async Task<Response> HandleRemove(Request request)
        {
            Remove remove = (Remove)request;
            bool removed = await Node.Entries.Remove(remove.Key);
            return removed ? RemoveResponse.Success 
                : RemoveResponse.Failed;
        }
    }
}
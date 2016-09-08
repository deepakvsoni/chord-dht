namespace Com.Five.Dht.ServiceImpl
{
    using Communication.Requests;
    using Communication.Responses;
    using Data;
    using log4net;
    using Service;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
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
            _handlers[typeof(Ping)] = HandlePing;
            _handlers[typeof(Join)] = HandleJoin;
            _handlers[typeof(GetSuccessor)] = HandleGetSuccessor;
        }

        public async Task<byte[]> Handle(int totalBytes
            , IList<ArraySegment<byte>> req)
        {
            Request reqObj = null;
            Response res = null;
            try
            {
                reqObj = (Request)_formatter.GetObject(totalBytes
                    , req);
                _l.InfoFormat("Received request: {0}", reqObj);
            }
            catch (InvalidCastException e)
            {
                _l.Error("Invalid type of request received.", e);

                res = InvalidRequestResponse.I;
                return _formatter.GetBytes(res);
            }
            catch(SerializationException ser)
            {
                _l.Error("Empty request received.", ser);

                res = InvalidRequestResponse.I;
                return _formatter.GetBytes(res);
            }
            catch (Exception e2)
            {
                _l.Error("Unknown error.", e2);
                res = InternalErrorResponse.I;
                return _formatter.GetBytes(res);
            }

            Handler handler;
            if (!_handlers.TryGetValue(reqObj.GetType(), out handler))
            {
                res = InvalidRequestResponse.I;
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

        Task<Response> HandlePing(Request request)
        {
            return Task.Factory.StartNew<Response>(() =>
            {
                return PingResponse.Alive;
            });
        }

        Task<Response> HandleJoin(Request request)
        {
            return Task.Factory.StartNew<Response>(() =>
            {
                return new JoinResponse();
            });
        }

        Task<Response> HandleGetSuccessor(Request request)
        {
            return Task.Factory.StartNew<Response>(() =>
            {
                GetSuccessor req = (GetSuccessor)request;

                //This is the only node in the ring.
                if (0 == Node.Successors.Count)
                {
                    return new GetSuccessorResponse
                    {
                        NodeInfo = Node.Info
                    };
                }
                /*
                 * Get the first node in the list whose Id is greater than
                 * the requesting node and return its successor.
                 */
                foreach (Id nodeId in Node.Successors.Keys)
                {
                    if (nodeId  > req.Id)
                    {
                        return new GetSuccessorResponse
                        {
                            NodeInfo = Node.Successors[nodeId]
                        };
                    }
                }
                /*
                 * No node found whose Id is greater the requesting node's Id.
                 * Returning the first node to wrap around the ring.
                 */ 
                return new GetSuccessorResponse
                {
                    NodeInfo = Node.Successors[Node.Successors.Keys[0]]
                };
            });
        }
    }
}
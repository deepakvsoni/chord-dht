namespace Com.Five.Dht.ServiceImpl
{
    using Common;
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

        Dictionary<Type, Handler> _handlers = new Dictionary<Type, Handler>();

        IdGenerator _idGenerator;

        public INode Node
        {
            get;
            set;
        }

        public RequestHandler(IRequestResponseFormatter formatter,
            IdGenerator idGenerator)
        {
            if (null == formatter)
            {
                throw new ArgumentNullException(nameof(formatter));
            }
            if(null == idGenerator)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }
            _formatter = formatter;
            _idGenerator = idGenerator;

            _handlers[typeof(Shutdown)] = HandleShutdown;
            _handlers[typeof(Put)] = HandlePut;
            _handlers[typeof(Get)] = HandleGet;
            _handlers[typeof(Remove)] = HandleRemove;
            _handlers[typeof(Ping)] = HandlePing;
            _handlers[typeof(GetSuccessor)] = HandleGetSuccessor;
            _handlers[typeof(Notify)] = HandleNotify;
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
                _l.DebugFormat("Received request: {0}", reqObj);
            }
            catch (InvalidCastException e)
            {
                _l.Error("Invalid type of request received.", e);

                res = InvalidRequestResponse.I;
                return _formatter.GetBytes(res);
            }
            catch (SerializationException ser)
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
            _l.Debug("Received a request to SHUTDOWN.");
            return Task.Factory.StartNew<Response>(() =>
            {
                Node.RequestShutdown();
                return new ShutdownResponse();
            });
        }

        async Task<INodeInfo> GetSuccessorNodeForId(Id id, Uri url)
        {
            //This is the only node in the ring.
            if (0 == Node.Successors.Count)
            {
                _l.Debug(
                    "Don't have any successors returning myself as successor.");
                return Node.Info;
            }
            /*
             * Checking if Requestor is in between me and my
             * successor or between my successors.
             */
            Id currentId = Node.Id;
            foreach (Id successorId in Node.Successors.Keys)
            {
                if (id.Bytes.IsBetween(currentId.Bytes
                    , successorId.Bytes))
                {
                    _l.Debug("Found successor in my successors list.");
                    return Node.Successors[successorId];
                }

                currentId = successorId;
            }

            _l.Info(
                "Searching for predecessor node in Finger table and pass request to it.");
            /*
             * Get the predecessor from finger table and send request
             * to it.
             */
            INodeInfo predecessor = Node.FingerTable.GetClosestPredecessor(id);

            //To avoid recursive calls to self.
            if(predecessor.Id == Node.Id)
            {
                return predecessor;
            }
            
            INodeClient client 
                = RingContext.Current.Factory.CreateNodeClient(predecessor.Url);

            INodeInfo successorFromPredecessor 
                = await client.GetSuccessor(id, url);

            /*
             * If predecessor could not return the successor then
             * use current node as successor.
             */
            if (null == successorFromPredecessor)
            {
                successorFromPredecessor = Node.Info;
            }

            return successorFromPredecessor;
        }

        async Task<Response> HandlePut(Request request)
        {
            Put put = (Put)request;

            _l.DebugFormat("Received request to PUT for key {0}.", put.Key);

            INodeInfo handlerNode = await GetSuccessorNodeForId(
                _idGenerator.Generate(put.Key), Node.Info.Url);
            if (null != handlerNode && handlerNode.Id != Node.Id)
            {
                _l.DebugFormat("Relaying PUT request to {0}."
                    , handlerNode.Url);
                
                INodeClient client 
                    = RingContext.Current.Factory.CreateNodeClient(handlerNode.Url);
                if(await client.Put(put.Key, put.Value))
                {
                    return PutResponse.Success;
                }
                return PutResponse.Failed;
            }

            _l.DebugFormat("Saving {0} at {1}", put.Value, put.Key);

            bool putSuccess = await Node.Entries.Put(put.Key, put.Value);

            if (putSuccess)
            {
                return PutResponse.Success;
            }
            return PutResponse.Failed;
        }

        async Task<Response> HandleGet(Request request)
        {
            Get get = (Get)request;

            _l.DebugFormat("Received a GET request for key {0}.", get.Key);

            INodeInfo handlerNode = await GetSuccessorNodeForId(
               _idGenerator.Generate(get.Key), Node.Info.Url);
            if (null != handlerNode &&
                handlerNode.Id != Node.Id)
            {
                _l.DebugFormat("Relaying GET request to {0}."
                    , handlerNode.Url);
                
                INodeClient client =
                    RingContext.Current.Factory.CreateNodeClient(handlerNode.Url);

                object value = await client.Get(get.Key);

                return new GetResponse { Status = Status.Ok, Value = value };
            }

            object val = await Node.Entries.Get(get.Key);
            return new GetResponse { Status = Status.Ok, Value = val };
        }

        async Task<Response> HandleRemove(Request request)
        {
            Remove remove = (Remove)request;

            _l.DebugFormat("Received a REMOVE request for key {0}.",
                remove.Key);

            INodeInfo handlerNode = await GetSuccessorNodeForId(
               _idGenerator.Generate(remove.Key), Node.Info.Url);
            if (null != handlerNode &&
                handlerNode.Id != Node.Id)
            {
                _l.DebugFormat("Relaying REMOVE request to {0}."
                    , handlerNode.Url);
                
                INodeClient client =
                    RingContext.Current.Factory.CreateNodeClient(handlerNode.Url);
                if(await client.Remove(remove.Key))
                {
                    return RemoveResponse.Success;
                }

                return RemoveResponse.Failed;
            }

            bool removed = await Node.Entries.Remove(remove.Key);
            return removed ? RemoveResponse.Success
                : RemoveResponse.Failed;
        }

        Task<Response> HandlePing(Request request)
        {
            _l.Debug("Received a PING request.");
            return Task.Factory.StartNew<Response>(() =>
            {
                return PingResponse.Alive;
            });
        }

        Task<Response> HandleNotify(Request request)
        {
            return Task.Factory.StartNew<Response>(() => {
                Notify notify = (Notify)request;

                _l.DebugFormat("Received a NOTIFY request from {0}."
                    , notify.Url);

                /*
                 * If I don't have a predecessor then set notifying node as
                 * predecessor.
                 * Or If the notifying node is between my predecessor and me 
                 * then set it as my predecessor.
                 */
                if (null == Node.Predecessor 
                || notify.Id.Bytes.IsBetween(Node.Predecessor.Id.Bytes,
                    Node.Id.Bytes))
                {
                    _l.InfoFormat("Updating my predecessor to {0}."
                        , notify.Url);
                    Node.Predecessor = new NodeInfo
                    {
                        Id = notify.Id,
                        Url = notify.Url
                    };
                }
                return NotifyResponse.I;
            });
        }

        async Task<Response> HandleGetSuccessor(Request request)
        {
            try
            {
                GetSuccessor getSuccessor = (GetSuccessor)request;

                _l.DebugFormat("Received a GET SUCCESSOR request from {0}."
                    , getSuccessor.Url);

                INodeInfo successor = await GetSuccessorNodeForId(
                    getSuccessor.Id, getSuccessor.Url);

                return new GetSuccessorResponse
                {
                    NodeInfo = successor
                };
            }
            catch (Exception e)
            {
                _l.Error(e);
                return InternalErrorResponse.I;
            }
        }
    }
}
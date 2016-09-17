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

        Dictionary<Type, Handler> _handlers
            = new Dictionary<Type, Handler>();

        public INode Node
        {
            get;
            set;
        }

        public RequestHandler(IRequestResponseFormatter formatter)
        {
            if (null == formatter)
            {
                throw new ArgumentNullException(nameof(formatter));
            }
            _formatter = formatter;

            _handlers[typeof(Shutdown)] = HandleShutdown;
            _handlers[typeof(Put)] = HandlePut;
            _handlers[typeof(Get)] = HandleGet;
            _handlers[typeof(Remove)] = HandleRemove;
            _handlers[typeof(Ping)] = HandlePing;
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

        async Task<Response> HandlePut(Request request)
        {

            //TODO: This should find the actual node.
            Put put = (Put)request;

            _l.DebugFormat("Received request to PUT at key {0}.", put.Key);

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

            object val = await Node.Entries.Get(get.Key);
            return new GetResponse { Status = Status.Ok, Value = val };
        }

        async Task<Response> HandleRemove(Request request)
        {
            Remove remove = (Remove)request;

            _l.DebugFormat("Received a REMOVE request for key {0}.",
                remove.Key);

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

        Task<Response> HandleGetSuccessor(Request request)
        {
            return Task.Factory.StartNew<Response>(() =>
            {
                try
                {
                    GetSuccessor getSuccessor = (GetSuccessor)request;

                    _l.DebugFormat("Received a GET SUCCESSOR request from {0}."
                        , getSuccessor.Url);


                    //This is the only node in the ring.
                    if (0 == Node.Successors.Count)
                    {
                        _l.Debug(
                            "Don't have any successors returning myself as successor.");
                        NodeInfo info = new NodeInfo
                        {
                            Id = getSuccessor.Id,
                            Uri = getSuccessor.Url
                        };
                        Node.Successors.Add(info.Id, info);
                        Node.FingerTable.AddEntry(info);

                        return new GetSuccessorResponse
                        {
                            NodeInfo = Node.Info
                        };
                    }
                    /*
                     * Checking if Requestor is to be placed in between me and my
                     * successor or between my successors.
                     */
                    Id currentId = Node.Id;
                    foreach (Id successorId in Node.Successors.Keys)
                    {
                        //if (currentId <= getSuccessor.Id &&
                        //    getSuccessor.Id <= successorId)
                        if(getSuccessor.Id.Bytes.IsBetween(currentId.Bytes
                            , successorId.Bytes))
                        {
                            _l.Debug("Found successor in my successors list.");
                            NodeInfo info = new NodeInfo
                            {
                                Id = getSuccessor.Id,
                                Uri = getSuccessor.Url
                            };
                            /*
                             * Adding this as my successor as this is between and
                             * largest successor
                             */
                            Node.Successors.Add(info.Id, info);
                            Node.FingerTable.AddEntry(info);

                            /*
                             * If number of successors exceeds the capacity remove
                             * the last successor
                             */
                            if (Node.Successors.Count > Node.Successors.Capacity)
                            {
                                INodeInfo nodeToBeRemoved
                                    = Node.Successors[Node.Successors.Keys[
                                        Node.Successors.Count]];

                                _l.DebugFormat(
                                    "Removing node {0} from successors list as the number of successors exceeds capacity.",
                                    nodeToBeRemoved.Uri);
                                Node.Successors.Remove(nodeToBeRemoved.Id);
                            }
                            return new GetSuccessorResponse
                            {
                                NodeInfo = Node.Successors[Node.Successors.Keys[0]]
                            };
                        }

                        currentId = successorId;
                    }

                    _l.Info("Searching for predecessor node in Finger table and pass request to it.");
                    /*
                     * Get the predecessor from finger table and send request
                     * to it.
                     */
                    INodeInfo predecessor 
                        = Node.FingerTable.GetClosestPredecessor(
                            getSuccessor.Id);

                    NodeClientBuilder builder = new NodeClientBuilder();
                    builder.SetServerUri(predecessor.Uri);

                    INodeClient client = builder.Build();

                    Task<INodeInfo> successorTask
                        = client.GetSuccessor(getSuccessor.Id
                            , getSuccessor.Url);

                    INodeInfo successorFromPredecessor = successorTask.Result;

                    /*
                     * If predecessor could not return the successor then
                     * use current node as successor.
                     */ 
                    if (null == successorFromPredecessor)
                    {
                        successorFromPredecessor = Node.Info;
                    }

                    /*
                     * No node found whose Id is greater the requesting node's Id.
                     * Returning the first node to wrap around the ring.
                     */
                    return new GetSuccessorResponse
                    {
                        NodeInfo = successorFromPredecessor
                    };
                }
                catch (Exception e)
                {
                    _l.Error(e);
                    return InternalErrorResponse.I;
                }
            });
        }
    }
}
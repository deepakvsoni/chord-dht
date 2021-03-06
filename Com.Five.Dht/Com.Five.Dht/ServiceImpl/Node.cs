﻿namespace Com.Five.Dht.ServiceImpl
{
    using Service;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Communication;
    using Data;
    using System.Threading;
    using log4net;
    using Factory;

    public class Node : INode, IChannelListener, IDisposable
    {
        ILog _l;

        Id _id;
        IChannel _channel;
        IRequestHandler _requestHandler;
        AutoResetEvent _channelAccepting = new AutoResetEvent(false);
        AutoResetEvent _channelClosed = new AutoResetEvent(false);

        public Node(Id id, IRingFactory ringFactory, IChannel channel
            , IDataEntries entries, IRequestHandler requestHandler)
        {
            if(null == id)
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (null == channel)
            {
                throw new ArgumentNullException(nameof(channel));
            }
            if (null == entries)
            {
                throw new ArgumentNullException(nameof(entries));
            }
            if(null == requestHandler)
            {
                throw new ArgumentNullException(nameof(requestHandler));
            }
            _l = LogManager.GetLogger(string.Format("[Node {0}]"
                , channel.Url));

            _id = id;
            _channel = channel;
            _requestHandler = requestHandler;

            Entries = entries;
            Factory = ringFactory;

            _channel.RegisterChannelListener(this);

            Info = new NodeInfo { Id = id, Url = channel.Url };

            FingerTable = new FingerTable(Info);
            Successors = new SortedList<Id, INodeInfo>(
                RingContext.Current.NoOfSuccessors);
        }

        public IRingFactory Factory
        {
            get;
            private set;
        }

        public Id Id
        {
            get
            {
                return _id;
            }
        }

        public INodeInfo Info
        {
            get;
            private set;
        }

        public IChannel Channel
        {
            get
            {
                return _channel;
            }
        }


        public IDataEntries Entries
        {
            get;
            private set;
        }

        public INodeInfo Predecessor
        {
            get;
            set;
        }

        public FingerTable FingerTable
        {
            get;
            private set;
        }

        public SortedList<Id, INodeInfo> Successors
        {
            get;
            private set;
        }

        public Task<byte[]> HandleRequest(int totalBytes
            , IList<ArraySegment<byte>> req)
        {
            return _requestHandler.Handle(totalBytes, req);
        }

        public void StateChange(State newState)
        {
            if (newState == State.Accepting)
            {
                _channelAccepting.Set();
            }
            if(newState == State.NotOpen)
            {
                _channelClosed.Set();
            }
        }

        public void HandleError(int errorCode)
        {
            //TBD
        }

        public void RequestShutdown()
        {
            _channel.RequestClose();

            _channelClosed.WaitOne(10000);
        }

        public void JoinRing(Uri url)
        {
            Start();

            Successors.Clear();

            DoJoinRing(url);
        }

        void DoJoinRing(Uri url)
        {
            _l.InfoFormat("Joining ring {0}", url);
            
            INodeClient client = RingContext.Current.Factory.CreateNodeClient(url);

            _l.DebugFormat("Getting my successor from {0}", url);

            Task<INodeInfo> successorNodeTask = client.GetSuccessor(Id
                , Channel.Url);
            try
            {
                INodeInfo successorNode = successorNodeTask.Result;
                if(null == successorNode)
                {
                    _l.Debug("Null successor node, shutting down.");
                    RequestShutdown();
                    return;
                }
                _l.DebugFormat("My successor {0}", successorNode.Url);

                Successors.Add(successorNode.Id, successorNode);

                
                _l.Debug("Notifying successor about I being new predecessor.");
                INodeClient successorClient
                    = RingContext.Current.Factory.CreateNodeClient(
                        successorNode.Url);

                Task<bool> notifyTask 
                    = successorClient.Notify(Id, Channel.Url);

                if (!notifyTask.Result)
                {
                    _l.Error("Error notifying successor.");
                    //TODO: Should I shutdown if error notifying?
                    RequestShutdown();
                }
                
            }
            catch (Exception e)
            {
                _l.Error("Error getting joining ring, shutting down.", e);
                RequestShutdown();
            }

        }

        public void Start()
        {
            _l.InfoFormat("Starting node {0}.", _channel.Url);

            _channel.Open();

            if (!_channelAccepting.WaitOne(15000))
            {
                _l.DebugFormat("Wait for channel {0} to open timed out."
                    , _channel.Url);
            }
            _l.DebugFormat("State of channel {0}: {1}", _channel.Url
                , _channel.State);

            if(_channel.State != State.Accepting)
            {
                _l.DebugFormat(
                    "Requesting close of channel {0} incase it opens later."
                    , _channel.Url);
                _channel.RequestClose();

                throw new ApplicationException("Could not open channel.");
            }
            _l.InfoFormat("Node {0} started", _channel.Url);

            Successors.Add(Id, Info);
        }

        #region IDisposable Support

        bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Channel.RequestClose();
                }

                disposedValue = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
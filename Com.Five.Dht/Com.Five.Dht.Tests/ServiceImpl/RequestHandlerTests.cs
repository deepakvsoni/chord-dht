namespace Com.Five.Dht.Tests.ServiceImpl
{
    using Dht.Service.Requests;
    using Dht.Service.Responses;
    using Dht.Data;
    using Dht.DataImpl;
    using Dht.Service;
    using Dht.Factory;
    using Dht.ServiceImpl;
    using Dht.FactoryImpl;
    using FluentAssertions;
    using NSubstitute;
    using NUnit.Framework;
    using Service;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [TestFixture]
    public class RequestHandlerTests
    {
        [Serializable]
        class UnknownRequest: Request
        {

        }

        IRequestResponseFormatter _formatter;

        IRingFactory _sRingFactory = Substitute.For<IRingFactory>();

        ArraySegment<byte> _shutdownRequestBytes, _putRequestBytes, _getRequestBytes
            , _removeRequestBytes, _invalidRequestBytes, _pingRequestBytes
            , _unknownRequestBytes;

        byte[] _internalErrorResponseBytes;
        IdGenerator _idGenerator = new IdGenerator(8,
            new SHA1HashFunction());

        IList<ArraySegment<byte>> GetArray(
            params ArraySegment<byte>[] bytes)
        {
            return new List<ArraySegment<byte>>(bytes);    
        }

        [SetUp]
        public void Setup()
        {
            _formatter = new RequestResponseBinaryFormatter();

            _shutdownRequestBytes = new ArraySegment<byte>(
                _formatter.GetBytes(new Shutdown()));
            _putRequestBytes = new ArraySegment<byte>(
                _formatter.GetBytes(new Put { Key = "123", Value = "123" }));
            _getRequestBytes = new ArraySegment<byte>(
                _formatter.GetBytes(new Get { Key = "123" }));
            _removeRequestBytes = new ArraySegment<byte>(
                _formatter.GetBytes(new Remove { Key = "123" }));
            _pingRequestBytes = new ArraySegment<byte>(
                _formatter.GetBytes(Ping.I));
            
            _unknownRequestBytes = new ArraySegment<byte>(
                _formatter.GetBytes(new UnknownRequest()));

            _invalidRequestBytes = new ArraySegment<byte>(
                _formatter.GetBytes(new object()));

            _internalErrorResponseBytes 
                = _formatter.GetBytes(new InternalErrorResponse());
        }

        [Category("Unit")]
        [Test]
        public void RequestHandler_Construct()
        {
            Action a = () => new RequestHandler(_formatter, _idGenerator);
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void RequestHandler_ConstructNullFormatter()
        {
            Action a = () => new RequestHandler(null, _idGenerator);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void RequestHandler_ConstructNullIdGenerator()
        {
            Action a = () => new RequestHandler(_formatter, null);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public async Task RequestHandler_InvalidRequest()
        {
            INode node = Substitute.For<INode>();

            RequestHandler reqHandler = new RequestHandler(_formatter
                , _idGenerator);
            reqHandler.Node = node;

            byte[] responseBytes = await reqHandler.Handle(
                _shutdownRequestBytes.Array.Length
                , GetArray(_invalidRequestBytes));

            object response = _formatter.GetObject(responseBytes.Length
                , GetArray(new ArraySegment<byte>(responseBytes)));

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<InvalidRequestResponse>();
        }

        [Category("Unit")]
        [Test]
        public async Task RequestHandler_EmptyRequest()
        {
            INode node = Substitute.For<INode>();

            RequestHandler reqHandler = new RequestHandler(_formatter
                , _idGenerator);
            reqHandler.Node = node;

            byte[] responseBytes = await reqHandler.Handle(
                0
                , GetArray(new ArraySegment<byte>(new byte[0])));

            object response = _formatter.GetObject(responseBytes.Length
                , GetArray(new ArraySegment<byte>(responseBytes)));

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<InvalidRequestResponse>();
        }

        [Category("Unit")]
        [Test]
        public async Task RequestHandler_UnknownRequest()
        {
            INode node = Substitute.For<INode>();

            RequestHandler reqHandler = new RequestHandler(_formatter
                , _idGenerator);
            reqHandler.Node = node;

            byte[] responseBytes = await reqHandler.Handle(
                _unknownRequestBytes.Array.Length
                , GetArray(_unknownRequestBytes));

            object response = _formatter.GetObject(responseBytes.Length
                , GetArray(new ArraySegment<byte>(responseBytes)));

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<InvalidRequestResponse>();
        }

        [Category("Unit")]
        [Test]
        public async Task RequestHandler_InvalidRequest3()
        {
            INode node = Substitute.For<INode>();
            IList<ArraySegment<byte>> l = GetArray(_invalidRequestBytes);
            IRequestResponseFormatter formatter 
                = Substitute.For<IRequestResponseFormatter>();
            formatter.GetObject(_invalidRequestBytes.Array.Length,
                l).Returns(x => { throw new Exception(); });
            formatter.GetBytes(Arg.Any<object>())
                .Returns(_internalErrorResponseBytes);

            RequestHandler reqHandler = new RequestHandler(formatter
                , _idGenerator);
            reqHandler.Node = node;

            byte[] responseBytes = await reqHandler.Handle(
               _invalidRequestBytes.Array.Length, l);

            object response = _formatter.GetObject(responseBytes.Length
                , GetArray(new ArraySegment<byte>(responseBytes)));

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<InternalErrorResponse>();
        }

        [Category("Unit")]
        [Test]
        public async Task RequestHandler_Shutdown()
        {
            INode node = Substitute.For<INode>();

            RequestHandler reqHandler = new RequestHandler(_formatter
                , _idGenerator);
            reqHandler.Node = node;

            byte[] responseBytes = await reqHandler.Handle(
                _shutdownRequestBytes.Array.Length
                , GetArray(_shutdownRequestBytes));

            object response = _formatter.GetObject(responseBytes.Length
                , GetArray(new ArraySegment<byte>(responseBytes)));

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<ShutdownResponse>();

            node.Received(1).RequestShutdown();
        }

        [Category("Unit")]
        [Test]
        public async Task RequestHandler_PutSuccess_OneInRing()
        {
            Id id = new Id(new byte[]{ 123 }, 8);

            IDataEntries entries = Substitute.For<IDataEntries>();
            entries.Put("123", "123").Returns(true);
            SortedList<Id, INodeInfo> mainNodeSuccessors
              = new SortedList<Id, INodeInfo>();
            INode node = Substitute.For<INode>();
            node.Id.Returns(id);
            node.Info.Returns(new NodeInfo
            {
                Id = id,
                Url = new Uri("sock://localhost:8000")
            });
            node.Entries.Returns(entries);
            node.Successors.Returns(mainNodeSuccessors);

            RequestHandler reqHandler = new RequestHandler(_formatter
                , _idGenerator);
            reqHandler.Node = node;

            byte[] responseBytes = await reqHandler.Handle(
                _shutdownRequestBytes.Array.Length
                , GetArray(_putRequestBytes));

            object response = _formatter.GetObject(responseBytes.Length
                , GetArray(new ArraySegment<byte>(responseBytes)));

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<PutResponse>();

            await entries.Received(1).Put("123", "123");

            PutResponse putResponse = (PutResponse)response;
            putResponse.Status.Should().Be(Status.Ok);
        }

        [Category("Unit")]
        [Test]
        public async Task RequestHandler_PutSuccess_ThreeInRing()
        {
            Id _123Id = new Id(new byte[] { 123 }, 8);
            Id _160Id = new Id(new byte[] { 160 }, 8);
            Id _33Id = new Id(new byte[] { 33 }, 8);

            IDataEntries entries = Substitute.For<IDataEntries>();
            entries.Put("123", "123").Returns(true);

            INodeInfo info = new NodeInfo
            {
                Id = _123Id,
                Url = new Uri("sock://localhost:4000")
            };

            INode node = Substitute.For<INode>();
            node.Id.Returns(_123Id);
            node.Entries.Returns(entries);
            node.Info.Returns(info);

            SortedList<Id, INodeInfo> mainNodeSuccessors
              = new SortedList<Id, INodeInfo>();

            node.Successors.Returns(mainNodeSuccessors);

            RequestHandler reqHandler = new RequestHandler(_formatter
                , _idGenerator);
            reqHandler.Node = node;

            byte[] responseBytes = await reqHandler.Handle(
                _shutdownRequestBytes.Array.Length
                , GetArray(_putRequestBytes));

            object response = _formatter.GetObject(responseBytes.Length
                , GetArray(new ArraySegment<byte>(responseBytes)));

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<PutResponse>();

            await entries.Received(1).Put("123", "123");

            PutResponse putResponse = (PutResponse)response;
            putResponse.Status.Should().Be(Status.Ok);
        }

        [Category("Unit")]
        [Test]
        public async Task RequestHandler_PutFailed()
        {
            IDataEntries entries = Substitute.For<IDataEntries>();
           
            INode node = Substitute.For<INode>();
            node.Entries.Returns(entries);

            SortedList<Id, INodeInfo> mainNodeSuccessors
              = new SortedList<Id, INodeInfo>();
            node.Successors.Returns(mainNodeSuccessors);

            RequestHandler reqHandler = new RequestHandler(_formatter
                , _idGenerator);
            reqHandler.Node = node;

            byte[] responseBytes = await reqHandler.Handle(
                _shutdownRequestBytes.Array.Length
                , GetArray(_putRequestBytes));

            object response = _formatter.GetObject(responseBytes.Length
                , GetArray(new ArraySegment<byte>(responseBytes)));

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<PutResponse>();

            await entries.Received(1).Put("123", "123");
            
            PutResponse putResponse = (PutResponse)response;
            putResponse.Status.Should().Be(Status.Failed);
            
        }

        [Category("Unit")]
        [Test]
        public async Task RequestHandler_Get()
        {
            IDataEntries entries = Substitute.For<IDataEntries>();
            entries.Get("123").Returns("123");

            INode node = Substitute.For<INode>();
            node.Entries.Returns(entries);

            SortedList<Id, INodeInfo> mainNodeSuccessors
              = new SortedList<Id, INodeInfo>();
            node.Successors.Returns(mainNodeSuccessors);

            RequestHandler reqHandler = new RequestHandler(_formatter
                , _idGenerator);
            reqHandler.Node = node;

            byte[] responseBytes = await reqHandler.Handle(
                _shutdownRequestBytes.Array.Length
                , GetArray(_getRequestBytes));

            object response = _formatter.GetObject(responseBytes.Length
                , GetArray(new ArraySegment<byte>(responseBytes)));

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<GetResponse>();

            GetResponse getRes = (GetResponse)response;

            await entries.Received(1).Get("123");

            getRes.Status.Should().Be(Status.Ok);
            getRes.Value.Should().Be("123");
        }

        [Category("Unit")]
        [Test]
        public async Task RequestHandler_RemoveSuccess()
        {
            IDataEntries entries = Substitute.For<IDataEntries>();
            entries.Remove("123").Returns(true);

            INode node = Substitute.For<INode>();
            node.Entries.Returns(entries);

            SortedList<Id, INodeInfo> mainNodeSuccessors
              = new SortedList<Id, INodeInfo>();
            node.Successors.Returns(mainNodeSuccessors);

            RequestHandler reqHandler = new RequestHandler(_formatter
                , _idGenerator);
            reqHandler.Node = node;

            byte[] responseBytes = await reqHandler.Handle(
                _shutdownRequestBytes.Array.Length
                , GetArray(_removeRequestBytes));

            object response = _formatter.GetObject(responseBytes.Length
                , GetArray(new ArraySegment<byte>(responseBytes)));

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<RemoveResponse>();

            RemoveResponse getRes = (RemoveResponse)response;

            await entries.Received(1).Remove("123");

            getRes.Status.Should().Be(Status.Ok);
        }

        [Category("Unit")]
        [Test]
        public async Task RequestHandler_RemoveFailed()
        {
            IDataEntries entries = Substitute.For<IDataEntries>();
            entries.Remove("123").Returns(false);

            INode node = Substitute.For<INode>();
            node.Entries.Returns(entries);

            SortedList<Id, INodeInfo> mainNodeSuccessors
              = new SortedList<Id, INodeInfo>();
            node.Successors.Returns(mainNodeSuccessors);

            RequestHandler reqHandler = new RequestHandler(_formatter
                , _idGenerator);
            reqHandler.Node = node;

            byte[] responseBytes = await reqHandler.Handle(
                _shutdownRequestBytes.Array.Length
                , GetArray(_removeRequestBytes));

            object response = _formatter.GetObject(responseBytes.Length
                , GetArray(new ArraySegment<byte>(responseBytes)));

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<RemoveResponse>();

            RemoveResponse getRes = (RemoveResponse)response;

            await entries.Received(1).Remove("123");

            getRes.Status.Should().Be(Status.Failed);
        }

        [Category("Unit")]
        [Test]
        public async Task RequestHandler_Ping()
        {
            INode node = Substitute.For<INode>();

            RequestHandler reqHandler = new RequestHandler(_formatter
                , _idGenerator);
            reqHandler.Node = node;

            byte[] responseBytes = await reqHandler.Handle(
                _pingRequestBytes.Array.Length
                , GetArray(_pingRequestBytes));

            object response = _formatter.GetObject(responseBytes.Length
                , GetArray(new ArraySegment<byte>(responseBytes)));

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<PingResponse>();
        }

        [Category("Unit")]
        [Test]
        public async Task RequestHandler_NotifyNoPredecessor()
        {
            Id _89Id = new Id(new byte[] { 89 }, 8);
            Id _240Id = new Id(new byte[] { 240 }, 8);
            Uri _240Uri = new Uri("sock://localhost:5005");

            SortedList<Id, INodeInfo> mainNodeSuccessors
                = new SortedList<Id, INodeInfo>();

            INode mainNode = Substitute.For<INode>();
            mainNode.Id.Returns(_89Id);
            mainNode.Predecessor.Returns((INodeInfo)null);

            mainNode.Info.Returns(new NodeInfo
            {
                Id = _89Id,
                Url = new Uri("sock://localhost:5000")
            });
            
            RequestHandler reqHandler = new RequestHandler(_formatter
                , _idGenerator);
            reqHandler.Node = mainNode;

            Notify notify = new Notify
            {
                Id = _240Id,
                Url = _240Uri
            };
            byte[] notifyBytes = _formatter.GetBytes(notify);

            byte[] responseBytes = await reqHandler.Handle(
                notifyBytes.Length
                , GetArray(new ArraySegment<byte>(notifyBytes)));

            object response = _formatter.GetObject(responseBytes.Length
                , GetArray(new ArraySegment<byte>(responseBytes)));

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<NotifyResponse>();

            NotifyResponse notifyResponse
                = (NotifyResponse)response;
            notifyResponse.Status.Should().Be(Status.Ok);

            mainNode.Predecessor.Should().NotBeNull();
            mainNode.Predecessor.Id.Should().Be(_240Id);
        }

        [Category("Unit")]
        [Test]
        public async Task RequestHandler_NotifyHasPredecessor()
        {
            Id _89Id = new Id(new byte[] { 89 }, 8);
            Id _160Id = new Id(new byte[] { 160 }, 8);
            Id _240Id = new Id(new byte[] { 240 }, 8);

            INode mainNode = Substitute.For<INode>();
            mainNode.Id.Returns(_89Id);
            mainNode.Info.Returns(new NodeInfo
            {
                Id = _89Id,
                Url = new Uri("sock://localhost:5000")
            });

            INodeInfo _160Node = new NodeInfo
            {
                Id = _160Id,
                Url = new Uri("sock://localhost:5002")
            };
            INodeInfo _240Node = new NodeInfo
            {
                Id = _240Id,
                Url = new Uri("sock://localhost:5005")
            };

            mainNode.Predecessor.Returns(_160Node);

            RequestHandler reqHandler = new RequestHandler(_formatter
                 , _idGenerator);
            reqHandler.Node = mainNode;

            Notify notify = new Notify
            {
                Id = _240Id,
                Url = _240Node.Url
            };
            byte[] notifyBytes = _formatter.GetBytes(notify);

            byte[] responseBytes = await reqHandler.Handle(
                notifyBytes.Length
                , GetArray(new ArraySegment<byte>(notifyBytes)));

            object response = _formatter.GetObject(responseBytes.Length
                , GetArray(new ArraySegment<byte>(responseBytes)));

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<NotifyResponse>();

            NotifyResponse notifyResponse
                = (NotifyResponse)response;
            notifyResponse.Status.Should().Be(Status.Ok);

            mainNode.Predecessor.Should().NotBeNull();
            mainNode.Predecessor.Id.Should().Be(_240Id);
        }

        [Category("Unit")]
        [Test]
        public async Task RequestHandler_GetSuccessor_ZeroSuccessors()
        {
            Id _89Id = new Id(new byte[] { 89 }, 8);
            Id _120Id = new Id(new byte[] { 120 }, 8);

            SortedList<Id, INodeInfo> mainNodeSuccessors 
                = new SortedList<Id, INodeInfo>();

            INode mainNode = Substitute.For<INode>();
            mainNode.Id.Returns(_89Id);
            mainNode.Successors.Returns(mainNodeSuccessors);

            mainNode.Info.Returns(new NodeInfo
            {
                Id = _89Id,
                Url = new Uri("sock://localhost:5000")
            });
            INodeInfo _120Node = new NodeInfo
            {
                Id = _120Id,
                Url = new Uri("sock://localhost:5001")
            };

            FingerTable table = new FingerTable(mainNode.Info);
            mainNode.FingerTable.Returns(table);

            RequestHandler reqHandler = new RequestHandler(_formatter
                , _idGenerator);
            reqHandler.Node = mainNode;

            GetSuccessor getSuccessor = new GetSuccessor
            {
                Id = _120Id,
                Url = _120Node.Url
            };
            byte[] getSuccessorBytes = _formatter.GetBytes(getSuccessor);

            byte[] responseBytes = await reqHandler.Handle(
                getSuccessorBytes.Length
                , GetArray(new ArraySegment<byte>(getSuccessorBytes)));

            object response = _formatter.GetObject(responseBytes.Length
                , GetArray(new ArraySegment<byte>(responseBytes)));

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<GetSuccessorResponse>();

            GetSuccessorResponse getSucResponse 
                = (GetSuccessorResponse)response;
            getSucResponse.NodeInfo.Id.Should().Be(mainNode.Id);
        }

        [Category("Unit")]
        [Test]
        public async Task RequestHandler_GetSuccessor_OneSuccessor()
        {
            Id _89Id = new Id(new byte[] { 89 }, 8);
            Id _160Id = new Id(new byte[] { 160 }, 8);
            Id _120Id = new Id(new byte[] { 120 }, 8);

            SortedList<Id, INodeInfo> mainNodeSuccessors
                = new SortedList<Id, INodeInfo>();

            INode mainNode = Substitute.For<INode>();
            mainNode.Id.Returns(_89Id);
            mainNode.Successors.Returns(mainNodeSuccessors);

            mainNode.Info.Returns(new NodeInfo
            {
                Id = _89Id,
                Url = new Uri("sock://localhost:5000")
            });

            INodeInfo _160Node = new NodeInfo
            {
                Id = _160Id,
                Url = new Uri("sock://localhost:5002")
            };
            INodeInfo _120Node = new NodeInfo
            {
                Id = _120Id,
                Url = new Uri("sock://localhost:5001")
            };

            mainNodeSuccessors.Add(_160Id, _160Node);
            
            FingerTable table = new FingerTable(mainNode.Info);
            table.AddEntry(_160Node);
            mainNode.FingerTable.Returns(table);

            RequestHandler reqHandler = new RequestHandler(_formatter
                , _idGenerator);
            reqHandler.Node = mainNode;

            GetSuccessor getSuccessor = new GetSuccessor
            {
                Id = _120Id,
                Url = _120Node.Url
            };
            byte[] getSuccessorBytes = _formatter.GetBytes(getSuccessor);

            byte[] responseBytes = await reqHandler.Handle(
                getSuccessorBytes.Length
                , GetArray(new ArraySegment<byte>(getSuccessorBytes)));

            object response = _formatter.GetObject(responseBytes.Length
                , GetArray(new ArraySegment<byte>(responseBytes)));

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<GetSuccessorResponse>();

            GetSuccessorResponse getSucResponse
                = (GetSuccessorResponse)response;
            getSucResponse.NodeInfo.Id.Should().Be(_160Id);
        }

        [Category("Unit")]
        [Test]
        public async Task RequestHandler_GetSuccessor_TwoSuccessor()
        {
            Id _89Id = new Id(new byte[] { 89 }, 8);
            Id _160Id = new Id(new byte[] { 160 }, 8);
            Id _120Id = new Id(new byte[] { 120 }, 8);
            Id _32Id = new Id(new byte[] { 32 }, 8);
            Id _240Id = new Id(new byte[] { 240 }, 8);

            SortedList<Id, INodeInfo> mainNodeSuccessors
                = new SortedList<Id, INodeInfo>();

            INode mainNode = Substitute.For<INode>();
            mainNode.Id.Returns(_89Id);
            mainNode.Successors.Returns(mainNodeSuccessors);

            mainNode.Info.Returns(new NodeInfo
            {
                Id = _89Id,
                Url = new Uri("sock://localhost:5000")
            });

            INodeInfo _160Node = new NodeInfo
            {
                Id = _160Id,
                Url = new Uri("sock://localhost:5002")
            };
            INodeInfo _120Node = new NodeInfo
            {
                Id = _120Id,
                Url = new Uri("sock://localhost:5001")
            };
            INodeInfo _32Node = new NodeInfo
            {
                Id = _32Id,
                Url = new Uri("sock://localhost:5004")
            };
            INodeInfo _240Node = new NodeInfo
            {
                Id = _240Id,
                Url = new Uri("sock://localhost:5005")
            };

            mainNodeSuccessors.Add(_160Id, _160Node);
            mainNodeSuccessors.Add(_32Id, _32Node);

            FingerTable table = new FingerTable(mainNode.Info);
            table.AddEntry(_160Node);
            table.AddEntry(_32Node);
            mainNode.FingerTable.Returns(table);

            RequestHandler reqHandler = new RequestHandler(_formatter
                , _idGenerator);
            reqHandler.Node = mainNode;

            GetSuccessor getSuccessor = new GetSuccessor
            {
                Id = _240Id,
                Url = _240Node.Url
            };
            byte[] getSuccessorBytes = _formatter.GetBytes(getSuccessor);

            byte[] responseBytes = await reqHandler.Handle(
                getSuccessorBytes.Length
                , GetArray(new ArraySegment<byte>(getSuccessorBytes)));

            object response = _formatter.GetObject(responseBytes.Length
                , GetArray(new ArraySegment<byte>(responseBytes)));

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<GetSuccessorResponse>();

            GetSuccessorResponse getSucResponse
                = (GetSuccessorResponse)response;
            getSucResponse.NodeInfo.Id.Should().Be(_32Id);
        }

        [Category("Unit")]
        [Test]
        public async Task RequestHandler_GetSuccessor_OneSuccessorFinger()
        {
            Id _89Id = new Id(new byte[] { 89 }, 8);
            Id _160Id = new Id(new byte[] { 160 }, 8);
            Id _120Id = new Id(new byte[] { 120 }, 8);
            Id _32Id = new Id(new byte[] { 32 }, 8);
            Id _240Id = new Id(new byte[] { 240 }, 8);

            SortedList<Id, INodeInfo> mainNodeSuccessors
                = new SortedList<Id, INodeInfo>();

            INodeInfo _160Node = new NodeInfo
            {
                Id = _160Id,
                Url = new Uri("sock://localhost:5002")
            };
            INodeInfo _120Node = new NodeInfo
            {
                Id = _120Id,
                Url = new Uri("sock://localhost:5001")
            };
            INodeInfo _32Node = new NodeInfo
            {
                Id = _32Id,
                Url = new Uri("sock://localhost:5004")
            };
            INodeInfo _240Node = new NodeInfo
            {
                Id = _240Id,
                Url = new Uri("sock://localhost:5005")
            };

            INodeClient c = Substitute.For<INodeClient>();
            c.GetSuccessor(_240Id, _240Node.Url)
                .Returns(x => { return _32Node; });

            IRingFactory f = Substitute.For<IRingFactory>();
            f.CreateNodeClient(_160Node.Url)
                .Returns(c);

            INode mainNode = Substitute.For<INode>();
            mainNode.Id.Returns(_89Id);
            mainNode.Successors.Returns(mainNodeSuccessors);
            mainNode.Factory.Returns(f);
            mainNode.Info.Returns(new NodeInfo
            {
                Id = _89Id,
                Url = new Uri("sock://localhost:5000")
            });
            
            GetSuccessor getSuccessor = new GetSuccessor
            {
                Id = _240Id,
                Url = _240Node.Url
            };
            byte[] getSuccessorBytes = _formatter.GetBytes(getSuccessor);
            
            mainNodeSuccessors.Add(_160Id, _160Node);

            FingerTable table = new FingerTable(mainNode.Info);
            table.AddEntry(_160Node);
            table.AddEntry(_32Node);
            mainNode.FingerTable.Returns(table);

            RequestHandler reqHandler = new RequestHandler(_formatter
                , _idGenerator);
            reqHandler.Node = mainNode;

            byte[] responseBytes = await reqHandler.Handle(
                getSuccessorBytes.Length
                , GetArray(new ArraySegment<byte>(getSuccessorBytes)));

            object response = _formatter.GetObject(responseBytes.Length
                , GetArray(new ArraySegment<byte>(responseBytes)));

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<GetSuccessorResponse>();

            GetSuccessorResponse getSucResponse
                = (GetSuccessorResponse)response;
            getSucResponse.NodeInfo.Id.Should().Be(_32Id);
        }

        [Category("Integration")]
        [Test]
        public async Task RequestHandler_GetSuccessor_OneSuccessorButNotInRange()
        {
            NodeBuilder builder = new NodeBuilder();
            builder.SetUrl(new Uri("sock://localhost:5000"))
                .SetRingFactory(new NodeRingFactory());

            INode mainNode = builder.Build();
            mainNode.Start();

            builder.SetUrl(new Uri("sock://localhost:5001"));
            INode node2 = builder.Build();

            node2.JoinRing(mainNode.Channel.Url);
            
            GetSuccessor getSuccessor = new GetSuccessor
            {
                Id = new Id(new byte[] { 132, 81, 108, 234, 18, 90, 80, 3 }, 64),
                Url = node2.Channel.Url
            };

            RequestHandler reqHandler = new RequestHandler(_formatter
                , _idGenerator);
            reqHandler.Node = mainNode;

            byte[] getSuccessorBytes = _formatter.GetBytes(getSuccessor);

            byte[] responseBytes = await reqHandler.Handle(
                getSuccessorBytes.Length
                , GetArray(new ArraySegment<byte>(getSuccessorBytes)));

            object response = _formatter.GetObject(responseBytes.Length
                , GetArray(new ArraySegment<byte>(responseBytes)));

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<GetSuccessorResponse>();

            GetSuccessorResponse getSucResponse
                = (GetSuccessorResponse)response;
            getSucResponse.NodeInfo.Id.Should().Be(mainNode.Id);

            mainNode.RequestShutdown();
            node2.RequestShutdown();
        }
    }
}
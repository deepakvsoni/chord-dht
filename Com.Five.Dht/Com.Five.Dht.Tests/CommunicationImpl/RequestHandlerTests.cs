namespace Com.Five.Dht.Tests.CommunicationImpl
{
    using Communication;
    using Communication.Requests;
    using Communication.Responses;
    using Dht.CommunicationImpl;
    using Dht.Data;
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

        ArraySegment<byte> _shutdownRequestBytes, _putRequestBytes, _getRequestBytes
            , _removeRequestBytes, _invalidRequestBytes, _pingRequestBytes
            , _joinRequestBytes, _unknownRequestBytes;

        byte[] _internalErrorResponseBytes;

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
            _joinRequestBytes = new ArraySegment<byte>(
                _formatter.GetBytes(new Join()));

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
            Action a = () => new RequestHandler(_formatter);
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void RequestHandler_ConstructNullFormatter()
        {
            Action a = () => new RequestHandler(null);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public async Task RequestHandler_InvalidRequest()
        {
            INode node = Substitute.For<INode>();

            RequestHandler reqHandler = new RequestHandler(_formatter);
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

            RequestHandler reqHandler = new RequestHandler(_formatter);
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

            RequestHandler reqHandler = new RequestHandler(_formatter);
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

            RequestHandler reqHandler = new RequestHandler(formatter);
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

            RequestHandler reqHandler = new RequestHandler(_formatter);
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
        public async Task RequestHandler_PutSuccess()
        {
            IDataEntries entries = Substitute.For<IDataEntries>();
            entries.Put("123", "123").Returns(true);

            INode node = Substitute.For<INode>();
            node.Entries.Returns(entries);

            RequestHandler reqHandler = new RequestHandler(_formatter);
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

            RequestHandler reqHandler = new RequestHandler(_formatter);
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

            RequestHandler reqHandler = new RequestHandler(_formatter);
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

            RequestHandler reqHandler = new RequestHandler(_formatter);
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

            RequestHandler reqHandler = new RequestHandler(_formatter);
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

            RequestHandler reqHandler = new RequestHandler(_formatter);
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
        public async Task RequestHandler_Join()
        {
            INode node = Substitute.For<INode>();

            RequestHandler reqHandler = new RequestHandler(_formatter);
            reqHandler.Node = node;

            byte[] responseBytes = await reqHandler.Handle(
                _joinRequestBytes.Array.Length
                , GetArray(_joinRequestBytes));

            object response = _formatter.GetObject(responseBytes.Length
                , GetArray(new ArraySegment<byte>(responseBytes)));

            response.Should().NotBeNull();
            response.Should().BeAssignableTo<JoinResponse>();
        }
    }
}
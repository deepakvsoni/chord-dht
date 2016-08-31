namespace Com.Five.Dht.Tests.CommunicationImpl
{
    using Dht.CommunicationImpl;
    using NUnit.Framework;
    using System;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Runtime.Serialization.Formatters.Binary;
    using Communication.Requests;
    using Communication;
    using Communication.Responses;
    using Service;
    using NSubstitute;
    using Dht.Data;

    [TestFixture]
    public class RequestHandlerTests
    {
        IRequestResponseFormatter _formatter;

        ArraySegment<byte> _shutdownRequestBytes, _putRequestBytes, _getRequestBytes
            , _removeRequestBytes, _invalidRequestBytes;

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

            _invalidRequestBytes = new ArraySegment<byte>(
                _formatter.GetBytes(new object()));
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
            response.Should().BeAssignableTo<InvalidRequest>();
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
    }
}
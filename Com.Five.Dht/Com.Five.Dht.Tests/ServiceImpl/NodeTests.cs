namespace Com.Five.Dht.Tests.ServiceImpl
{
    using NUnit.Framework;
    using System;
    using System.Text;
    using FluentAssertions;
    using NSubstitute;
    using Dht.ServiceImpl;
    using Dht.Data;
    using Communication;
    using Dht.Service;

    [TestFixture]
    public class NodeTests
    {
        Id _id = new Id(Encoding.UTF8.GetBytes("sock://localhost:5000"), 160);
        IChannel _sChannel;
        IDataEntries _sDataEntries;
        IRequestHandler _sRequestHandler;

        [SetUp]
        public void Setup()
        {
            _sChannel = Substitute.For<IChannel>();
            _sDataEntries = Substitute.For<IDataEntries>();
            _sRequestHandler = Substitute.For<IRequestHandler>();
        }

        [Category("Unit")]
        [Test]
        public void Node_Construct()
        {
            Action a = () => new Node(_id, _sChannel, _sDataEntries
                    , _sRequestHandler);
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void Node_ConstructNullId()
        {
            Action a = () => new Node(null, _sChannel, _sDataEntries
                    , _sRequestHandler);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void Node_ConstructNullChannel()
        {
            Action a = () => new Node(_id, null, _sDataEntries
                    , _sRequestHandler);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void Node_ConstructNullDataEntries()
        {
            Action a = () => new Node(_id, _sChannel, null
                    , _sRequestHandler);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void Node_ConstructNullRequestHandler()
        {
            Action a = () => new Node(_id, _sChannel, _sDataEntries
                    , null);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void Node_Dispose()
        {
            Node node = new Node(_id, _sChannel, _sDataEntries
                    , _sRequestHandler);
            Action a = () => node.Dispose();
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void Node_CreateRing()
        {
            Node node = new Node(_id, _sChannel, _sDataEntries
                    , _sRequestHandler);

            _sChannel.State.Returns(State.Accepting);

            _sChannel.When(x => x.Open())
                .Do(x => { node.StateChange(State.Accepting); });

            Action a = () => node.Start();
            a.ShouldNotThrow();

            _sChannel.Received(1).Open();
        }

        [Category("Unit")]
        [Test]
        public void Node_CreateRingFailed()
        {
            Node node = new Node(_id, _sChannel, _sDataEntries
                    , _sRequestHandler);
            
            Action a = () => node.Start();
            a.ShouldThrow<ApplicationException>();

            _sChannel.Received(1).Open();
            _sChannel.Received(1).RequestClose();
        }

        [Category("Integration")]
        [Test]
        public void Node_CreateRing_SocketChannel()
        {
            NodeBuilder builder = new NodeBuilder();
            builder.SetUri(new Uri("sock://localhost:5000"));

            Node node = builder.Build();
            Action a = () => node.Start();
            a.ShouldNotThrow();

            node.RequestShutdown();
        }

        [Category("Integration")]
        [Test]
        public void Node_JoinRing_SocketChannel()
        {
            NodeBuilder builder = new NodeBuilder();
            builder.SetUri(new Uri("sock://localhost:5000"));

            Node node = builder.Build();
            node.Start();

            node.Channel.State.Should().Be(State.Accepting);

            builder.SetUri(new Uri("sock://localhost:5001"));

            Node clientNode = builder.Build();
            clientNode.JoinRing(node.Channel.Url);

            clientNode.Successors.Count.Should().Be(1);

            node.RequestShutdown();
            clientNode.RequestShutdown();
        }

        [Category("Integration")]
        [Test]
        public void Node_JoinRing_Three()
        {
            NodeBuilder builder = new NodeBuilder();
            builder.SetUri(new Uri("sock://localhost:5000"));

            Node node = builder.Build();
            node.Start();

            node.Channel.State.Should().Be(State.Accepting);

            builder.SetUri(new Uri("sock://localhost:5001"));

            Node clientNode = builder.Build();
            clientNode.JoinRing(node.Channel.Url);

            clientNode.Successors.Count.Should().Be(1);

            builder.SetUri(new Uri("sock://localhost:5002"));

            Node clientNode2 = builder.Build();
            clientNode2.JoinRing(node.Channel.Url);

            clientNode2.Successors.Count.Should().Be(1);

            node.RequestShutdown();
            clientNode.RequestShutdown();
            clientNode2.RequestShutdown();
        }
    }
}

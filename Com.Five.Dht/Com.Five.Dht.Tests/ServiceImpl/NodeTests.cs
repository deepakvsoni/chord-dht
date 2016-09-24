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
    using Dht.Service.Factory;
    using Dht.ServiceImpl.FactoryImpl;

    [TestFixture]
    public class NodeTests
    {
        Id _id = new Id(Encoding.UTF8.GetBytes("sock://localhost:5000"), 160);
        IdGenerator _idGenerator;
        IDataEntries _sDataEntries;
        IChannel _sChannel;
        IRequestResponseFormatter _sFormatter;
        IRequestHandler _sRequestHandler;
        IRingFactory _sRingFactory;

        [SetUp]
        public void Setup()
        {
            IHashFunction hashFunction = Substitute.For<IHashFunction>();
            hashFunction.Length.Returns(8);
            hashFunction.ComputeHash(Arg.Any<byte[]>())
                .Returns(new byte[] { 123 });

            _idGenerator = new IdGenerator(8, hashFunction);

            _sDataEntries = Substitute.For<IDataEntries>();
            _sFormatter = Substitute.For<IRequestResponseFormatter>();
            _sRequestHandler = Substitute.For<IRequestHandler>();
            _sChannel = Substitute.For<IChannel>();

            _sRingFactory = Substitute.For<IRingFactory>();
            _sRingFactory.CreateDataEntries().Returns(_sDataEntries);
            _sRingFactory.Formatter.Returns(_sFormatter);
            _sRingFactory.IdGenerator.Returns(_idGenerator);
            _sRingFactory.CreateRequestHandler(
                Arg.Any<IRequestResponseFormatter>(), Arg.Any<IdGenerator>())
                .Returns(_sRequestHandler);
            _sRingFactory.CreateChannel(Arg.Any<Uri>()).Returns(_sChannel);
        }

        [Category("Unit")]
        [Test]
        public void Node_Construct()
        {
            Action a = () => new Node(_id, _sRingFactory, _sChannel
                , _sDataEntries, _sRequestHandler);
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void Node_ConstructNullId()
        {
            Action a = () => new Node(null, _sRingFactory, _sChannel
                , _sDataEntries, _sRequestHandler);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void Node_ConstructNullChannel()
        {
            Action a = () => new Node(_id, _sRingFactory, null, _sDataEntries
                    , _sRequestHandler);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void Node_ConstructNullDataEntries()
        {
            Action a = () => new Node(_id, _sRingFactory, _sChannel, null
                    , _sRequestHandler);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void Node_ConstructNullRequestHandler()
        {
            Action a = () => new Node(_id, _sRingFactory, _sChannel
                , _sDataEntries, null);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void Node_Dispose()
        {
            Node node = new Node(_id, _sRingFactory, _sChannel, _sDataEntries
                    , _sRequestHandler);
            Action a = () => node.Dispose();
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void Node_CreateRing()
        {
            Node node = new Node(_id, _sRingFactory, _sChannel, _sDataEntries
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
            Node node = new Node(_id, _sRingFactory, _sChannel, _sDataEntries
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
            builder.SetUrl(new Uri("sock://localhost:5000"))
                .SetRingFactory(new NodeRingFactory());

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
            builder.SetUrl(new Uri("sock://localhost:5000"))
                 .SetRingFactory(new NodeRingFactory());

            Node node = builder.Build();
            node.Start();

            node.Channel.State.Should().Be(State.Accepting);

            builder.SetUrl(new Uri("sock://localhost:5001"));

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
            builder.SetUrl(new Uri("sock://localhost:5000"))
                 .SetRingFactory(new NodeRingFactory());

            Node node = builder.Build();
            node.Start();

            node.Channel.State.Should().Be(State.Accepting);

            builder.SetUrl(new Uri("sock://localhost:5001"));

            Node clientNode = builder.Build();
            clientNode.JoinRing(node.Channel.Url);

            clientNode.Successors.Count.Should().Be(1);

            builder.SetUrl(new Uri("sock://localhost:5002"));

            Node clientNode2 = builder.Build();
            clientNode2.JoinRing(node.Channel.Url);

            clientNode2.Successors.Count.Should().Be(1);

            node.RequestShutdown();
            clientNode.RequestShutdown();
            clientNode2.RequestShutdown();
        }
    }
}

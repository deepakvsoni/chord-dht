namespace Com.Five.Dht.Tests.ServiceImpl
{
    using Communication;
    using Dht.CommunicationImpl;
    using Dht.Data;
    using Dht.DataImpl;
    using Dht.Service;
    using Dht.Factory;
    using Dht.ServiceImpl;
    using FluentAssertions;
    using NSubstitute;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class NodeBuilderTests
    {
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
        public void NodeBuilder_Construct()
        {
            Action a = () => new NodeBuilder();
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void NodeBuilder_BuildWithoutRingFactory()
        {
            NodeBuilder b = new NodeBuilder();
            b.SetUrl(new Uri("sock://localhost:5000"));

            Action a = () => b.Build();
            a.ShouldThrow<InvalidOperationException>();
        }

        [Category("Unit")]
        [Test]
        public void NodeBuilder_BuildWithoutUri()
        {
            NodeBuilder b = new NodeBuilder();
            Action a = () => b.Build();
            a.ShouldThrow<InvalidOperationException>();
        }

        [Category("Unit")]
        [Test]
        public void NodeBuilder_BuildDefaultNoErrors()
        {
            NodeBuilder b = new NodeBuilder();
            b.SetUrl(new Uri("sock://localhost:5000"))
                .SetRingFactory(_sRingFactory);
            Action a = () => b.Build();
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void NodeBuilder_BuildDefault()
        {
            NodeBuilder b = new NodeBuilder();
            b.SetUrl(new Uri("sock://localhost:5000"))
                 .SetRingFactory(_sRingFactory);

            Node node = b.Build();

            node.Id.Should().NotBeNull();

            node.Channel.Should().NotBeNull();
            node.Channel.Should().BeAssignableTo<IChannel>();
            node.Channel.Should().Be(_sChannel);

            node.Entries.Should().NotBeNull();
            node.Entries.Should().BeAssignableTo<IDataEntries>();
            node.Entries.Should().Be(_sDataEntries);

            node.Factory.Should().Be(_sRingFactory);

            node.Info.Should().NotBeNull();
        }

        [Category("Unit")]
        [Test]
        public void NodeBuilder_SetHashFunction()
        {
            NodeBuilder b = new NodeBuilder();
            Action a = () =>
                b.SetHashFunction(_sRingFactory.HashFunction);
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void NodeBuilder_SetRequestHandler()
        {
            NodeBuilder b = new NodeBuilder();
            Action a = () =>
                b.SetRequestHandler(new RequestHandler(
                    new RequestResponseBinaryFormatter(), _idGenerator));
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void NodeBuilder_SetRingFactory()
        {
            NodeBuilder b = new NodeBuilder();
            Action a = () => b.SetRingFactory(_sRingFactory);
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void NodeBuilder_SetRequestResponseFormatter()
        {
            NodeBuilder b = new NodeBuilder();
            Action a = () =>
                b.SetRequestResponseFormatter(
                    new RequestResponseBinaryFormatter());
            a.ShouldNotThrow();
        }


        [Category("Unit")]
        [Test]
        public void NodeBuilder_SetUri()
        {
            NodeBuilder b = new NodeBuilder();
            Action a = () =>
                b.SetUrl(new Uri("sock://localhost:5000"));
       
            a.ShouldNotThrow();
        }


        [Category("Unit")]
        [Test]
        public void NodeBuilder_SetChannel()
        {
            NodeBuilder b = new NodeBuilder();
            Action a = () => b.SetChannel(new SocketChannel(
                    new Uri("sock://localhost:5000")));
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void NodeBuilder_SetDataEntries()
        {
            NodeBuilder b = new NodeBuilder();
            Action a = () => b.SetDataEntries(new DataEntries());
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void NodeBuilder_SetIdGenerator()
        {
            NodeBuilder b = new NodeBuilder();
            Action a = () => b.SetIdGenerator(_idGenerator);
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void NodeBuilder_SetCascade()
        {
            SocketChannel c = new SocketChannel(
                new Uri("sock://localhost:5000"));
            SHA1HashFunction f = new SHA1HashFunction();
            DataEntries d = new DataEntries();
            RequestHandler h = new RequestHandler(
                new RequestResponseBinaryFormatter(), _idGenerator);
            NodeBuilder b = new NodeBuilder();
            Action a = () =>
                b.SetUrl(new Uri("sock://localhost:5000"))
                    .SetChannel(c)
                    .SetHashFunction(f)
                    .SetDataEntries(d)
                    .SetRequestHandler(h)
                    .SetRingFactory(_sRingFactory);
            a.ShouldNotThrow();

            INode node = b.Build();

            node.Id.Should().NotBeNull();

            node.Channel.Should().NotBeNull();
            node.Channel.Should().BeAssignableTo<IChannel>();
            node.Channel.Should().Be(c);

            node.Entries.Should().NotBeNull();
            node.Entries.Should().BeAssignableTo<IDataEntries>();
            node.Entries.Should().Be(d);

            node.Factory.Should().Be(_sRingFactory);

            node.Info.Should().NotBeNull();
        }
    }
}
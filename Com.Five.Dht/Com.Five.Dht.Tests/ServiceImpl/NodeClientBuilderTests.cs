namespace Com.Five.Dht.Tests.ServiceImpl
{
    using Communication;
    using Dht.Data;
    using Dht.Service;
    using Dht.Service.Factory;
    using Dht.ServiceImpl;
    using FluentAssertions;
    using NSubstitute;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class NodeClientBuilderTests
    {
        IdGenerator _idGenerator;
        IChannelClient _sChannelClient;
        IRequestResponseFormatter _sFormatter;
        IRingFactory _sRingFactory;

        [SetUp]
        public void Setup()
        {
            IHashFunction hashFunction = Substitute.For<IHashFunction>();
            hashFunction.Length.Returns(8);
            hashFunction.ComputeHash(Arg.Any<byte[]>())
                .Returns(new byte[] { 123 });

            _idGenerator = new IdGenerator(8, hashFunction);

            _sFormatter = Substitute.For<IRequestResponseFormatter>();
            _sChannelClient = Substitute.For<IChannelClient>();

            _sRingFactory = Substitute.For<IRingFactory>();
            _sRingFactory.Formatter.Returns(_sFormatter);
            _sRingFactory.IdGenerator.Returns(_idGenerator);
            _sRingFactory.CreateChannelClient(Arg.Any<Uri>())
                .Returns(_sChannelClient);
        }

        [Category("Unit")]
        [Test]
        public void NodeClientBuilder_Construct()
        {
            Action a = () => new NodeClientBuilder();
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void NodeClientBuilder_BuildNullServerUrl()
        {
            NodeClientBuilder b = new NodeClientBuilder();
            Action a = () => b.Build();
            a.ShouldThrow<InvalidOperationException>();
        }

        [Category("Unit")]
        [Test]
        public void NodeClientBuilder_BuildNullFactory()
        {
            NodeClientBuilder b = new NodeClientBuilder();
            b.SetServerUrl(new Uri("sock://localhost:5000"));

            Action a = () => b.Build();
            a.ShouldThrow<InvalidOperationException>();
        }

        [Category("Unit")]
        [Test]
        public void NodeClientBuilder_BuildNullFactoryButChannelClientSet()
        {
            NodeClientBuilder b = new NodeClientBuilder();
            b.SetServerUrl(new Uri("sock://localhost:5000"))
                .SetChannelClient(_sChannelClient);

            Action a = () => b.Build();
            a.ShouldThrow<InvalidOperationException>();
        }

        [Category("Unit")]
        [Test]
        public void NodeClientBuilder_BuildNullFactoryButFormatterSet()
        {
            NodeClientBuilder b = new NodeClientBuilder();
            b.SetServerUrl(new Uri("sock://localhost:5000"))
                .SetRequestResponseFormatter(_sRingFactory.Formatter);

            Action a = () => b.Build();
            a.ShouldThrow<InvalidOperationException>();
        }

        [Category("Unit")]
        [Test]
        public void NodeClientBuilder_BuildProvidedInfo()
        {
            NodeClientBuilder b = new NodeClientBuilder();
            b.SetServerUrl(new Uri("sock://localhost:5000"))
                .SetRequestResponseFormatter(_sRingFactory.Formatter)
                .SetChannelClient(_sChannelClient);

            INodeClient client = b.Build();
            client.Should().NotBeNull();
            client.ChannelClient.Should().Be(_sChannelClient);
        }

        [Category("Unit")]
        [Test]
        public void NodeClientBuilder_BuildFromFactory()
        {
            NodeClientBuilder b = new NodeClientBuilder();
            b.SetServerUrl(new Uri("sock://localhost:5000"))
                .SetRingFactory(_sRingFactory);

            INodeClient client = b.Build();
            client.Should().NotBeNull();
            client.ChannelClient.Should().Be(_sChannelClient);
        }
    }
}
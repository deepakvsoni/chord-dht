namespace Com.Five.Dht.Tests.ServiceImpl
{
    using Dht.CommunicationImpl;
    using Dht.Data;
    using Dht.DataImpl;
    using Dht.ServiceImpl;
    using FluentAssertions;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class NodeBuilderTests
    {
        IdGenerator _idGenerator = new IdGenerator(8, new SHA1HashFunction());

        [Category("Unit")]
        [Test]
        public void NodeBuilder_Construct()
        {
            Action a = () => new NodeBuilder();
            a.ShouldNotThrow();
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
            b.SetUrl(new Uri("sock://localhost:5000"));
            Action a = () => b.Build();
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void NodeBuilder_BuildDefault()
        {
            NodeBuilder b = new NodeBuilder();
            b.SetUrl(new Uri("sock://localhost:5000"));

            Node node = b.Build();

            node.Id.Should().NotBeNull();

            node.Channel.Should().NotBeNull();
            node.Channel.Should().BeAssignableTo<SocketChannel>();

            node.Entries.Should().NotBeNull();
            node.Entries.Should().BeAssignableTo<DataEntries>();
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
        public void NodeBuilder_SetCascade()
        {
            NodeBuilder b = new NodeBuilder();
            Action a = () =>
                b.SetUrl(new Uri("sock://localhost:5000"))
                    .SetChannel(new SocketChannel(
                        new Uri("sock://localhost:5000")))
                    .SetMaxNoOfBits(32)
                    .SetHashFunction(new SHA1HashFunction())
                    .SetDataEntries(new DataEntries())
                    .SetRequestHandler(new RequestHandler(
                        new RequestResponseBinaryFormatter(), _idGenerator));
            a.ShouldNotThrow();
        }
    }
}
namespace Com.Five.Dht.Tests.ServiceImpl.FactoryImpl
{
    using NUnit.Framework;
    using System;
    using FluentAssertions;
    using Dht.ServiceImpl.FactoryImpl;
    using Communication;
    using Dht.Data;
    using Dht.Service;

    [TestFixture]
    public class NodeRingFactoryTests
    {
        Uri _url = new Uri("sock://localhost:4000");

        [Category("Unit")]
        [Test]
        public void NodeRingFactory_Construct()
        {
            Action a = () => new NodeRingFactory();
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void NodeRingFactory_ConstructMaxNoOfBits()
        {
            Action a = () => new NodeRingFactory(8);
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void NodeRingFactory_ConstructZeroMaxNoOfBits()
        {
            Action a = () => new NodeRingFactory(0);
            a.ShouldThrow<ArgumentException>();
        }

        [Category("Unit")]
        [Test]
        public void NodeRingFactory_Defaults()
        {
            NodeRingFactory f = new NodeRingFactory();
            f.Formatter.Should().NotBeNull();
            f.HashFunction.Should().NotBeNull();
            f.IdGenerator.Should().NotBeNull();
        }

        [Category("Unit")]
        [Test]
        public void NodeRingFactory_CreateChannel()
        {
            NodeRingFactory f = new NodeRingFactory();
            IChannel c = f.CreateChannel(_url);
            c.Should().NotBeNull();
            c.Url.Should().Be(_url);
        }

        [Category("Unit")]
        [Test]
        public void NodeRingFactory_CreateDataEntries()
        {
            NodeRingFactory f = new NodeRingFactory();
            IDataEntries d = f.CreateDataEntries();
            d.Should().NotBeNull();
        }

        [Category("Unit")]
        [Test]
        public void NodeRingFactory_CreateRequestHandler()
        {
            NodeRingFactory f = new NodeRingFactory();
            IRequestHandler r = f.CreateRequestHandler(f.Formatter,
                f.IdGenerator);
            r.Should().NotBeNull();
        }

        [Category("Unit")]
        [Test]
        public void NodeRingFactory_CreateChannelClient()
        {
            NodeRingFactory f = new NodeRingFactory();
            IChannelClient cc = f.CreateChannelClient(_url);
            cc.Should().NotBeNull();
        }

        [Category("Unit")]
        [Test]
        public void NodeRingFactory_CreateNode()
        {
            NodeRingFactory f = new NodeRingFactory();
            INode n = f.CreateNode(_url);
            n.Should().NotBeNull();
            n.Channel.Should().NotBeNull();
            n.Entries.Should().NotBeNull();
            n.Factory.Should().NotBeNull();
            n.Info.Should().NotBeNull();
        }
    }
}

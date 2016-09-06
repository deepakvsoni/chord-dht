namespace Com.Five.Dht.Tests.CommunicationImpl
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using FluentAssertions;
    using NSubstitute;
    using Dht.CommunicationImpl;
    using Communication;

    [TestFixture]
    public class SocketChannelClientTests
    {
        [Category("Unit")]
        [Test]
        public void SocketChannelClient_Construct()
        {
            Action a = ()
                => new SocketChannelClient(new Uri("sock://localhost:5000"));
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void SocketChannelClient_ConstructNullUri()
        {
            Action a = ()
                => new SocketChannelClient(null);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void SocketChannelClient_Dispose()
        {
            SocketChannelClient c
                = new SocketChannelClient(new Uri("sock://localhost:5000"));
            Action a = ()
                => c.Dispose();
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void SocketChannelClient_RegisterNullListener()
        {
            SocketChannelClient client = new SocketChannelClient(
                new Uri("sock://localhost:5000"));
            Action a = ()
                    => client.RegisterChannelClientListener(null);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void SocketChannelClient_RegisterListener()
        {
            SocketChannelClient client = new SocketChannelClient(
                new Uri("sock://localhost:5000"));

            IChannelClientListener listener 
                = Substitute.For<IChannelClientListener>();

            Action a = ()
                    => client.RegisterChannelClientListener(listener);
            a.ShouldNotThrow();
        }
    }
}
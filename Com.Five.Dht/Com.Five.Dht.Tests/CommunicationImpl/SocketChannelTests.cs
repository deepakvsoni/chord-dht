namespace Com.Five.Dht.Tests.CommunicationImpl
{
    using Dht.CommunicationImpl;
    using NUnit.Framework;
    using System;
    using FluentAssertions;
    using Communication;
    using NSubstitute;
    using System.Threading;
    using System.Net.Sockets;

    [TestFixture]
    public class SocketChannelTests
    {
        [Category("Unit")]
        [Test]
        public void SocketChannel_Construct()
        {
            Action a = () => new SocketChannel(new Uri("sock://localhost:5000"));
            a.ShouldNotThrow();        
        }

        [Category("Unit")]
        [Test]
        public void SocketChannel_ConstructNullUrl()
        {
            Action a = () => new SocketChannel(null);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void SocketChannel_ConstructInvalidScheme()
        {
            Action a = () => new SocketChannel(new Uri("http://localhost"));
            a.ShouldThrow<ArgumentException>();
        }

        [Category("Unit")]
        [Test]
        public void SocketChannel_ConstructInvalidPort()
        {
            Action a = () => new SocketChannel(new Uri("sock://localhost"));
            a.ShouldThrow<ArgumentException>();
        }

        [Category("Unit")]
        [Test]
        public void SocketChannel_Dispose()
        {
            SocketChannel c = new SocketChannel(new Uri("sock://localhost:5000")); ;
            Action a = () => c.Dispose();
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void SocketChannel_RegisterNullListener()
        {
            SocketChannel c = new SocketChannel(new Uri("sock://localhost:5000"));
            Action a = () => c.RegisterChannelListener(null);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void SocketChannel_Open()
        {
            AutoResetEvent e = new AutoResetEvent(false);
            IChannelListener listener = Substitute.For<IChannelListener>();
            listener.When(x => x.StateChange(State.Accepting))
                .Do(x => e.Set());
            listener.When(x => x.StateChange(State.NotOpen))
                            .Do(x => e.Set());

            SocketChannel c = new SocketChannel(new Uri("sock://localhost:5000"));
            c.RegisterChannelListener(listener);
            c.Open();

            e.WaitOne(10000);

            c.State.Should().Be(State.Accepting);

            c.RequestClose();

            e.WaitOne(100000);
        }

        [Category("Unit")]
        [Test]
        public void SocketChannel_OpenTwoOnSamePort()
        {
            AutoResetEvent e = new AutoResetEvent(false);
            IChannelListener listener = Substitute.For<IChannelListener>();
            listener.When(x => x.StateChange(State.Accepting))
                .Do(x => e.Set());
            listener.When(x => x.HandleError(
                (int)SocketError.AddressAlreadyInUse))
                .Do(x => e.Set());
            listener.When(x => x.StateChange(State.NotOpen))
                .Do(x => e.Set());
            SocketChannel c = new SocketChannel(new Uri("sock://localhost:5000"));
            c.RegisterChannelListener(listener);
            c.Open();

            e.WaitOne(10000);

            c.State.Should().Be(State.Accepting);

            c = new SocketChannel(new Uri("sock://localhost:5000"));
            c.RegisterChannelListener(listener);
            c.Open();

            e.WaitOne(10000);

            listener.Received(1).HandleError(
                (int)SocketError.AddressAlreadyInUse);

            c.State.Should().Be(State.Error);

            c.RequestClose();

            e.WaitOne(10000);
        }
    }
}

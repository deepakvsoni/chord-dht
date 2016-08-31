namespace Com.Five.Dht.Tests.CommunicationImpl
{
    using Dht.CommunicationImpl;
    using NUnit.Framework;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Communication.Requests;
    using NSubstitute;
    using Communication;
    using System.Collections.Generic;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;
    using Communication.Responses;
    using System.Text;

    [TestFixture]
    public class SocketTests
    {
        Uri _serverUri = new Uri("sock://localhost:10000");

        byte[] shutdownResponseBytes, insertResponseBytes;

        [SetUp]
        public void Setup()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, new ShutdownResponse
                {
                    Status = Status.Ok,
                    ShutdownAccepted = true
                });

                ms.Seek(0, SeekOrigin.Begin);

                shutdownResponseBytes = ms.GetBuffer();
            }
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, new PutResponse
                {
                    Status = Status.Ok
                });

                ms.Seek(0, SeekOrigin.Begin);

                insertResponseBytes = ms.GetBuffer();
            }
        }

        [Category("Integration")]
        [Test]
        public async Task SocketCommunication_StartShutdown()
        {
            AutoResetEvent _accepting = new AutoResetEvent(false);
            AutoResetEvent _notopen = new AutoResetEvent(false);
            SocketChannel channel = new SocketChannel(_serverUri);

            IChannelListener listener = Substitute.For<IChannelListener>();

            listener.HandleRequest(Arg.Any<int>(),
               Arg.Any<IList<ArraySegment<byte>>>())
               .Returns(shutdownResponseBytes)
               .AndDoes((p) =>
               {
                   channel.RequestClose();
               });
            listener.When(x => x.StateChange(State.Accepting))
                .Do(x => _accepting.Set());
            listener.When(x => x.StateChange(State.NotOpen))
                .Do(x => _notopen.Set());

            channel.RegisterChannelListener(listener);

            channel.Open();

            _accepting.WaitOne(10000);

            channel.State.Should().Be(State.Accepting);

            SocketChannelClient client = new SocketChannelClient(_serverUri);
            client.Connect().Should().BeTrue();

            Response response = await client.SendRequest(new Shutdown());

            response.Should().BeOfType<ShutdownResponse>();
            response.Status.Should().Be(Status.Ok);
            ((ShutdownResponse)response).ShutdownAccepted.Should().BeTrue();

            _notopen.WaitOne(10000);

            channel.State.Should().Be(State.NotOpen);
        }
        
        [Category("Integration")]
        [Test]
        public async Task SocketCommunication_Insert()
        {
            AutoResetEvent _accepting = new AutoResetEvent(false);
            AutoResetEvent _notopen = new AutoResetEvent(false);
            SocketChannel channel = new SocketChannel(_serverUri);

            IChannelListener listener = Substitute.For<IChannelListener>();

            listener.HandleRequest(Arg.Any<int>(),
               Arg.Any<IList<ArraySegment<byte>>>())
               .Returns(insertResponseBytes)
               .AndDoes((p) =>
               {
                   channel.RequestClose();
               });
            listener.When(x => x.StateChange(State.Accepting))
                .Do(x => _accepting.Set());
            listener.When(x => x.StateChange(State.NotOpen))
                .Do(x => _notopen.Set());

            channel.RegisterChannelListener(listener);

            channel.Open();

            _accepting.WaitOne(10000);

            channel.State.Should().Be(State.Accepting);

            SocketChannelClient client = new SocketChannelClient(_serverUri);
            client.Connect().Should().BeTrue();

            Put insert = new Put
            {
                Key = "ABC"
            };

            StringBuilder s = new StringBuilder(2048);
            for(int i = 0; i < 2048; ++i)
            {
                s.Append("#");
            }
            insert.Value = s.ToString();

            Response response = await client.SendRequest(insert);

            response.Should().BeOfType<PutResponse>();
            response.Status.Should().Be(Status.Ok);

            _notopen.WaitOne(10000);

            channel.State.Should().Be(State.NotOpen);
        }

        [Category("Integration")]
        [Test]
        public void SocketCommunication_StartForceClose()
        {
            AutoResetEvent _accepting = new AutoResetEvent(false);
            AutoResetEvent _notopen = new AutoResetEvent(false);
            SocketChannel channel = new SocketChannel(_serverUri);

            IChannelListener listener = Substitute.For<IChannelListener>();
            
            listener.When(x => x.StateChange(State.Accepting))
                .Do(x => _accepting.Set());
            listener.When(x => x.StateChange(State.NotOpen))
                .Do(x => _notopen.Set());

            channel.RegisterChannelListener(listener);

            channel.Open();

            _accepting.WaitOne(10000);

            channel.State.Should().Be(State.Accepting);

            SocketChannelClient client = new SocketChannelClient(_serverUri);
            client.Connect().Should().BeTrue();

            channel.RequestClose();
            
            _notopen.WaitOne(20000);

            channel.State.Should().Be(State.NotOpen);
        }

        [Category("Integration")]
        [Test]
        public void SocketCommunication_StartForceClose2Client()
        {
            AutoResetEvent _accepting = new AutoResetEvent(false);
            AutoResetEvent _notopen = new AutoResetEvent(false);
            SocketChannel channel = new SocketChannel(_serverUri);

            IChannelListener listener = Substitute.For<IChannelListener>();

            listener.When(x => x.StateChange(State.Accepting))
                .Do(x => _accepting.Set());
            listener.When(x => x.StateChange(State.NotOpen))
                .Do(x => _notopen.Set());

            channel.RegisterChannelListener(listener);

            channel.Open();

            _accepting.WaitOne(10000);

            channel.State.Should().Be(State.Accepting);

            SocketChannelClient client = new SocketChannelClient(_serverUri);
            client.Connect().Should().BeTrue();

            SocketChannelClient client2 = new SocketChannelClient(_serverUri);
            client2.Connect().Should().BeTrue();

            channel.RequestClose();

            _notopen.WaitOne(20000);

            channel.State.Should().Be(State.NotOpen);
        }
    }
}
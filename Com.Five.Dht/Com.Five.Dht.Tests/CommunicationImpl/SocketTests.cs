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

    [TestFixture]
    public class SocketTests
    {
        Uri _serverUri = new Uri("sock://localhost:10000");

        byte[] shutdownBytes;

        [SetUp]
        public void Setup()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using(MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, new Shutdown());

                ms.Seek(0, SeekOrigin.Begin);

                shutdownBytes = ms.GetBuffer();
            }
        }

        [Category("Integration")]
        [Test]
        public async Task SocketCommunication_StartShutdown()
        {
            AutoResetEvent _listening = new AutoResetEvent(false);
            SocketChannel channel = new SocketChannel(_serverUri);
            int count = 0;

            IChannelListener listener = Substitute.For<IChannelListener>();

            listener.HandleRequest(channel, Arg.Any<int>(),
               Arg.Any<IList<ArraySegment<byte>>>())
               .Returns(shutdownBytes)
               .AndDoes((p) =>
               {
                   IChannel c = (IChannel)p[0];
                   c.RequestClose();
               });

            channel.RegisterChannelListener(listener);

            channel.Open();
            count = 0;

            while ((++count) != 5
            && channel.State != State.Accepting)
            {
                Task.Delay(500).Wait();
            }
            channel.State.Should().Be(State.Accepting);

            SocketChannelClient client = new SocketChannelClient(_serverUri);
            client.Connect().Should().BeTrue();

            await client.SendRequest(new Shutdown());

            count = 0;

            while ((++count) != 5
            && channel.State != State.NotOpen)
            {
                await Task.Delay(500);
            }
            channel.State.Should().Be(State.NotOpen);
        }
    }
}

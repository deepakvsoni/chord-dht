namespace Com.Five.Dht.Tests.CommunicationImpl
{
    using Dht.CommunicationImpl;
    using NUnit.Framework;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Communication.Requests;

    [TestFixture]
    public class SocketTests
    {
        Uri _serverUri = new Uri("sock://localhost:10000");

        [Category("Integration")]
        [Test]
        public async Task SocketCommunication_StartShutdown()
        {
            AutoResetEvent _listening = new AutoResetEvent(false);
            SocketChannel channel = new SocketChannel(_serverUri);
            int count = 0;

            bool serverStarted = await Task.Factory.StartNew<bool>(() =>
            {
                channel.Open();
                count = 0;

                while ((++count) != 5 
                && channel.State != Communication.State.Accepting)
                {
                    Task.Delay(500).Wait();
                }
                channel.State.Should().Be(Communication.State.Accepting);

                return true;
            });

            SocketChannelClient client = new SocketChannelClient(_serverUri);
            client.Connect().Should().BeTrue();

            await client.SendRequest(new Shutdown());

            count = 0;

            while ((++count) != 5
            && channel.State != Communication.State.NotOpen)
            {
                await Task.Delay(500);
            }
            channel.State.Should().Be(Communication.State.NotOpen);

        }
    }
}

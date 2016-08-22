namespace Com.Five.Dht.Tests.CommunicationImpl
{
    using Dht.CommunicationImpl;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using FluentAssertions;

    [TestFixture]
    public class SocketChannelTests
    {
        [Category("Unit")]
        [Test]
        public void SocketChannel_Construct()
        {
            Action a = () => new SocketChannel(new Uri("localhost:5000"));
            a.ShouldNotThrow();        
        }
    }
}

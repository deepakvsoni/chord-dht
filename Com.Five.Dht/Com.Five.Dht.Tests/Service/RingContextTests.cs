namespace Com.Five.Dht.Tests.Service
{
    using Dht.Service;
    using NUnit.Framework;
    using FluentAssertions;

    [TestFixture]
    public class RingContextTests
    {
        [Category("Unit")]
        [Test]
        public void RingContext_StaticConstruct()
        {
            RingContext c = RingContext.Current;
            c.Factory.Should().NotBeNull();
            c.MaxNoOfBits.Should().BeGreaterThan(0);
            c.NoOfSuccessors.Should().BeGreaterThan(0);        
        }
    }
}

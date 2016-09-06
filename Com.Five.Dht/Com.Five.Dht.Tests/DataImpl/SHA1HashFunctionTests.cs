namespace Com.Five.Dht.Tests.DataImpl
{
    using Dht.Data;
    using Dht.DataImpl;
    using FluentAssertions;
    using NUnit.Framework;
    using System;
    using System.Text;

    [TestFixture]
    public class SHA1HashFunctionTests
    {
        [Category("Unit")]
        [Test]
        public void SHA1HashFunction_Construct()
        {
            Action c = () => new SHA1HashFunction();
            c.ShouldNotThrow(
                "because it instantiates default SHA1 crypto provider and does nothing else");
        }

        [Category("Unit")]
        [Test]
        public void SHA1HashFunction_ComputeHash()
        {
            IHashFunction function = new SHA1HashFunction();
            function.ComputeHash(Encoding.ASCII.GetBytes("Node123"))
                .Should().Equal(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231, 225
                    , 143, 185, 85, 104, 185, 11, 132, 160, 151 });
        }

        [Category("Unit")]
        [Test]
        public void SHA1HashFunction_Dispose()
        {
            SHA1HashFunction function = new SHA1HashFunction();
            Action a = () => function.Dispose();
            a.ShouldNotThrow();
        }
    }
}

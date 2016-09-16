namespace Com.Five.Dht.Tests.DataImpl
{
    using NUnit.Framework;
    using System;
    using FluentAssertions;
    using Dht.Data;
    using NSubstitute;
    using Dht.DataImpl;

    [TestFixture]
    public class IdGeneratorTests
    {
        IHashFunction _sHashFunction;

        [SetUp]
        public void Setup()
        {
            _sHashFunction = Substitute.For<IHashFunction>();
        }

        [Category("Unit")]
        [Test]
        public void IdGenerator_Construct()
        {
            _sHashFunction.Length.Returns(64);
            Action a = () => new IdGenerator(32,
                _sHashFunction);
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void IdGenerator_ConstructZeroMaxNoOfBits()
        {
            Action a = () => new IdGenerator(0,
                _sHashFunction);
            a.ShouldThrow<ArgumentException>();
        }

        [Category("Unit")]
        [Test]
        public void IdGenerator_ConstructNullHashFunction()
        {
            Action a = () => new IdGenerator(32,
                null);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void IdGenerator_ConstructMaxNoOfBitsGtThanHashFunctionLength()
        {
            _sHashFunction.Length.Returns(64);

            Action a = () => new IdGenerator(128,
                _sHashFunction);
            a.ShouldThrow<ArgumentException>();
        }

        [Category("Unit")]
        [Test]
        public void IdGenerator_ConstructMaxNoOfBitsEqHashFunctionLength()
        {
            _sHashFunction.Length.Returns(128);

            Action a = () => new IdGenerator(128,
                _sHashFunction);
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void IdGenerator_ConstructMaxNoOfBitsNotMultipleOf8()
        {
            _sHashFunction.Length.Returns(64);
            Action a = () => new IdGenerator(34,
                _sHashFunction);
            a.ShouldNotThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void IdGenerator_ConstructGenerate()
        {
            IdGenerator generator = new IdGenerator(32
                , new SHA1HashFunction());
            
            Id id = generator.Generate("sock://localhost:5000");

            id.Should().Equals(new Id(new byte[] { 15, 180, 234, 92 }, 32));
        }

        [Category("Unit")]
        [Test]
        public void IdGenerator_ConstructGenerateMaxNoOfBitsNotMultipleOf8()
        {
            IdGenerator generator = new IdGenerator(36
                , new SHA1HashFunction());

            Id id = generator.Generate("sock://localhost:5000");

            id.Should().Equals(new Id(new byte[] { 15, 180, 234, 92, 2 }, 36));
        }

        [Category("Unit")]
        [Test]
        public void IdGenerator_ConstructGenerateMaxNoOfBitsEqHashFunctionLength()
        {
            IdGenerator generator = new IdGenerator(160
                , new SHA1HashFunction());

            Id id = generator.Generate("sock://localhost:5000");

            id.Should().Equals(new Id(new byte[] { 15, 180, 234, 92, 130, 9
                , 38, 65, 103, 130, 22, 213, 135, 39, 46, 231, 51, 104, 41
                , 10 }, 160));
        }

        [Category("Unit")]
        [Test]
        public void IdGenerator_ConstructGenerateMaxNoOfBitsNotMultipleOf8_2()
        {
            IdGenerator generator = new IdGenerator(132
                , new SHA1HashFunction());

            Id id = generator.Generate("sock://localhost:5000");

            id.Should().Equals(new Id(new byte[] { 15, 180, 234, 92, 130, 9
                , 38, 65, 103, 130, 22, 213, 135, 39, 46, 231, 3 }, 132));
        }

        [Category("Unit")]
        [Test]
        public void IdGenerator_ConstructGenerateMaxNoOfBitsNotMultipleOf8_3()
        {
            IdGenerator generator = new IdGenerator(1
                , new SHA1HashFunction());

            Id id = generator.Generate("sock://localhost:5000");

            id.Should().Equals(new Id(new byte[] { 1 }, 1));
        }

        [Category("Unit")]
        [Test]
        public void IdGenerator_ConstructGenerateMaxNoOfBitsNotMultipleOf8_4()
        {
            IdGenerator generator = new IdGenerator(159
                , new SHA1HashFunction());

            Id id = generator.Generate("sock://localhost:5000");

            id.Should().Equals(new Id(new byte[] { 15, 180, 234, 92, 130, 9
                , 38, 65, 103, 130, 22, 213, 135, 39, 46, 231, 51, 104, 41
                , 10 }, 159));
        }
    }
}

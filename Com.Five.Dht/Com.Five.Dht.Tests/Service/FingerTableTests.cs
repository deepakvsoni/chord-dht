namespace Com.Five.Dht.Tests.Service
{
    using Dht.Service;
    using NUnit.Framework;
    using System;
    using FluentAssertions;

    [TestFixture]
    public class FingerTableTests
    {
        [Category("Unit")]
        [Test]
        public void FingerTable_ConstructZeroMaxNoOfBits()
        {
            Action a = () => new FingerTable(0);
            a.ShouldThrow<ArgumentException>();
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_Construct()
        {
            Action a = () => new FingerTable(10);
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_AddPowerOfTwo()
        {
            FingerTable table = new FingerTable(8);
            byte[] bytesAfterAddition = table.AddPowerOfTwo(
                new byte[] { 255 }, 8);
            bytesAfterAddition.Should().Equal(new byte[]
            {
                127
            });
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_AddPowerOfTwo_16Bit()
        {
            FingerTable table = new FingerTable(16);
            byte[] bytesAfterAddition = table.AddPowerOfTwo(
                new byte[] { 255, 0 }, 8);
            bytesAfterAddition.Should().Equal(new byte[]
            {
                127, 1
            });
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_AddPowerOfTwo_16Bit_2()
        {
            FingerTable table = new FingerTable(16);
            byte[] bytesAfterAddition = table.AddPowerOfTwo(
                new byte[] { 255, 1 }, 8);
            bytesAfterAddition.Should().Equal(new byte[]
            {
                127, 2
            });
        }


        [Category("Unit")]
        [Test]
        public void FingerTable_AddPowerOfTwo_64Bit()
        {
            FingerTable table = new FingerTable(64);
            byte[] bytesAfterAddition = table.AddPowerOfTwo(
                new byte[] { 255, 255, 0, 0, 0, 0, 0, 0 }, 8);
            bytesAfterAddition.Should().Equal(new byte[]
            {
                127, 0, 1, 0,0,0,0,0
            });
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_AddPowerOfTwoNullBytes()
        {
            FingerTable table = new FingerTable(8);
            Action a = () => table.AddPowerOfTwo(
               null, 8);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_AddPowerOfTwoZeroPower()
        {
            FingerTable table = new FingerTable(8);
            Action a = () => table.AddPowerOfTwo(
               new byte[] { 255 }, 0);
            a.ShouldThrow<ArgumentOutOfRangeException>();
        }


        [Category("Unit")]
        [Test]
        public void FingerTable_AddPowerOfTwo24Power()
        {
            FingerTable table = new FingerTable(8);
            Action a = () => table.AddPowerOfTwo(
               new byte[] { 255 }, 24);
            a.ShouldThrow<ArgumentOutOfRangeException>();
        }
    }
}
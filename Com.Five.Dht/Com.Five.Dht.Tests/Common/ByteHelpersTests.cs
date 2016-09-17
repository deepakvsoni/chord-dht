namespace Com.Five.Dht.Tests.Common
{
    using Dht.Common;
    using FluentAssertions;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestFixture]
    public class ByteHelpersTests
    {
        [Category("Unit")]
        [Test]
        public void ByteHelpers_IsBetween_NullThis()
        {
            byte[] b = null;
            Action a =()=> b.IsBetween(new byte[] { }, new byte[] { });
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void ByteHelpers_IsBetween_NullFrom()
        {
            byte[] b = new byte[0];
            Action a = () => b.IsBetween(null, new byte[] { });
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void ByteHelpers_IsBetween_NullTo()
        {
            byte[] b = new byte[0];
            Action a = () => b.IsBetween(new byte[] { }, null);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void ByteHelpers_IsBetween_1()
        {
            byte[] b = { 128 };
            b.IsBetween(new byte[] { 64 }, new byte[] { 255 })
                .Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void ByteHelpers_IsBetween_2()
        {
            byte[] b = { 64 };
            b.IsBetween(new byte[] { 64 }, new byte[] { 64 })
                .Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void ByteHelpers_IsBetween_3()
        {
            byte[] b = { 64 };
            b.IsBetween(new byte[] { 255 }, new byte[] { 64 })
                .Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void ByteHelpers_IsBetween_4()
        {
            byte[] b = { 155 };
            b.IsBetween(new byte[] { 128 }, new byte[] { 78 })
                .Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void ByteHelpers_IsBetween_Not_1()
        {
            byte[] b = { 32 };
            b.IsBetween(new byte[] { 64 }, new byte[] { 255 })
                .Should().BeFalse();
        }

        [Category("Unit")]
        [Test]
        public void ByteHelpers_IsBetween_Not_2()
        {
            byte[] b = { 32 };
            b.IsBetween(new byte[] { 64 }, new byte[] { 64 })
                .Should().BeFalse();
        }

        [Category("Unit")]
        [Test]
        public void ByteHelpers_IsBetween_Not_3()
        {
            byte[] b = { 32 };
            b.IsBetween(new byte[] { 244 }, new byte[] { 12 })
                .Should().BeFalse();
        }

        [Category("Unit")]
        [Test]
        public void ByteHelpers_IsBetween_Not_4()
        {
            byte[] b = { 32 };
            b.IsBetween(new byte[] { 48 }, new byte[] { 30 })
                .Should().BeFalse();
        }

        [Category("Unit")]
        [Test]
        public void ByteHelpers_CompareTo_NullThis()
        {
            byte[] b = null;
            Action a = () => b.CompareTo(new byte[0]);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void ByteHelpers_CompareTo_NullOther()
        {
            byte[] b = new byte[0];
            Action a = () => b.CompareTo(null);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void ByteHelpers_CompareTo_LengthMismatch()
        {
            Action a = () => new byte[1].CompareTo(new byte[3]);
            a.ShouldThrow<InvalidOperationException>();
        }

        [Category("Unit")]
        [Test]
        public void ByteHelpers_CompareTo_Equal()
        {
            (new byte[] { 122 }.CompareTo(new byte[] { 122 })).Should().Be(0);
        }

        [Category("Unit")]
        [Test]
        public void ByteHelpers_CompareTo_Less()
        {
            (new byte[] { 120 }.CompareTo(new byte[] { 122 })).Should().Be(-1);
        }


        [Category("Unit")]
        [Test]
        public void ByteHelpers_CompareTo_Greater()
        {
            (new byte[] { 120 }.CompareTo(new byte[] { 112 })).Should().Be(1);
        }

        [Category("Unit")]
        [Test]
        public void ByteHelpers_CompareTo_MEqual()
        {
            (new byte[] { 122, 244, 23 }.CompareTo(
                new byte[] { 122, 244, 23 })).Should().Be(0);
        }

        [Category("Unit")]
        [Test]
        public void ByteHelpers_CompareTo_MLess()
        {
            (new byte[] { 120, 244, 23 }.CompareTo(
                new byte[] { 122, 244, 23 })).Should().Be(-1);
        }

        [Category("Unit")]
        [Test]
        public void ByteHelpers_CompareTo_MGreater()
        {
            (new byte[] { 120, 244, 23 }.CompareTo(
                new byte[] { 112, 244, 23 })).Should().Be(1);
        }

        [Category("Unit")]
        [Test]
        public void ByteHelpers_CompareTo_MLess2()
        {
            (new byte[] { 122, 232, 23 }.CompareTo(
                new byte[] { 122, 244, 23 })).Should().Be(-1);
        }

        [Category("Unit")]
        [Test]
        public void ByteHelpers_CompareTo_MGreater2()
        {
            (new byte[] { 112, 245, 23 }.CompareTo(
                new byte[] { 112, 244, 23 })).Should().Be(1);
        }
    }
}

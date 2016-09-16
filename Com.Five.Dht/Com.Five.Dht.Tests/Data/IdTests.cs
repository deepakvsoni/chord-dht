namespace Com.Five.Dht.Tests.Data
{
    using Dht.Data;
    using FluentAssertions;
    using NUnit.Framework;
    using System;
    using System.Linq;
    using System.Collections.Generic;

    [TestFixture]
    public class IdTests
    {
        [Category("Unit")]
        [Test]
        public void Id_Construct()
        {
            Action a = () => new Id(new byte[] { 211, 122, 245, 12, 64, 160
                , 111, 239, 211, 231, 225, 143, 185, 85, 104, 185, 11, 132
                , 160, 151 }, 160);
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void Id_Equal_Transitive()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id3 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            id1.Equals(id2).Should().BeTrue();
            id2.Equals(id3).Should().BeTrue();
            id1.Equals(id3).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void Id_Equal_Commutative()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);

            id1.Equals(id2).Should().BeTrue();
            id2.Equals(id1).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void Id_Equal_Reflexive()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);

#pragma warning disable RECS0088 // Comparing equal expression for equality is usually useless
            id1.Equals(id1).Should().BeTrue();
#pragma warning restore RECS0088 // Comparing equal expression for equality is usually useless
        }

        [Category("Unit")]
        [Test]
        public void Id_ObjEqual_Transitive()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id3 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            id1.Equals((object)id2).Should().BeTrue();
            id2.Equals((object)id3).Should().BeTrue();
            id1.Equals((object)id3).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void Id_ObjEqual_Commutative()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);

            id1.Equals((object)id2).Should().BeTrue();
            id2.Equals((object)id1).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void Id_ObjEqual_Reflexive()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);

#pragma warning disable RECS0088 // Comparing equal expression for equality is usually useless
            id1.Equals((object)id1).Should().BeTrue();
#pragma warning restore RECS0088 // Comparing equal expression for equality is usually useless
        }

        [Category("Unit")]
        [Test]
        public void Id_ObjEqual_NotSameType()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);

            id1.Equals(new object()).Should().BeFalse();
        }

        [Category("Unit")]
        [Test]
        public void Id_GetHashcode()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);

            id1.GetHashCode().Should().Be(id2.GetHashCode());
        }

        [Category("Unit")]
        [Test]
        public void Id_CompareTo_Same_Transitive()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id3 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            id1.CompareTo(id2).Should().Equals(0);
            id2.CompareTo(id3).Should().Equals(0);
            id1.CompareTo(id3).Should().Equals(0);
        }

        [Category("Unit")]
        [Test]
        public void Id_CompareTo_Greater_Transitive()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 150, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id3 = new Id(new byte[] { 211, 122, 245, 12, 64, 140, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            id1.CompareTo(id2).Should().Equals(1);
            id2.CompareTo(id3).Should().Equals(1);
            id1.CompareTo(id3).Should().Equals(1);
        }

        [Category("Unit")]
        [Test]
        public void Id_CompareTo_Less_Transitive()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 140, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 150, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id3 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            id1.CompareTo(id2).Should().Equals(-1);
            id2.CompareTo(id3).Should().Equals(-1);
            id1.CompareTo(id3).Should().Equals(-1);
        }

        [Category("Unit")]
        [Test]
        public void Id_CompareTo_Same_Commutative()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);

            id1.CompareTo(id2).Should().Equals(0);
            id2.CompareTo(id1).Should().Equals(0);
        }

        [Category("Unit")]
        [Test]
        public void Id_CompareTo_Greater()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 150, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);

            id1.CompareTo(id2).Should().Equals(1);
            id2.CompareTo(id1).Should().Equals(-1);
        }

        [Category("Unit")]
        [Test]
        public void Id_CompareTo_UnEqualLength()
        {
            Id id1 = new Id(new byte[] { 211 }, 8);
            Id id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 150, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);

            Action a = () => id1.CompareTo(id2);
            a.ShouldThrow<InvalidOperationException>();
        }

        [Category("Unit")]
        [Test]
        public void Id_CompareTo_Less()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 150, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);

            id1.CompareTo(id2).Should().Equals(-1);
            id2.CompareTo(id1).Should().Equals(1);
        }

        [Category("Unit")]
        [Test]
        public void Id_CompareTo_Same_Reflexive()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);

            id1.CompareTo(id1).Should().Equals(0);
        }

        [Category("Unit")]
        [Test]
        public void Id_EqOp_Transitive()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id3 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);

            (id1 == id2).Should().BeTrue();
            (id2 == id3).Should().BeTrue();
            (id1 == id3).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void Id_EqOp_NotNullLhs()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);

            (id1 == (Id)null).Should().BeFalse();
        }

        [Category("Unit")]
        [Test]
        public void Id_EqOp_NotNullRhs()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);

            ((Id)null == id1).Should().BeFalse();
        }

        [Category("Unit")]
        [Test]
        public void Id_EqOp_Null()
        {
#pragma warning disable RECS0088 // Comparing equal expression for equality is usually useless
            ((Id)null == (Id)null).Should().BeTrue();
#pragma warning restore RECS0088 // Comparing equal expression for equality is usually useless
        }

        [Category("Unit")]
        [Test]
        public void Id_EqOp_Commutative()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);

            (id1 == id2).Should().BeTrue();
            (id2 == id1).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void Id_EqOp_Reflexive()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);

#pragma warning disable CS1718 // Comparison made to same variable
#pragma warning disable RECS0088 // Comparing equal expression for equality is usually useless
            (id1 == id1).Should().BeTrue();
#pragma warning restore RECS0088 // Comparing equal expression for equality is usually useless
#pragma warning restore CS1718 // Comparison made to same variable
        }

        [Category("Unit")]
        [Test]
        public void Id_GtOp()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 66, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id2 = new Id(new byte[] { 211, 122, 245, 12, 65, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);

            (id1 > id2).Should().BeTrue();
            (id2 < id1).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void Id_LtOp()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 63, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);

            (id1 < id2).Should().BeTrue();
            (id2 > id1).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void Id_NotEqOp()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 66, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            Id id2 = new Id(new byte[] { 211, 122, 245, 12, 65, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);

            (id1 != id2).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void Id_NotEqOp_NotNullLhs()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);
            (id1 != (Id)null).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void Id_NotEqOp_NotNullRhs()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239
                , 211, 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }
            , 160);

            ((Id)null != id1).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void Id_NotEqOp_Null()
        {
#pragma warning disable RECS0088 // Comparing equal expression for equality is usually useless
            ((Id)null != (Id)null).Should().BeFalse();
#pragma warning restore RECS0088 // Comparing equal expression for equality is usually useless
        }

        [Category("Unit")]
        [Test]
        public void Id_GetPowers_ZeroMaxNoOfBits()
        {
            Action a = () => Id.GetPowersOfTwo(0);
            a.ShouldThrow<ArgumentException>();
        }

        [Category("Unit")]
        [Test]
        public void Id_GetPowers_MaxBitsTwo()
        {
            IEnumerable<byte[]> powers = Id.GetPowersOfTwo(2);
            powers.Should().HaveCount(2);

            powers.ElementAt(0).Length.Should().Be(1);
            powers.ElementAt(1).Length.Should().Be(1);

            powers.ElementAt(0).Should().Equal(new byte[] { 1 });
            powers.ElementAt(1).Should().Equal(new byte[] { 2 });
        }

        [Category("Unit")]
        [Test]
        public void Id_GetPowers_MaxBitsTwenty()
        {
            IEnumerable<byte[]> powers = Id.GetPowersOfTwo(20);
            powers.Should().HaveCount(20);

            foreach(byte[] power in powers)
            {
                power.Length.Should().Be(3);
            }

            powers.ElementAt(0).Should().Equal(new byte[] { 1, 0, 0 });
            powers.ElementAt(1).Should().Equal(new byte[] { 2, 0, 0 });
            powers.ElementAt(2).Should().Equal(new byte[] { 4, 0, 0, });
            powers.ElementAt(3).Should().Equal(new byte[] { 8, 0, 0, });
            powers.ElementAt(4).Should().Equal(new byte[] { 16, 0, 0, });
            powers.ElementAt(5).Should().Equal(new byte[] { 32, 0, 0, });
            powers.ElementAt(6).Should().Equal(new byte[] { 64, 0, 0, });
            powers.ElementAt(7).Should().Equal(new byte[] { 128, 0, 0 });
            powers.ElementAt(8).Should().Equal(new byte[] { 0, 1, 0 });
            powers.ElementAt(9).Should().Equal(new byte[] { 0, 2, 0 });
            powers.ElementAt(10).Should().Equal(new byte[] { 0, 4, 0 });
            powers.ElementAt(11).Should().Equal(new byte[] { 0, 8, 0 });
            powers.ElementAt(12).Should().Equal(new byte[] { 0, 16, 0 });
            powers.ElementAt(13).Should().Equal(new byte[] { 0, 32, 0 });
            powers.ElementAt(14).Should().Equal(new byte[] { 0, 64, 0 });
            powers.ElementAt(15).Should().Equal(new byte[] { 0, 128, 0 });
            powers.ElementAt(16).Should().Equal(new byte[] { 0, 0, 1 });
            powers.ElementAt(17).Should().Equal(new byte[] { 0, 0, 2 });
            powers.ElementAt(18).Should().Equal(new byte[] { 0, 0, 4 });
            powers.ElementAt(19).Should().Equal(new byte[] { 0, 0, 8 });
        }

        [Category("Unit")]
        [Test]
        public void Id_GetPowers_MaxBits160()
        {
            IEnumerable<byte[]> powers = Id.GetPowersOfTwo(160);
            powers.Should().HaveCount(160);

            foreach (byte[] power in powers)
            {
                power.Length.Should().Be(20);
            }
        }
    }
}
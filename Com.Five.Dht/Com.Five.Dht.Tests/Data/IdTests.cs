namespace Com.Five.Dht.Tests.Data
{
    using Dht.Data;
    using FluentAssertions;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class IdTests
    {
        [Category("Unit")]
        [Test]
        public void Id_Construct()
        {
            Action a = () => new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void Id_Equal_Transitive()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id3 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            id1.Equals(id2).Should().BeTrue();
            id2.Equals(id3).Should().BeTrue();
            id1.Equals(id3).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void Id_Equal_Commutative()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

            id1.Equals(id2).Should().BeTrue();
            id2.Equals(id1).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void Id_Equal_Reflexive()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

#pragma warning disable RECS0088 // Comparing equal expression for equality is usually useless
            id1.Equals(id1).Should().BeTrue();
#pragma warning restore RECS0088 // Comparing equal expression for equality is usually useless
        }

        [Category("Unit")]
        [Test]
        public void Id_ObjEqual_Transitive()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id3 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            id1.Equals((object)id2).Should().BeTrue();
            id2.Equals((object)id3).Should().BeTrue();
            id1.Equals((object)id3).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void Id_ObjEqual_Commutative()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

            id1.Equals((object)id2).Should().BeTrue();
            id2.Equals((object)id1).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void Id_ObjEqual_Reflexive()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

#pragma warning disable RECS0088 // Comparing equal expression for equality is usually useless
            id1.Equals((object)id1).Should().BeTrue();
#pragma warning restore RECS0088 // Comparing equal expression for equality is usually useless
        }

        [Category("Unit")]
        [Test]
        public void Id_ObjEqual_NotSameType()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

            id1.Equals(new object()).Should().BeFalse();
        }

        [Category("Unit")]
        [Test]
        public void Id_GetHashcode()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

            id1.GetHashCode().Should().Be(id2.GetHashCode());
        }

        [Category("Unit")]
        [Test]
        public void Id_CompareTo_Same_Transitive()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id3 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            id1.CompareTo(id2).Should().Equals(0);
            id2.CompareTo(id3).Should().Equals(0);
            id1.CompareTo(id3).Should().Equals(0);
        }

        [Category("Unit")]
        [Test]
        public void Id_CompareTo_Greater_Transitive()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 150, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id3 = new Id(new byte[] { 211, 122, 245, 12, 64, 140, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            id1.CompareTo(id2).Should().Equals(1);
            id2.CompareTo(id3).Should().Equals(1);
            id1.CompareTo(id3).Should().Equals(1);
        }

        [Category("Unit")]
        [Test]
        public void Id_CompareTo_Less_Transitive()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 140, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 150, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id3 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            id1.CompareTo(id2).Should().Equals(-1);
            id2.CompareTo(id3).Should().Equals(-1);
            id1.CompareTo(id3).Should().Equals(-1);
        }

        [Category("Unit")]
        [Test]
        public void Id_CompareTo_Same_Commutative()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

            id1.CompareTo(id2).Should().Equals(0);
            id2.CompareTo(id1).Should().Equals(0);
        }

        [Category("Unit")]
        [Test]
        public void Id_CompareTo_Greater()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 150, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

            id1.CompareTo(id2).Should().Equals(1);
            id2.CompareTo(id1).Should().Equals(-1);
        }

        [Category("Unit")]
        [Test]
        public void Id_CompareTo_UnEqualLength()
        {
            var id1 = new Id(new byte[] { 211 });
            var id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 150, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

            Action a = () => id1.CompareTo(id2);
            a.ShouldThrow<InvalidOperationException>();
        }

        [Category("Unit")]
        [Test]
        public void Id_CompareTo_Less()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 150, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

            id1.CompareTo(id2).Should().Equals(-1);
            id2.CompareTo(id1).Should().Equals(1);
        }

        [Category("Unit")]
        [Test]
        public void Id_CompareTo_Same_Reflexive()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

            id1.CompareTo(id1).Should().Equals(0);
        }

        [Category("Unit")]
        [Test]
        public void Id_EqOp_Transitive()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id3 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

            (id1 == id2).Should().BeTrue();
            (id2 == id3).Should().BeTrue();
            (id1 == id3).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void Id_EqOp_NotNullLhs()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

            (id1 == (Id)null).Should().BeFalse();
        }

        [Category("Unit")]
        [Test]
        public void Id_EqOp_NotNullRhs()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

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
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

            (id1 == id2).Should().BeTrue();
            (id2 == id1).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void Id_EqOp_Reflexive()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

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
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 66, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id2 = new Id(new byte[] { 211, 122, 245, 12, 65, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

            (id1 > id2).Should().BeTrue();
            (id2 < id1).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void Id_LtOp()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 63, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id2 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

            (id1 < id2).Should().BeTrue();
            (id2 > id1).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void Id_NotEqOp()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 66, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            var id2 = new Id(new byte[] { 211, 122, 245, 12, 65, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

            (id1 != id2).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void Id_NotEqOp_NotNullLhs()
        {
            var id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });
            (id1 != (Id)null).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void Id_NotEqOp_NotNullRhs()
        {
            Id id1 = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

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
    }
}
namespace Com.Five.Dht.Tests.Data
{
    using Dht.Data;
    using FluentAssertions;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class StringKeyDataEntriesTests
    {
        Id id = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

        [Category("Unit")]
        [Test]
        public void StringKeyDataEntries_Construct()
        {
            Action a = () => new StringKeyDataEntries();
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void StringKeyDataEntries_InsertNullId()
        {
            StringKeyDataEntries entries = new StringKeyDataEntries();
            Action a = () => entries.Insert(null, "com.five.dht.name", "Chord dht");
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void StringKeyDataEntries_InsertNullKey()
        {
            StringKeyDataEntries entries = new StringKeyDataEntries();
            Action a = () => entries.Insert(id, null, "Chord dht");
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void StringKeyDataEntries_Insert()
        {
            StringKeyDataEntries entries = new StringKeyDataEntries();
            entries.Insert(id, "com.five.dht.name", "Chord dht").Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void StringKeyDataEntries_InsertMultiple()
        {
            StringKeyDataEntries entries = new StringKeyDataEntries();
            entries.Insert(id, "com.five.dht.firstname", "Chord").Should().BeTrue();
            entries.Insert(id, "com.five.dht.lastname", "Dht").Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public void StringKeyDataEntries_LookupNullId()
        {
            StringKeyDataEntries entries = new StringKeyDataEntries();

            Action a = () => entries.Lookup(null, "com.five.dht.lastname");
            a.ShouldThrow<ArgumentNullException>();
        }


        [Category("Unit")]
        [Test]
        public void StringKeyDataEntries_LookupNullKey()
        {
            StringKeyDataEntries entries = new StringKeyDataEntries();

            Action a = () => entries.Lookup(id, null);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void StringKeyDataEntries_Lookup()
        {
            StringKeyDataEntries entries = new StringKeyDataEntries();
            entries.Insert(id, "com.five.dht.firstname", "Chord").Should().BeTrue();
            entries.Insert(id, "com.five.dht.lastname", "Dht").Should().BeTrue();

            entries.Lookup(id, "com.five.dht.lastname").Should().Be("Dht");
        }

        [Category("Unit")]
        [Test]
        public void StringKeyDataEntries_LookupNonExistentId()
        {
            StringKeyDataEntries entries = new StringKeyDataEntries();
            entries.Insert(id, "com.five.dht.name", "Chord dht");

            entries.Lookup(new Id(new byte[] { 1 }), "com.five.dht.unknown").Should().Be(null);
        }

        [Category("Unit")]
        [Test]
        public void StringKeyDataEntries_LookupNonExistentKey()
        {
            StringKeyDataEntries entries = new StringKeyDataEntries();
            entries.Insert(id, "com.five.dht.name", "Chord dht");

            entries.Lookup(id, "com.five.dht.unknown").Should().Be(null);
        }
    }
}
namespace Com.Five.Dht.Tests.Data
{
    using DataImpl;
    using Dht.Data;
    using FluentAssertions;
    using NUnit.Framework;
    using System;
    using System.Threading.Tasks;

    [TestFixture]
    public class StringKeyDataEntriesTests
    {
        Id id = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211, 231
                , 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 });

        [Category("Unit")]
        [Test]
        public void StringKeyDataEntries_Construct()
        {
            Action a = () => new DataEntries();
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public async Task StringKeyDataEntries_InsertNullId()
        {
            DataEntries entries = new DataEntries();
            Func<Task> a = async () =>
            {
                await entries.Insert(null, "com.five.dht.name", "Chord dht");
            };
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public async Task StringKeyDataEntries_InsertNullKey()
        {
            DataEntries entries = new DataEntries();
            Func<Task> a = async () =>
            {
                await entries.Insert(id, null, "Chord dht");
            };
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public async Task StringKeyDataEntries_Insert()
        {
            DataEntries entries = new DataEntries();
            (await entries.Insert(id, "com.five.dht.name", "Chord dht")).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public async Task StringKeyDataEntries_InsertMultiple()
        {
            DataEntries entries = new DataEntries();
            (await entries.Insert(id, "com.five.dht.firstname", "Chord")).Should().BeTrue();
            (await entries.Insert(id, "com.five.dht.lastname", "Dht")).Should().BeTrue();
        }

        [Category("Unit")]
        [Test]
        public async Task StringKeyDataEntries_LookupNullId()
        {
            DataEntries entries = new DataEntries();

            Func<Task> a = async () =>
            {
                await entries.Lookup(null, "com.five.dht.lastname");
            };
            a.ShouldThrow<ArgumentNullException>();
        }


        [Category("Unit")]
        [Test]
        public async Task StringKeyDataEntries_LookupNullKey()
        {
            DataEntries entries = new DataEntries();

            Func<Task> a = async () =>
            {
                await entries.Lookup(id, null);
            };
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public async Task StringKeyDataEntries_Lookup()
        {
            DataEntries entries = new DataEntries();
            (await entries.Insert(id, "com.five.dht.firstname", "Chord")).Should().BeTrue();
            (await entries.Insert(id, "com.five.dht.lastname", "Dht")).Should().BeTrue();

            (await entries.Lookup(id, "com.five.dht.lastname")).Should().Be("Dht");
        }

        [Category("Unit")]
        [Test]
        public async Task StringKeyDataEntries_LookupNonExistentId()
        {
            DataEntries entries = new DataEntries();
            await entries.Insert(id, "com.five.dht.name", "Chord dht");

            (await entries.Lookup(new Id(new byte[] { 1 }), "com.five.dht.unknown"))
                .Should().Be(null);
        }

        [Category("Unit")]
        [Test]
        public async Task StringKeyDataEntries_LookupNonExistentKey()
        {
            DataEntries entries = new DataEntries();
            await entries.Insert(id, "com.five.dht.name", "Chord dht");

            (await entries.Lookup(id, "com.five.dht.unknown")).Should().Be(null);
        }

        [Category("Unit")]
        [Test]
        public async Task StringKeyDataEntries_RemoveNullId()
        {
            DataEntries entries = new DataEntries();

            Func<Task> a = async () =>
            {
                await entries.Remove(null, "com.five.dht.lastname");
            };
            a.ShouldThrow<ArgumentNullException>();
        }


        [Category("Unit")]
        [Test]
        public async Task StringKeyDataEntries_RemoveNullKey()
        {
            DataEntries entries = new DataEntries();

            Func<Task> a = async () =>
            {
                await entries.Remove(id, null);
            };
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public async Task StringKeyDataEntries_Remove()
        {
            DataEntries entries = new DataEntries();
            (await entries.Insert(id, "com.five.dht.firstname", "Chord")).Should().BeTrue();
            (await entries.Insert(id, "com.five.dht.lastname", "Dht")).Should().BeTrue();

            (await entries.Lookup(id, "com.five.dht.lastname")).Should().Be("Dht");

            await entries.Remove(id, "com.five.dht.lastname");

            (await entries.Lookup(id, "com.five.dht.lastname")).Should().Be(null);
        }
    }
}
namespace Com.Five.Dht.Tests.DataImpl
{
    using Dht.DataImpl;
    using FluentAssertions;
    using NUnit.Framework;
    using System;
    using System.Threading.Tasks;

    [TestFixture]
    public class DataEntriesTests
    {
        [Category("Unit")]
        [Test]
        public void DataEntries_Construct()
        {
            Action a = () => new DataEntries();
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public async Task DataEntries_InsertNullKey()
        {
            DataEntries entries = new DataEntries();
            Func<Task> a = async () =>
            {
                await entries.Put(null, "Chord dht");
            };
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public async Task DataEntries_InsertNullValue()
        {
            DataEntries entries = new DataEntries();
            Func<Task> a = async () =>
            {
                await entries.Put("com.five.dht.name", null);
            };
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public async Task DataEntries_Insert()
        {
            DataEntries entries = new DataEntries();
            (await entries.Put("com.five.dht.name", "Chord dht")).Should()
                .BeTrue();
        }

        [Category("Unit")]
        [Test]
        public async Task DataEntries_InsertMultiple()
        {
            DataEntries entries = new DataEntries();
            (await entries.Put("com.five.dht.firstname", "Chord")).Should()
                .BeTrue();
            (await entries.Put("com.five.dht.lastname", "Dht")).Should()
                .BeTrue();
        }

        [Category("Unit")]
        [Test]
        public async Task DataEntries_LookupNullKey()
        {
            DataEntries entries = new DataEntries();

            Func<Task> a = async () =>
            {
                await entries.Get(null);
            };
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public async Task DataEntries_Lookup()
        {
            DataEntries entries = new DataEntries();
            (await entries.Put("com.five.dht.firstname", "Chord")).Should()
                .BeTrue();
            (await entries.Put("com.five.dht.lastname", "Dht")).Should()
                .BeTrue();

            (await entries.Get("com.five.dht.lastname")).Should().Be("Dht");
        }

        [Category("Unit")]
        [Test]
        public async Task DataEntries_LookupNonExistentKey()
        {
            DataEntries entries = new DataEntries();
            await entries.Put("com.five.dht.name", "Chord dht");

            (await entries.Get("com.five.dht.unknown")).Should().Be(null);
        }

        [Category("Unit")]
        [Test]
        public async Task DataEntries_RemoveNullKey()
        {
            DataEntries entries = new DataEntries();

            Func<Task> a = async () =>
            {
                await entries.Remove(null);
            };
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public async Task DataEntries_Remove()
        {
            DataEntries entries = new DataEntries();
            (await entries.Put("com.five.dht.firstname", "Chord")).Should()
                .BeTrue();
            (await entries.Put("com.five.dht.lastname", "Dht")).Should()
                .BeTrue();

            (await entries.Get("com.five.dht.lastname")).Should().Be("Dht");

            (await entries.Remove("com.five.dht.lastname")).Should().BeTrue();

            (await entries.Get("com.five.dht.lastname")).Should().Be(null);
        }
    }
}
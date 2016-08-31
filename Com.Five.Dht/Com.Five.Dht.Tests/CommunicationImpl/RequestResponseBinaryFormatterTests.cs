namespace Com.Five.Dht.Tests.CommunicationImpl
{
    using Dht.CommunicationImpl;
    using NUnit.Framework;
    using System;
    using FluentAssertions;
    using Communication.Requests;
    using System.Collections.Generic;

    [TestFixture]
    public class RequestResponseBinaryFormatterTests
    {
        [Category("Unit")]
        [Test]        
        public void RequestResponseBinaryFormatter_Construct()
        {
            Action a = () => new RequestResponseBinaryFormatter();
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void RequestResponseBinaryFormatter_GetBytes()
        {
            RequestResponseBinaryFormatter formatter
                = new RequestResponseBinaryFormatter();

            Request req = new Shutdown();

            byte[] bytes = formatter.GetBytes(req);

            bytes.Should().NotBeNull();
            bytes.Should().NotBeEmpty();
        }

        [Category("Unit")]
        [Test]
        public void RequestResponseBinaryFormatter_GetObject()
        {
            RequestResponseBinaryFormatter formatter
                = new RequestResponseBinaryFormatter();

            Request req = new Shutdown();

            byte[] bytes = formatter.GetBytes(req);

            object obj = formatter.GetObject(bytes.Length
                , new List<ArraySegment<byte>>(
                    new[] { new ArraySegment<byte>(bytes)}));

            obj.Should().NotBeNull();
            obj.Should().BeAssignableTo<Shutdown>();
        }
    }
}

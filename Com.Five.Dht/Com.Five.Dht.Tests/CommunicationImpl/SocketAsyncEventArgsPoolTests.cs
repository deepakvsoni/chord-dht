namespace Com.Five.Dht.Tests.CommunicationImpl
{
    using Dht.CommunicationImpl;
    using NUnit.Framework;
    using System;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Net.Sockets;

    [TestFixture]
    public class SocketAsyncEventArgsPoolTests
    {
        [Category("Unit")]
        [Test]
        public void SocketAsyncEventArgsPool_Construct()
        {
            Action a = () => new SocketAsyncEventArgsPool(
                Guid.NewGuid().ToString(), 10, null);
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void SocketAsyncEventArgsPool_Initialize()
        {
            SocketAsyncEventArgsPool pool = new SocketAsyncEventArgsPool(
                Guid.NewGuid().ToString(), 10, null);
            pool.Initialize();

            pool.Count.Should().Be(10);
        }

        [Category("Unit")]
        [Test]
        public void SocketAsyncEventArgsPool_InitializeMultiple()
        {
            SocketAsyncEventArgsPool pool = new SocketAsyncEventArgsPool(
                Guid.NewGuid().ToString(), 10, null);
            pool.Initialize();
            pool.Initialize();

            pool.Count.Should().Be(10);
        }

        [Category("Unit")]
        [Test]
        public void SocketAsyncEventArgsPool_InitializeAcrossThreads()
        {
            SocketAsyncEventArgsPool pool = new SocketAsyncEventArgsPool(
                Guid.NewGuid().ToString(), 10, null);

            Parallel.Invoke(() =>
            {
                pool.Initialize();
            }, 
            () =>
            {
                pool.Initialize();
            });

            pool.Count.Should().Be(10);
        }

        [Category("Unit")]
        [Test]
        public void SocketAsyncEventArgsPool_Pop()
        {
            SocketAsyncEventArgsPool pool = new SocketAsyncEventArgsPool(
                Guid.NewGuid().ToString(), 10, null);
            pool.Initialize();

            pool.Pop().Should().NotBeNull();
        }

        [Category("Unit")]
        [Test]
        public void SocketAsyncEventArgsPool_PopThread()
        {
            SocketAsyncEventArgsPool pool = new SocketAsyncEventArgsPool(
                Guid.NewGuid().ToString(), 10, null);
            pool.Initialize();

            Action a = () =>
            {
                pool.Pop();
            };

            Parallel.Invoke(a, a);

            pool.Count.Should().Be(8);
        }

        [Category("Unit")]
        [Test]
        public void SocketAsyncEventArgsPool_PopNull()
        {
            SocketAsyncEventArgsPool pool = new SocketAsyncEventArgsPool(
                Guid.NewGuid().ToString(), 10, null);
            pool.Initialize();

            for(int i = 0; i < 10; ++i)
            {
                pool.Pop();
            }

            pool.Pop().Should().Be(null);
        }

        [Category("Unit")]
        [Test]
        public void SocketAsyncEventArgsPool_Push()
        {
            SocketAsyncEventArgsPool pool = new SocketAsyncEventArgsPool(
                Guid.NewGuid().ToString(), 10, null);
            pool.Initialize();

            SocketAsyncEventArgs popped = pool.Pop();
            pool.Count.Should().Be(9);

            pool.Push(popped);

            pool.Count.Should().Be(10);
        }

        [Category("Unit")]
        [Test]
        public void SocketAsyncEventArgsPool_PushThread()
        {
            SocketAsyncEventArgsPool pool = new SocketAsyncEventArgsPool(
                Guid.NewGuid().ToString(), 10, null);
            pool.Initialize();

            Action a = () =>
            {
                SocketAsyncEventArgs p1 = pool.Pop();
                pool.Push(p1);
            };

            Parallel.Invoke(a, a);

            pool.Count.Should().Be(10);
        }

        [Category("Unit")]
        [Test]
        public void SocketAsyncEventArgsPool_PushNull()
        {
            SocketAsyncEventArgsPool pool = new SocketAsyncEventArgsPool(
                Guid.NewGuid().ToString(), 10, null);
            pool.Initialize();

            SocketAsyncEventArgs popped = pool.Pop();

            Action a = () => pool.Push(null);
            a.ShouldThrow<ArgumentNullException> ();
        }
    }
}
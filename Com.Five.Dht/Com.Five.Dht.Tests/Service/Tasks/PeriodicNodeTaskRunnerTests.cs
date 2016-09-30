namespace Com.Five.Dht.Tests.Service.Tasks
{
    using Dht.Service.Tasks;
    using FluentAssertions;
    using NSubstitute;
    using NUnit.Framework;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    [TestFixture]
    public class PeriodicNodeTaskRunnerTests
    {
        TimeSpan s = TimeSpan.FromSeconds(3);

        [Category("Unit")]
        [Test]
        public void PeriodicNodeTaskRunner_ConstructNullNodeTask()
        {
            Action a = () => new PeriodicNodeTaskRunner(null, s);
            a.ShouldThrowExactly<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void PeriodicNodeTaskRunner_Construct()
        {
            INodeTask t = Substitute.For<INodeTask>();

            Action a = () => new PeriodicNodeTaskRunner(t, s);
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void PeriodicNodeTaskRunner_Start()
        {
            INodeTask t = Substitute.For<INodeTask>();
            AutoResetEvent e = new AutoResetEvent(false);
            CancellationTokenSource source = new CancellationTokenSource();

            t.When(x => x.Run(source.Token))
                .Do(x =>
                {
                    e.Set();
                });

            PeriodicNodeTaskRunner r = new PeriodicNodeTaskRunner(t, s);

            r.Start(source.Token);

            r.StopHandle.IsSet.Should().BeFalse();

            r.Status.Should().Be(NodeTaskRunnerStatus.Running);

            e.WaitOne();

            source.Cancel();

            r.StopHandle.Wait(5000);


            t.Received().Run(source.Token);
        }

        [Category("Unit")]
        [Test]
        public void PeriodicNodeTaskRunner_StartTwice()
        {
            INodeTask t = Substitute.For<INodeTask>();

            CancellationTokenSource source = new CancellationTokenSource();

            PeriodicNodeTaskRunner r = new PeriodicNodeTaskRunner(t, s);

            r.Start(source.Token);

            r.StopHandle.IsSet.Should().BeFalse();

            r.Status.Should().Be(NodeTaskRunnerStatus.Running);

            Action a = () => r.Start(source.Token);
            a.ShouldThrowExactly<InvalidOperationException>();

            Task.Delay(5000).Wait();

            source.Cancel();

            r.StopHandle.Wait(5000);

            t.Received().Run(source.Token);
        }

        [Category("Unit")]
        [Test]
        public void PeriodicNodeTaskRunner_StartTwiceParallel()
        {
            INodeTask t = Substitute.For<INodeTask>();

            CancellationTokenSource source = new CancellationTokenSource();

            PeriodicNodeTaskRunner r = new PeriodicNodeTaskRunner(t, s);

            Action a = () => r.Start(source.Token);

            //Just trying to reach the second throw exception block.
            Action b = () => Parallel.Invoke(a, a, a, a, a, a);

            b.ShouldThrow<InvalidOperationException>();

            r.StopHandle.IsSet.Should().BeFalse();

            r.Status.Should().Be(NodeTaskRunnerStatus.Running);

            Task.Delay(5000).Wait();

            source.Cancel();

            r.StopHandle.Wait(5000);

            t.Received().Run(source.Token);
        }

        [Category("Unit")]
        [Test]
        public void PeriodicNodeTaskRunner_StartRunException()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            AutoResetEvent e = new AutoResetEvent(false);

            INodeTask t = Substitute.For<INodeTask>();
            t.When(x => x.Run(source.Token)).Do(x =>
            {
                e.Set();
                throw new Exception();
            });

            PeriodicNodeTaskRunner r = new PeriodicNodeTaskRunner(t, s);

            r.Start(source.Token);

            e.WaitOne(5000);

            r.StopHandle.IsSet.Should().BeFalse();

            r.Status.Should().Be(NodeTaskRunnerStatus.Running);

            source.Cancel();

            r.StopHandle.Wait(5000);

            t.Received().Run(source.Token);
        }
    }
}
namespace Com.Five.Dht.Tests.Service.Tasks
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using NSubstitute;
    using NUnit.Framework;
    using Dht.Service.Tasks;
    using System.Threading;

    [TestFixture]
    public class ContinuousNodeTaskRunnerTests
    {
        [Category("Unit")]
        [Test]
        public void ContinuousNodeTaskRunner_ConstructNullNodeTask()
        {
            Action a = () => new ContinuousNodeTaskRunner(null);
            a.ShouldThrowExactly<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void ContinuousNodeTaskRunner_Construct()
        {
            INodeTask t = Substitute.For<INodeTask>();

            Action a = () => new ContinuousNodeTaskRunner(t);
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void ContinuousNodeTaskRunner_Start()
        {
            INodeTask t = Substitute.For<INodeTask>();
            AutoResetEvent e = new AutoResetEvent(false);
            CancellationTokenSource source = new CancellationTokenSource();

            t.When(x => x.Run(source.Token))
                .Do(x =>
                {
                    e.Set();
                });

            ContinuousNodeTaskRunner r = new ContinuousNodeTaskRunner(t);

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
        public void ContinuousNodeTaskRunner_StartTwice()
        {
            INodeTask t = Substitute.For<INodeTask>();

            CancellationTokenSource source = new CancellationTokenSource();

            ContinuousNodeTaskRunner r = new ContinuousNodeTaskRunner(t);

            r.Start(source.Token);

            r.StopHandle.IsSet.Should().BeFalse();

            r.Status.Should().Be(NodeTaskRunnerStatus.Running);

            Action a = () => r.Start(source.Token);
            a.ShouldThrowExactly<InvalidOperationException>();

            source.Cancel();

            r.StopHandle.Wait(5000);

            t.Received().Run(source.Token);
        }

        [Category("Unit")]
        [Test]
        public void ContinuousNodeTaskRunner_StartTwiceParallel()
        {
            INodeTask t = Substitute.For<INodeTask>();

            CancellationTokenSource source = new CancellationTokenSource();

            ContinuousNodeTaskRunner r = new ContinuousNodeTaskRunner(t);

            Action a = () => r.Start(source.Token);

            //Just trying to reach the second throw exception block.
            Action b = () => Parallel.Invoke(a, a, a, a, a, a);

            b.ShouldThrow<InvalidOperationException>();
            
            r.StopHandle.IsSet.Should().BeFalse();

            r.Status.Should().Be(NodeTaskRunnerStatus.Running);

            source.Cancel();

            r.StopHandle.Wait(5000);

            t.Received().Run(source.Token);
        }

        [Category("Unit")]
        [Test]
        public void ContinuousNodeTaskRunner_StartRunException()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            AutoResetEvent e = new AutoResetEvent(false);

            INodeTask t = Substitute.For<INodeTask>();
            t.When(x => x.Run(source.Token)).Do(x =>
            {
                e.Set();
                throw new Exception();
            });

            ContinuousNodeTaskRunner r = new ContinuousNodeTaskRunner(t);

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
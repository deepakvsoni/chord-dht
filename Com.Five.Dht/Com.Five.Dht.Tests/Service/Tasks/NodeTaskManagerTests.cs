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
    public class NodeTaskManagerTests
    {
        [Category("Unit")]
        [Test]
        public void NodeTaskManager_Construct()
        {
            Action a = () => new NodeTaskManager();
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void NodeTaskManager_ExecutePeriodicallyNull()
        {
            NodeTaskManager tm = new NodeTaskManager();
            Action a = () => tm.ExecutePeriodically(null, TimeSpan.MaxValue);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void NodeTaskManager_ExecuteContinuouslyNull()
        {
            NodeTaskManager tm = new NodeTaskManager();
            Action a = () => tm.ExecuteContinuously(null);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void NodeTaskManager_ExecutePeriodically()
        {
            INodeTask t = Substitute.For<INodeTask>();

            NodeTaskManager tm = new NodeTaskManager();

            tm.ExecutePeriodically(t, TimeSpan.FromSeconds(3));

            Task.Delay(5000).Wait();

            foreach (INodeTaskRunner tr in tm.Tasks)
            {
                tr.Status.Should().Be(NodeTaskRunnerStatus.Running);
            }

            tm.StopAllTasks();

            t.Received().Run(Arg.Any<CancellationToken>());
        }

        [Category("Unit")]
        [Test]
        public void NodeTaskManager_ExecuteContinuously()
        {
            INodeTask t = Substitute.For<INodeTask>();

            NodeTaskManager tm = new NodeTaskManager();

            tm.ExecuteContinuously(t);

            Task.Delay(5000).Wait();

            foreach (INodeTaskRunner tr in tm.Tasks)
            {
                tr.Status.Should().Be(NodeTaskRunnerStatus.Running);
            }

            tm.StopAllTasks();

            t.Received().Run(Arg.Any<CancellationToken>());
        }

        [Category("Unit")]
        [Test]
        public void NodeTaskManager_ExecuteMultiple()
        {
            INodeTask t = Substitute.For<INodeTask>();
            INodeTask t2 = Substitute.For<INodeTask>();

            NodeTaskManager tm = new NodeTaskManager();

            tm.ExecuteContinuously(t2);
            tm.ExecutePeriodically(t, TimeSpan.FromSeconds(2));

            Task.Delay(5000).Wait();

            foreach(INodeTaskRunner tr in tm.Tasks)
            {
                tr.Status.Should().Be(NodeTaskRunnerStatus.Running);
            }

            tm.StopAllTasks();

            t.Received().Run(Arg.Any<CancellationToken>());
            t2.Received().Run(Arg.Any<CancellationToken>());

        }
    }
}
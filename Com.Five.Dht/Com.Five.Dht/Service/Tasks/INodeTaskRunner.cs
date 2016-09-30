namespace Com.Five.Dht.Service.Tasks
{
    using System.Threading;

    public interface INodeTaskRunner
    {
        NodeTaskRunnerStatus Status { get; }

        ManualResetEventSlim StopHandle { get; }

        bool Start(CancellationToken source);
    }

    public enum NodeTaskRunnerStatus
{
        NotStarted,
        Running,
        Stopped
    }
}
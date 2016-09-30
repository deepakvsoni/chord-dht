namespace Com.Five.Dht.Service.Tasks
{
    using System.Threading;

    public interface INodeTask
    {
        string Name { get; }

        void Run(CancellationToken token);
    }
}

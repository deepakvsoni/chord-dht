namespace Com.Five.Dht.Service.Tasks
{
    using log4net;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class ContinuousNodeTaskRunner : INodeTaskRunner
    {
        ILog _l;

        object _lock = new object();
        INodeTask _task;

        public ContinuousNodeTaskRunner(INodeTask task)
        {
            if (null == task)
            {
                throw new ArgumentNullException(nameof(task));
            }
            _task = task;
            _l = LogManager.GetLogger(
                string.Format("Task runner {0}", task.Name));
            StopHandle = new ManualResetEventSlim();
        }

        public NodeTaskRunnerStatus Status
        {
            get;
            private set;
        }

        public ManualResetEventSlim StopHandle
        {
            get;
            private set;
        }

        public bool Start(CancellationToken source)
        {
            if (Status == NodeTaskRunnerStatus.Running)
            {
                throw new InvalidOperationException("Task already running.");
            }
            lock (_lock)
            {
                if (Status == NodeTaskRunnerStatus.Running)
                {
                    throw new InvalidOperationException(
                        "Task already running.");
                }
                Task.Factory.StartNew(() =>
                {
                    StopHandle.Reset();

                    while (!source.IsCancellationRequested)
                    {
                        try
                        {
                            _task.Run(source);
                        }
                        catch (Exception e)
                        {
                            _l.Error("Error running task.", e);
                        }
                    }

                    _l.Info("Task stopped.");
                    Status = NodeTaskRunnerStatus.Stopped;
                    StopHandle.Set();
                }, source);
                Status = NodeTaskRunnerStatus.Running;
            }
            return true;
        }
    }
}
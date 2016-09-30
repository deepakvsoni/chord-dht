namespace Com.Five.Dht.Service.Tasks
{
    using log4net;
    using System;
    using System.Threading;

    public class PeriodicNodeTaskRunner : INodeTaskRunner
    {
        object _lock = new object();

        ILog _l;

        INodeTask _task;
        TimeSpan _period;
        System.Timers.Timer _timer;

        public PeriodicNodeTaskRunner(INodeTask task, TimeSpan period)
        {
            if (null == task)
            {
                throw new ArgumentNullException(nameof(task));
            }
            _l = LogManager.GetLogger(string.Format("Periodic Task {0}"
                    , task.Name));

            _task = task;
            _period = period;

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
                _timer = new System.Timers.Timer(_period.TotalMilliseconds);
                _timer.Elapsed += (s, e) =>
                {
                    if (!source.IsCancellationRequested)
                    {
                        try
                        {
                            _task.Run(source);
                        }
                        catch (Exception ex)
                        {
                            _l.Error("Error running task.", ex);
                        }
                        return;
                    }

                    _timer.Stop();

                    StopHandle.Set();

                    _timer.Dispose();
                    _l.Info("Task stopped.");
                };
                _timer.Start();

                Status = NodeTaskRunnerStatus.Running;
            }
            return true;
        }
    }
}
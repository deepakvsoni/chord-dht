namespace Com.Five.Dht.Service.Tasks
{
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Linq;

    public class NodeTaskManager
    {
        ILog _l;

        CancellationTokenSource _source = new CancellationTokenSource();
        List<INodeTaskRunner> _tasks = new List<INodeTaskRunner>();

        public IReadOnlyCollection<INodeTaskRunner> Tasks
        {
            get
            {
                return  _tasks.ToList();
            }
        }

        public NodeTaskManager()
        {
            _l = LogManager.GetLogger(string.Format("[Task Manager {0}]"
                    , Guid.NewGuid()));
        }

        public bool ExecutePeriodically(INodeTask task, TimeSpan period)
        {
            if (null == task)
            {
                throw new ArgumentNullException(nameof(task));
            }
            _l.InfoFormat("Adding task {0} to be executed periodically.",
                task.Name);

            PeriodicNodeTaskRunner runner = new PeriodicNodeTaskRunner(task
                , period);
            return StartTask(task, runner);

        }

        public bool ExecuteContinuously(INodeTask task)
        {
            if (null == task)
            {
                throw new ArgumentNullException(nameof(task));
            }
            _l.InfoFormat("Adding task {0} to be executed continuously.",
                task.Name);

            ContinuousNodeTaskRunner runner
                = new ContinuousNodeTaskRunner(task);
            return StartTask(task, runner);
        }

        bool StartTask(INodeTask task, INodeTaskRunner runner)
        {
            try
            {
                _l.DebugFormat("Starting task {0}.", task.Name);

                if (runner.Start(_source.Token))
                {
                    _l.DebugFormat("Task started {0}.", task.Name);

                    _tasks.Add(runner);

                    _l.DebugFormat(
                        "Task {0} added to list of tasks I'm managing.",
                        task.Name);

                    return true;
                }
                _l.InfoFormat("Could not start task {0}.", task.Name);

                return false;
            }
            catch (Exception ex)
            {
                _l.Error(string.Format("Error starting task {0}", task.Name)
                    , ex);
                return false;
            }
        }

        public void StopAllTasks()
        {
            _l.Info("Requesting stop to all task runners.");

            _source.Cancel();

            _l.Debug("Request to stop sent.");

            List<WaitHandle> stopHandles = new List<WaitHandle>();

            _tasks.ForEach(t => stopHandles.Add(t.StopHandle.WaitHandle));

            WaitHandle.WaitAll(stopHandles.ToArray(), 10000);

            _l.Info("All tasks stopped.");
        }
    }
}
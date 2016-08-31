namespace Com.Five.Dht.DataImpl
{
    using Data;
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    public class DataEntries : IDataEntries
    {
        ConcurrentDictionary<string, object> _entries
            = new ConcurrentDictionary<string, object>();

        public Task<bool> Put(string key, object val)
        {
            return Task.Factory.StartNew(() =>
            {
                if (null == key)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                if (null == val)
                {
                    throw new ArgumentNullException(nameof(val));
                }
                _entries[key] = val;
                return true;
            });
        }

        public Task<object> Get(string key)
        {
            return Task.Factory.StartNew<object>(() =>
            {
                if (null == key)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                object val = null;
                if (_entries.TryGetValue(key, out val))
                {
                    return val;
                }
                return val;
            });
        }

        public Task<bool> Remove(string key)
        {
            return Task.Factory.StartNew(() =>
            {
                if (null == key)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                object val;
                return _entries.TryRemove(key, out val);
            });
        }
    }
}
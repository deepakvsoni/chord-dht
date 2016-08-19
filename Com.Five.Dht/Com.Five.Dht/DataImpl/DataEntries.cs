namespace Com.Five.Dht.DataImpl
{
    using Data;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class DataEntries : IDataEntries
    {
        Dictionary<Id, Dictionary<string, object>> _entries 
            = new Dictionary<Id, Dictionary<string, object>>();

        public Task<bool> Insert(Id id, string key, object val)
        {
            return Task.Factory.StartNew<bool>(() =>
            {
                if (null == id)
                {
                    throw new ArgumentNullException(nameof(id));
                }
                if (null == key)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                lock (_entries)
                {
                    Dictionary<string, object> values;
                    if (!_entries.TryGetValue(id, out values))
                    {
                        values = new Dictionary<string, object>();
                        _entries[id] = values;
                    }
                    values[key] = val;
                }
                return true;
            });
        }

        public Task<object> Lookup(Id id, string key)
        {
            return Task.Factory.StartNew<object>(() =>
            {
                if (null == id)
                {
                    throw new ArgumentNullException(nameof(id));
                }
                if (null == key)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                lock (_entries)
                {
                    Dictionary<string, object> values;
                    if (_entries.TryGetValue(id, out values))
                    {
                        object val;
                        if (values.TryGetValue(key, out val))
                        {
                            return val;
                        }
                    }
                }
                return null;
            });
        }

        public Task Remove(Id id, string key)
        {
            return Task.Factory.StartNew(() =>
            {
                if (null == id)
                {
                    throw new ArgumentNullException(nameof(id));
                }
                if (null == key)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                lock (_entries)
                {
                    Dictionary<string, object> values;
                    if (_entries.TryGetValue(id, out values))
                    {
                        values.Remove(key);
                    }
                }
            });
        }
    }
}

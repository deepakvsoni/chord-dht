namespace Com.Five.Dht.Data
{
    using System;
    using System.Collections.Generic;

    public class StringKeyDataEntries : IDataEntries<string>
    {
        Dictionary<Id, Dictionary<string, object>> _entries 
            = new Dictionary<Id, Dictionary<string, object>>();

        public bool Insert(Id id, string key, object val)
        {
            if(null == id)
            {
                throw new ArgumentNullException(nameof(id));
            }
            if(null == key)
            {
                throw new ArgumentNullException(nameof(key));
            }
            lock (_entries)
            {
                Dictionary<string, object> values;
                if(!_entries.TryGetValue(id, out values))
                {
                    values = new Dictionary<string, object>();
                    _entries[id] = values;
                }
                values[key] = val;
            }
            return true;
        }

        public object Lookup(Id id, string key)
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
                    if(values.TryGetValue(key, out val))
                    {
                        return val;
                    }
                }
            }
            return null;
        }
    }
}

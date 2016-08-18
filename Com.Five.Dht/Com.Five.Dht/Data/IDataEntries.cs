namespace Com.Five.Dht.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IDataEntries<T>
    {
        bool Insert(Id id, T key, object val);

        object Lookup(Id id, T key);
    }
}

namespace Com.Five.Dht.Data
{
    using System.Threading.Tasks;

    public interface IDataEntries
    {
        Task<bool> Insert(Id id, string key, object val);

        Task<object> Lookup(Id id, string key);

        Task Remove(Id id, string key);
    }
}

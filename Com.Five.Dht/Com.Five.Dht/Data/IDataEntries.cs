namespace Com.Five.Dht.Data
{
    using System.Threading.Tasks;

    public interface IDataEntries
    {
        Task<bool> Put(string key, object val);

        Task<object> Get(string key);

        Task<bool> Remove(string key);
    }
}

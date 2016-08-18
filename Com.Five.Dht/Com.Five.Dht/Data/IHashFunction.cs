namespace Com.Five.Dht.Data
{
    public interface IHashFunction
    {
        byte[] ComputeHash(byte[] data);
    }
}
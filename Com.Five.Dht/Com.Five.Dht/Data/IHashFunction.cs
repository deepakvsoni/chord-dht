namespace Com.Five.Dht.Data
{
    public interface IHashFunction
    {
        int Length
        {
            get;
        }

        byte[] ComputeHash(byte[] data);
    }
}
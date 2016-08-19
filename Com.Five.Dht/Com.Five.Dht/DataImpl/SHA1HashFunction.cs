namespace Com.Five.Dht.DataImpl
{
    using Data;
    using System.Security.Cryptography;

    public class SHA1HashFunction : IHashFunction
    {
        SHA1 _sha1 = new SHA1CryptoServiceProvider();

        public byte[] ComputeHash(byte[] data)
        {
            return _sha1.ComputeHash(data);
        }
    }
}

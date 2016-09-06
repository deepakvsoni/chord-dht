namespace Com.Five.Dht.DataImpl
{
    using Data;
    using System;
    using System.Security.Cryptography;

    public class SHA1HashFunction : IHashFunction, IDisposable
    {
        SHA1 _sha1 = new SHA1CryptoServiceProvider();

        public byte[] ComputeHash(byte[] data)
        {
            return _sha1.ComputeHash(data);
        }

        #region IDisposable Support

        bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _sha1.Dispose();
                }

                disposedValue = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}

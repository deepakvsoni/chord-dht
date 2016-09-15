namespace Com.Five.Dht.Data
{
    using System;
    using System.Text;

    public sealed class IdGenerator
    {
        /*
         * To avoid bit length comparison to check if we need to truncate 
         * the length of the id in Generate.
         */ 
        delegate Id GenerateId(byte[] bytes);

        int _noOfBytes, _alignmentMask;
        GenerateId _generate;

        public IdGenerator(byte maxNoOfBits, IHashFunction hashFunction)
        {
            if (0 == maxNoOfBits)
            {
                throw new ArgumentException(
                    "Max number of bits must be greater than zero.");
            }
            if (null == hashFunction)
            {
                throw new ArgumentNullException(nameof(hashFunction));
            }
            if(maxNoOfBits > hashFunction.Length)
            {
                throw new ArgumentException(
                    "Length of Hash function less than max number of bits.");
            }
            MaxNoOfBits = maxNoOfBits;
            HashFunction = hashFunction;

            _generate = GenerateIdNoTruncation;

            /*
             * We calculate the no of Bytes and mask only if the no of bits
             * required is not same as length of the hash function.
             */ 
            if (MaxNoOfBits != HashFunction.Length)
            {
                _generate = GenerateIdWithTruncation;

                _noOfBytes = (int)Math.Ceiling(maxNoOfBits / 8.0);

                /*
                 * If maxNoOfBits is not a multiple of 8 then we will have
                 * few bits which needs to be cleared, this mask will help 
                 * clear the partial bits in the byte.
                 */
                int noOfBitsSetInByte = maxNoOfBits % 8;
                _alignmentMask = 0;
                for (int i = 0; i < noOfBitsSetInByte; ++i)
                {
                    _alignmentMask |= (1 << i);
                }
            }
        }

        public int MaxNoOfBits
        {
            get;
            private set;
        }

        public IHashFunction HashFunction
        {
            get;
            private set;
        }

        public Id Generate(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);

            bytes = HashFunction.ComputeHash(bytes);

            return _generate(bytes);
        }

        Id GenerateIdNoTruncation(byte[] bytes)
        {
            return new Id(bytes);
        }

        Id GenerateIdWithTruncation(byte[] bytes)
        {
            byte[] idBytes = new byte[_noOfBytes];
            for (int i = 0; i < _noOfBytes; ++i)
            {
                idBytes[i] = bytes[i];
            }
            if (0 != _alignmentMask)
            {
                idBytes[_noOfBytes - 1] = (byte)(idBytes[_noOfBytes - 1]
                    & _alignmentMask);
            }

            return new Id(idBytes);
        }
    }
}

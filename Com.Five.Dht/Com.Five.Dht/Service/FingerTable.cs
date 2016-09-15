namespace Com.Five.Dht.Service
{
    using Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class FingerTable
    {
        IEnumerable<byte[]> _powers;

        int _maxNoOfBits;

        public FingerTable(byte maxNoOfBits)
        {
            if(0 == maxNoOfBits)
            {
                throw new ArgumentException(
                    "Invalid maxNoOfBits, should be greater than zero.");
            }
            _maxNoOfBits = maxNoOfBits;

            _powers = Id.GetPowersOfTwo(maxNoOfBits);
        }

        /*
         * TODO: Should this be in Id or FingerTable as this 
         * is the only class using it?
         */
        public byte[] AddPowerOfTwo(byte[] bytes, int power)
        {
            if(null == bytes)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            if(1 > power || power > _maxNoOfBits)
            {
                throw new ArgumentOutOfRangeException(nameof(power));
            }
            byte[] bytesAfterAddition = new byte[bytes.Length];

            byte[] powerToBeAdded = _powers.ElementAt(power - 1);

            /*
             * Adding from lowest index to high.
             */
            int carry = 0;
            for (int i = 0; i < bytes.Length; ++i)
            {
                int sum = (bytes[i] + powerToBeAdded[i] + carry);

                bytesAfterAddition[i] = (byte)sum;

                if (sum > 255)
                {
                    carry = 1;
                }
                else
                {
                    carry = 0;
                }
            }

            return bytesAfterAddition;
        }
    }
}

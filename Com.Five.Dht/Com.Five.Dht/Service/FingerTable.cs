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

        public INodeInfo[] Nodes
        {
            get;
            private set;
        }

        public FingerTable(byte maxNoOfBits)
        {
            if(0 == maxNoOfBits)
            {
                throw new ArgumentException(
                    "Invalid maxNoOfBits, should be greater than zero.");
            }
            _maxNoOfBits = maxNoOfBits;

            //Get byte[] of powers of two which can be added to the Id.
            _powers = Id.GetPowersOfTwo(maxNoOfBits);

            Nodes = new INodeInfo[maxNoOfBits];
        }

        /*
         * TODO: Should this be in Id or FingerTable as this 
         * is the only class using it?
         * 
         * Calculates byte[] after adding power of two, this also acts as
         * module 2^maxNoOfBits as the carry is not carried after last byte
         * in array.
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
             * Adding from lowest index to high thinking of this as a number
             * split at byte size.
             */
            int carry = 0;
            for (int i = 0; i < bytes.Length; ++i)
            {
                int sum = (bytes[i] + powerToBeAdded[i] + carry);

                /*
                 * This cast trim the higher bits resulting in correct byte 
                 * value with carry
                 */

                bytesAfterAddition[i] = (byte)sum;

                carry = (sum > 255) ? 1 : 0;
            }

            return bytesAfterAddition;
        }
    }
}
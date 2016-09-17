namespace Com.Five.Dht.Data
{
    using System;
    using System.Collections.Generic;
    using Common;

    [Serializable]
    public sealed class Id : IEquatable<Id>, IComparable<Id>
    {
        public static IEnumerable<byte[]> GetPowersOfTwo(byte maxNoOfBits)
        {
            if(0 == maxNoOfBits)
            {
                throw new ArgumentException(
                    "Invalid maxNoOfBits, should be greater than zero.");
            }
            IList<byte[]> powers = new List<byte[]>();

            int noOfBytes = (int)Math.Ceiling(maxNoOfBits / 8.0);
            int currentByte = 0;
            byte currentBit = 0;

            for (int i = 0; i < maxNoOfBits; ++i)
            {
                byte[] power = new byte[noOfBytes];
                for (int j = 0; j < noOfBytes; ++j)
                {
                    power[j] = 0;
                }
                power[currentByte] = (byte)(1 << currentBit);
                powers.Add(power);

                currentBit++;

                if (i != 0 && currentBit % 8 == 0)
                {
                    currentByte++;
                    currentBit = 0;
                }
            }

            return powers;
        }

        readonly byte[] _id;

        public byte MaxNoOfBits
        {
            get;
            private set;
        }

        public byte[] Bytes
        {
            get
            {
                return _id;
            }
        }

        public Id(byte[] id, byte length)
        {
            _id = id;
            MaxNoOfBits = length;
        }

        public int CompareTo(Id other)
        {
            if (MaxNoOfBits != other.MaxNoOfBits)
            {
                throw new InvalidOperationException("Comparing Ids of different size.");
            }
            
            return _id.CompareTo(other._id);
        }

        public bool Equals(Id other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            Id otherId = obj as Id;
            if(null == otherId)
            {
                return false;
            }
            return Equals(otherId);
        }

        public override int GetHashCode()
        {
            int result = 19;
            for (int i = 0; i < _id.Length; i++)
            {
                result = 13 * result + _id[i];
            }
            return result;
        }
        
        public static bool operator <(Id id1, Id id2)
        {
            return id1.CompareTo(id2) == -1;
        }

        public static bool operator <=(Id id1, Id id2)
        {
            return id1.CompareTo(id2) <= 0;
        }

        public static bool operator >(Id id1, Id id2)
        {
            return id1.CompareTo(id2) == 1;
        }

        public static bool operator >=(Id id1, Id id2)
        {
            return id1.CompareTo(id2) >= 0;
        }

        public static bool operator ==(Id id1, Id id2)
        {
            if(ReferenceEquals(id1, null) && ReferenceEquals(id2, null))
            {
                return true;
            }
            if(ReferenceEquals(id1, null) || ReferenceEquals(id2, null))
            {
                return false;
            }
            return id1.CompareTo(id2) == 0;
        }

        public static bool operator !=(Id id1, Id id2)
        {
            if (ReferenceEquals(id1, null) && ReferenceEquals(id2, null))
            {
                return false;
            }
            if (ReferenceEquals(id1, null) || ReferenceEquals(id2, null))
            {
                return true;
            }
            return id1.CompareTo(id2) != 0;
        }
    }
}
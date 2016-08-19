namespace Com.Five.Dht.Data
{
    using System;

    [Serializable]
    public sealed class Id : IEquatable<Id>, IComparable<Id>
    {
        readonly byte[] _id;

        public Id(byte[] id)
        {
            _id = id;
        }

        public int CompareTo(Id other)
        {
            if(_id.Length != other._id.Length)
            {
                throw new InvalidOperationException("Comparing Ids of different size.");
            }
            for(int i = 0; i < _id.Length; ++i)
            {
                if(_id[i] < other._id[i])
                {
                    return -1;
                }
                if (_id[i] > other._id[i])
                {
                    return 1;
                }
            }
            return 0;
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

        public static bool operator >(Id id1, Id id2)
        {
            return id1.CompareTo(id2) == 1;
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

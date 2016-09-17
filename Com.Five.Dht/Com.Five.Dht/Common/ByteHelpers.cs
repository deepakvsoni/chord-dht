namespace Com.Five.Dht.Common
{
    using System;

    public static class ByteHelpers
    {
        public static bool IsBetween(this byte[] objThis, byte[] from, byte[] to)
        {
            if (null == objThis)
            {
                throw new ArgumentNullException(nameof(objThis));
            }
            if (null == to)
            {
                throw new ArgumentNullException(nameof(to));
            }
            if (null == from)
            {
                throw new ArgumentNullException(nameof(from));
            }
            int comparison = from.CompareTo(to);
            if (0 == comparison)
            {
                return objThis.CompareTo(from) == 0;
            }
            if(-1 == comparison)
            {
                return objThis.CompareTo(from) >= 0 && objThis.CompareTo(to) <= 0;
            }
            byte[] min = new byte[objThis.Length];
            byte[] max = new byte[objThis.Length];
            for(int i = 0; i < max.Length;++i)
            {
                max[i] = 255;
            }

            return (objThis.CompareTo(from) >= 0 && objThis.CompareTo(max) <= 0) ||
                (objThis.CompareTo(to) <= 0 && objThis.CompareTo(min) >= 0);
        }

        public static int CompareTo(this byte[] objThis, byte[] objOther)
        {
            if(null == objThis)
            {
                throw new ArgumentNullException(nameof(objThis));
            }
            if(null == objOther)
            {
                throw new ArgumentNullException(nameof(objOther));
            }
            if (objThis.Length != objOther.Length)
            {
                throw new InvalidOperationException(
                    "Comparing Ids of different size.");
            }
            for (int i = 0; i < objThis.Length; ++i)
            {
                if (objThis[i] < objOther[i])
                {
                    return -1;
                }
                if (objThis[i] > objOther[i])
                {
                    return 1;
                }
            }
            return 0;
        }
    }
}

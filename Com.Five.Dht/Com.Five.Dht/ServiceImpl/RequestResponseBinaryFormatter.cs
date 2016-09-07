namespace Com.Five.Dht.ServiceImpl
{
    using Service;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public class RequestResponseBinaryFormatter : IRequestResponseFormatter
    {
        BinaryFormatter _formatter = new BinaryFormatter();

        public byte[] GetBytes(object obj)
        {
            byte[] data;

            using (MemoryStream ms = new MemoryStream())
            {
                _formatter.Serialize(ms, obj);

                data = ms.GetBuffer();
            }
            return data;
        }

        public object GetObject(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                ms.Seek(0, SeekOrigin.Begin);

                object obj = _formatter.Deserialize(ms);
                return obj;
            }
        }

        public object GetObject(int totalBytes
            , IList<ArraySegment<byte>> bytes)
        {
            using (MemoryStream ms = new MemoryStream(totalBytes))
            {
                foreach (ArraySegment<byte> segment in bytes)
                {
                    ms.Write(segment.Array, segment.Offset, segment.Count);
                }
                ms.Seek(0, SeekOrigin.Begin);

                object obj = _formatter.Deserialize(ms);
                return obj;
            }
        }
    }
}
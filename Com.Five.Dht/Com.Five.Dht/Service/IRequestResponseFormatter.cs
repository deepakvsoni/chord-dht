﻿namespace Com.Five.Dht.Service
{
    using System;
    using System.Collections.Generic;

    public interface IRequestResponseFormatter
    {
        object GetObject(int totalBytes, IList<ArraySegment<byte>> bytes);

        object GetObject(byte[] bytes);

        byte[] GetBytes(object obj);
    }
}

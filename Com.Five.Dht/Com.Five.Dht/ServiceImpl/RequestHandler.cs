namespace Com.Five.Dht.CommunicationImpl
{
    using Communication;
    using System.Threading.Tasks;
    using Communication.Requests;
    using Communication.Responses;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;
    using log4net;

    public class RequestHandler : IRequestHandler
    {
        ILog _l = LogManager.GetLogger(typeof(RequestHandler));

        BinaryFormatter _formatter = new BinaryFormatter();

         public Task<byte[]> Handle(IChannel channel, int totalBytes
             , IList<ArraySegment<byte>> req)
        {
            return Task.Factory.StartNew(() =>
            {
                Request reqObj = null;
                using (MemoryStream ms = new MemoryStream(totalBytes))
                {
                    foreach (ArraySegment<byte> segment in req)
                    {
                        ms.Write(segment.Array, segment.Offset, segment.Count);
                    }
                    ms.Seek(0, SeekOrigin.Begin);

                    try
                    {
                        reqObj = (Request)_formatter.Deserialize(ms);
                        _l.InfoFormat("Received qequest: {0}", req);
                    }
                    catch (InvalidCastException e)
                    {
                        _l.Error("Invalid type of request received.", e);
                    }
                }
                Response res = null;
                if (null == req)
                {
                    res = InvalidRequest.I;
                }
                else
                {
                    if(req is Shutdown)
                    {
                        channel.RequestClose();
                    }
                }

                byte[] responseBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    _formatter.Serialize(ms, res);

                    ms.Seek(0, SeekOrigin.Begin);

                    responseBytes = ms.GetBuffer();
                }
                return responseBytes;
            });
        }
    }
}
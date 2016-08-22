namespace Com.Five.Dht.CommunicationImpl
{
    using Communication;
    using System.Threading.Tasks;
    using Communication.Requests;
    using Communication.Responses;

    public class RequestHandler : IRequestHandler
    {
        public Task<Response> Handle(Request req)
        {
            return Task.Factory.StartNew<Response>(() =>
            {
                if (req is Shutdown)
                {
                    return new ShutdownResponse();
                }
                return null;
            });
        }
    }
}
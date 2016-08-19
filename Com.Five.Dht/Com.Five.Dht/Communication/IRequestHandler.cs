namespace Com.Five.Dht.Communication
{
    using Requests;
    using Responses;
    using System.Threading.Tasks;

    public interface IRequestHandler
    {
        Task<Response> Handle(Request req);
    }
}

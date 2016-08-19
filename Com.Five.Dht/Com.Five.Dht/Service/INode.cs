namespace Com.Five.Dht.Service
{
    using Communication;
    using Data;
    using System.Threading.Tasks;

    public interface INode
    {
        Id Id { get; }

        IChannel Endpoint { get; }

        INode Predecessor { get; set; }
    }
}

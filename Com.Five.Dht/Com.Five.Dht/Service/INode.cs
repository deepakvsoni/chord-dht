namespace Com.Five.Dht.Service
{
    using Communication;
    using Data;

    public interface INode
    {
        Id Id { get; }

        IEndpoint Endpoint { get; }
    }
}

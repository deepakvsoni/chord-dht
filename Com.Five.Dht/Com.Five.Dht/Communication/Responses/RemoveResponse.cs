namespace Com.Five.Dht.Communication.Responses
{
    public class RemoveResponse : Response
    {
        public static readonly RemoveResponse Success
            = new RemoveResponse { Status = Status.Ok };

        public static readonly RemoveResponse Failed
            = new RemoveResponse { Status = Status.Failed };
    }
}

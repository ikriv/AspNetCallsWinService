namespace IKriv.Sample.CallWinService.Interfaces
{
    public interface IRequestHandler
    {
        AcceptedRequest CreateRequest(Request req);
        AcceptedRequest GetRequest(long id);
        RequestList GetAllRequests(long sinceRevision); // use sinceRevision = 0 to get all available requests
    }
}

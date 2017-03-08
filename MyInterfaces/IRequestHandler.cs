using System.ServiceModel;

namespace IKriv.Sample.CallWinService.Interfaces
{
    [ServiceContract]
    public interface IRequestHandler
    {
        [OperationContract] AcceptedRequest CreateRequest(Request req);
        [OperationContract] AcceptedRequest GetRequest(long id);
        [OperationContract] RequestList GetAllRequests(long sinceRevision); // use sinceRevision = 0 to get all available requests
    }
}

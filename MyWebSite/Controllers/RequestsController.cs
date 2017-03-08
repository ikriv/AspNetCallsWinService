using System;
using System.Web.Http;
using IKriv.Sample.CallWinService.Communication;
using IKriv.Sample.CallWinService.Interfaces;

namespace IKriv.Sample.CallWinService.WebSite.Controllers
{
    public class RequestsController : ApiController
    {
        private readonly Lazy<IRequestHandler> _handler = new Lazy<IRequestHandler>(GetHandler);

        [HttpGet]
        public AcceptedRequest GetRequest(int id)
        {
            return _handler.Value.GetRequest(id);
        }

        [HttpPost]
        public IHttpActionResult CreateRequest(Request request)
        {
            var result = _handler.Value.CreateRequest(request);
            return Created(result.Id.ToString(), result);
        }

        [HttpGet]
        public RequestList GetAllRequests(long? sinceRevision = null)
        {
            return _handler.Value.GetAllRequests(sinceRevision ?? 0);
        }

        private static IRequestHandler GetHandler()
        {
            return new RemotingWrapper().CreateClient<IRequestHandler>(ServiceConstants.Url);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using IKriv.Sample.CallWinService.Interfaces;

namespace IKriv.Sample.CallWinService.WindowsService
{
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single,
        IncludeExceptionDetailInFaults=true)]
    public class RequestHandler : IRequestHandler
    {
        private readonly Dictionary<long, AcceptedRequest> _requests = new Dictionary<long, AcceptedRequest>();
        private long _lastRequestId; // = 0
        private long _lastRevision; // = 0

        public AcceptedRequest CreateRequest(Request rawRequest)
        {
            if (rawRequest == null) throw new ArgumentException(nameof(rawRequest));
            var delay = rawRequest.RequestedProcessingTimeMs;
            if (delay < 0)
            {
                throw new ArgumentException("RequestProcessingTimeMs must not be negative");
            }

            var now = DateTime.UtcNow;
            AcceptedRequest request;

            lock (_requests)
            {
                var id = ++_lastRequestId;
                var revision = ++_lastRevision;

                request = new AcceptedRequest
                {
                    Id = id,
                    Revision = revision,
                    Input = rawRequest.Input,
                    RequestedProcessingTimeMs = delay,
                    CreatedOn = now,
                    ScheduledCompletionTime = now.AddMilliseconds(delay)
                };

                _requests[id] = request;
            }

            Task.Delay(delay).ContinueWith((task, state) => CompleteRequest(request), null);
            return request;
        }

        public AcceptedRequest GetRequest(long id)
        {
            lock (_requests)
            {
                AcceptedRequest result;
                if (!_requests.TryGetValue(id, out result)) return null;
                return result;
            }
        }

        public RequestList GetAllRequests(long sinceRevision)
        {
            lock (_requests)
            {
                return new RequestList
                {
                    Revision = _lastRevision,
                    Requests = _requests.Values
                        .Where(r => r.Revision > sinceRevision)
                        .OrderByDescending(r => r.ActualCompletionTime ?? r.ScheduledCompletionTime)
                        .ToArray()
                };
            }
        }

        private void CompleteRequest(AcceptedRequest request)
        {
            var now = DateTime.UtcNow;
            var output = (request.Input ?? "") + " DONE";

            lock (_requests)
            {
                request.Revision = ++_lastRevision;
                request.Output = output;
                request.ActualCompletionTime = now;
                request.ActualProcessingTimeMs = (int) ((now - request.CreatedOn).TotalMilliseconds + 0.5);
                request.IsCompleted = true;
            }
        }
    }
}

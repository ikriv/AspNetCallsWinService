using System;

namespace IKriv.Sample.CallWinService.Interfaces
{
    [Serializable]
    public class Request
    {
        public string Input { get; set; }
        public int RequestedProcessingTimeMs { get; set; }
    }

    [Serializable]
    public class AcceptedRequest : Request
    {
        public long Id { get; set; }
        public long Revision { get; set; } // revision acts as 'last modified date'
        public bool IsCompleted { get; set; }
        public string Output { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ScheduledCompletionTime { get; set; }
        public DateTime? ActualCompletionTime { get; set; }
        public int? ActualProcessingTimeMs { get; set; }
    }

    [Serializable]
    public class RequestList
    {
        public long Revision { get; set; }
        public AcceptedRequest[] Requests { get; set; }
    }
}
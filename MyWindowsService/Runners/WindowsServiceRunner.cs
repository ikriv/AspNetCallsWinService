using System;
using System.ServiceProcess;

namespace IKriv.Sample.CallWinService.WindowsService.Runners
{
    /// <summary>
    /// Runs given service as a windows service
    /// </summary>
    internal class WindowsServiceRunner : ServiceBase, IRunner
    {
        readonly IService _service;

        public WindowsServiceRunner(IService service, string serviceName)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (serviceName == null) throw new ArgumentNullException(nameof(serviceName));
            _service = service;
            ServiceName = serviceName;
        }

        public void Run()
        {
            ServiceBase.Run(this);
        }

        protected override void OnStart(string[] args)
        {
            _service.Start();
        }

        protected override void OnStop()
        {
            _service.Stop();
        }
    }
}

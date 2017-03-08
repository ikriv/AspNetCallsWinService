using System;
using IKriv.Sample.CallWinService.Communication;
using IKriv.Sample.CallWinService.Interfaces;
using IKriv.Sample.CallWinService.WindowsService.Log;
using IKriv.Sample.CallWinService.WindowsService.Runners;

namespace IKriv.Sample.CallWinService.WindowsService
{
    internal class MyService : IService
    {
        private readonly ILog _log;
        private IDisposable _wcfHost;

        public MyService(ILog log)
        {
            _log = log;
        }

        public void Start()
        {
            _log.Write(LogLevel.Debug, "Running as " + Environment.UserName);

            if (_wcfHost != null)
            {
                _log.Write(LogLevel.Warning, "WCF host has already been created. Did you attempt to start the service twice without stopping it?");
                return;
            }
            IRequestHandler handler = new RequestHandler();
            _wcfHost = new WcfWrapper().CreateServer(handler, ServiceConstants.Url);
        }

        public void Stop()
        {
            _wcfHost.Dispose();
            _wcfHost = null;
            _log.Write(LogLevel.Info, "Exiting service");
        }
    }
}

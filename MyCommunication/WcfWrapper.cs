using System;
using System.ServiceModel;

namespace IKriv.Sample.CallWinService.Communication
{
    public class WcfWrapper
    {
        public T CreateClient<T>(string url)
        {
            var address = new EndpointAddress(url);
            var channelFactory = new ChannelFactory<T>(new NetNamedPipeBinding(NetNamedPipeSecurityMode.None));
            var client = channelFactory.CreateChannel(address);
            return client;
        }

        public IDisposable CreateServer<TInterface>(TInterface obj, string url)
        {
            return new WcfHost().Open(obj, url);
        }

        private class WcfHost : IDisposable
        {
            ServiceHost _serviceHost;

            public WcfHost Open<T>(T instance, string url)
            {
                _serviceHost = new ServiceHost(instance, new Uri(url));
                _serviceHost.AddServiceEndpoint(typeof(T), new NetNamedPipeBinding(NetNamedPipeSecurityMode.None), "");
                _serviceHost.Open();
                return this;
            }

            public void Dispose()
            {
                _serviceHost?.Close();
            }
        }
    }
}

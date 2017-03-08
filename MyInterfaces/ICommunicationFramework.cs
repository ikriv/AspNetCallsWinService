using System;

namespace IKriv.Sample.CallWinService.Interfaces
{
    public interface ICommunicationFramework
    {
        T CreateClient<T>(string url);
        IDisposable CreateServer<TInterface>(string url, TInterface obj);
    }
}

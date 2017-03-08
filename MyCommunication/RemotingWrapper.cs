using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace IKriv.Sample.CallWinService.Communication
{
    public class RemotingWrapper
    {
        public T CreateClient<T>(string url)
        {
            IpcServices.RegisterChannel();
            return (T)Activator.GetObject(typeof(T), url);
        }

        public IDisposable CreateServer<TInterface>(TInterface obj, string url)
        {
            var objType = obj.GetType();
            var mbrObj = obj as MarshalByRefObject;
            if (mbrObj == null)
            {
                throw new InvalidOperationException($"Cannot expose object of type ${objType.Name} as remoting server, because it does not derive from MarshalByRefObject");
            }

            const string ipcUrlPattern = "^ipc://([a-zA-Z0-9_\\.~-]+)/([a-zA-Z0-9_\\.~-]+)$";
            var regex = new Regex(ipcUrlPattern);
            var match = regex.Match(url);
            if (!match.Success)
            {
                throw new InvalidOperationException(
                    $"Invalid remoting URL '{url}'. Remoting URL must have form ipc://{{channelName}}/{{object}}");
            }

            var channelName = match.Groups[1].Value;
            var objectName = match.Groups[2].Value;

            IpcServices.RegisterChannel(channelName);
            var objRef = RemotingServices.Marshal(mbrObj, objectName, typeof(TInterface));
            return new DisposableObjRef(objRef);
        }

        private class DisposableObjRef : IDisposable
        {
            private readonly ObjRef _objRef;

            public DisposableObjRef(ObjRef objRef)
            {
                _objRef = objRef;
            }

            public void Dispose()
            {
                RemotingServices.Unmarshal(_objRef);
            }
        }

        private static class IpcServices
        {
            private static string _channelName;
            private static readonly object Lock = new object();

            public static void RegisterChannel()
            {
                lock (Lock)
                {
                    if (_channelName != null) return;
                    RegisterChannel(Guid.NewGuid().ToString());
                }
            }

            public static void RegisterChannel(string channelName)
            {
                lock (Lock)
                {
                    if (_channelName != null) // already registered
                    {
                        if (_channelName == channelName) return; // already registsred under the same name, mothing to do

                        // previously registered under a different name, throw
                        throw new InvalidOperationException(
                            $"Cannot register remoting channel '{channelName}'. Channel '{_channelName}' has already been registered");
                    }

                    var serverProvider = new BinaryServerFormatterSinkProvider { TypeFilterLevel = TypeFilterLevel.Full };
                    var clientProvider = new BinaryClientFormatterSinkProvider();

                    // "authorizedGroup" property contains the name of the group that has the right to access the channel
                    // "Authenticated Users" group includes real users and local accounts like Network Service
                    // However, it has different names on different localized version of Windows, e.g. "Authentifizierte Benutzer"
                    // on German windows, so we must retrieve the name based on the well known SID type

                    var properties = new Hashtable
                    {
                        ["portName"] = channelName,
                        ["authorizedGroup"] = GetAccountName(WellKnownSidType.AuthenticatedUserSid)
                    };

                    var channel = new IpcChannel(properties, clientProvider, serverProvider);
                    ChannelServices.RegisterChannel(channel, false);
                    _channelName = channelName;
                }
            }

            private static string GetAccountName(WellKnownSidType wellKnownSid)
            {
                var sid = new SecurityIdentifier(wellKnownSid, null);
                var account = sid.Translate(typeof(NTAccount)) as NTAccount;
                return account?.Value;
            }
        }
    }
}

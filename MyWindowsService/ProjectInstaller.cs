using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace IKriv.Sample.CallWinService.WindowsService
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            var processInstaller = new ServiceProcessInstaller { Account = ServiceAccount.NetworkService };

            var serviceInstaller = new ServiceInstaller 
            { 
                ServiceName = StringConstants.ServiceName, 
                DisplayName = StringConstants.ServiceDisplayName,
                StartType = ServiceStartMode.Manual,
                Description = StringConstants.ServiceDescription
            };

            Installers.AddRange(new Installer[] { processInstaller, serviceInstaller });
        }
    }
}

using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace BackUpService
{
  [RunInstaller(true)]
  public class BackupServiceInstaller : Installer
  {
    private readonly ServiceInstaller myInstaller;
    private readonly ServiceProcessInstaller myProcessInstaller;


    public BackupServiceInstaller()
    {
      myInstaller = new ServiceInstaller();
      myProcessInstaller = new ServiceProcessInstaller();

      myProcessInstaller.Account = ServiceAccount.LocalSystem;
      myInstaller.StartType = ServiceStartMode.Automatic;
      myInstaller.ServiceName = BackUpService.SERVICE_NAME;

      Installers.Add(myInstaller);
      Installers.Add(myProcessInstaller);
    }
  }
}
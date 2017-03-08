using System;
using System.Configuration.Install;
using System.IO;
using System.Reflection;
using IKriv.Sample.CallWinService.WindowsService.Log;
using IKriv.Sample.CallWinService.WindowsService.Runners;

namespace IKriv.Sample.CallWinService.WindowsService
{
    static class Program
    {
        const string ProgramName = "MyWindowsService.exe";

        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 1)
                {
                    if (Environment.UserInteractive)
                    {
                        Usage();
                    }
                    else
                    {
                        RunAsService();
                    }
                }
                else
                {
                    ProcessCommand(args[0]);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }

        private static void ProcessCommand(string command)
        {
            switch (command)
            {
                case "help": Help(Console.Out); break;
                case "console": RunAsConsole(); break;
                case "install": Install(); break;
                case "uninstall": Uninstall(); break;
                default: UnknownCommand(command); break;
            }
        }

        private static void RunAsService()
        {
            var log = new SystemLog(StringConstants.ServiceName);
            var service = new MyService(log);
            var runner = new WindowsServiceRunner(service, StringConstants.ServiceName);
            runner.Run();
        }

        private static void Usage()
        {
            Console.Error.WriteLine(
                $"{ProgramName} is a Windows service.\r\n"+
                $"Use \"{ProgramName} install\" or \"{ProgramName} help\".");
        }

        private static void Help(TextWriter output)
        {
            output.WriteLine(
@"Usage:
    " + ProgramName + @" [command]
    If invoked without arguments, runs as Windows service.

Supported commands:
    help        Print this text
    install     Install Windows service " + StringConstants.ServiceName + @" (requires admin rights)
    uninstall   Uninstall windows service (requires admin rights)
    console     Run as console program instead of service");
        }

        private static void UnknownCommand(string command)
        {
            Console.Error.WriteLine($"Unknown command: '{command}'\r\n");
            Help(Console.Error);
        }

        private static void RunAsConsole()
        {
            var log = new ConsoleLog();
            log.Write(LogLevel.Info, $"Running {ProgramName} as console program. Press Ctrl+C to exit.");
            var service = new MyService(log);
            var runner = new ConsoleRunner(service);
            runner.Run();
        }

        private static void Install()
        {
            ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
        }

        private static void Uninstall()
        {
            ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
        }
    }
}

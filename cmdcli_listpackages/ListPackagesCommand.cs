using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using cmdcli_core;
using CommandRunner;

namespace cmdcli_listpackages
{
    public class CLIModuleInfo : AbstractCmdModuleInfo
    {
        public override string[] Dependencies => new[]
        {
            "cmdcli_core.dll"
        };
        public override string ModuleName => "list";
        public override void RunArgs(string[] args)
        {
            Runner.AddAssembly(Assembly.GetExecutingAssembly());
            Runner.RunCommands(args);
        }
    }

    public class ListPackagesCommand : AbstractCommand
    {
        private static string _HelpText => "list\n Lists all available packages that can be added.";

        

        private static void ListPackages(StartupInfo info, string[] args)
        {
            WebClient wc = new WebClient();
            string s = wc.DownloadString(ModuleCommandLineInterface.ModuleURL + "ModuleList.list");
            wc.Dispose();
            string[] modules = s.Replace("\r", "").Split(new[] {'\n'});
            Console.WriteLine("Available Modules:");
            foreach (string module in modules)
            {
                Console.WriteLine("\t"+module);
            }
        }

        public ListPackagesCommand() : base(ListPackages, new[] { "--list", "-l" }, _HelpText, true)
        {

        }
    }
}

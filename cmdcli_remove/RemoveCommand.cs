using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using cmdcli_core;
using CommandRunner;

namespace cmdcli_add
{


    public class CLIModuleInfo : AbstractCmdModuleInfo
    {
        public override string[] Dependencies => new[]
        {
            "cmdcli_core.dll"
        };
        public override string ModuleName => "remove";
        public override void RunArgs(string[] args)
        {
            Runner.AddAssembly(Assembly.GetExecutingAssembly());
            Runner.RunCommands(args);
        }
    }

    public class RemoveCommand : AbstractCommand
    {
        private static string _HelpText => "remove <ModuleName> <ModuleName> ...\n Removes a Module from the cmdcli";

        private static void Remove(string[] moduleNames)
        {
            

            List<ModuleInfo> modules = ModuleCommandLineInterface.LoadModuleList();

            for (int i = 0; i < moduleNames.Length; i++)
            {
                if (modules.ContainsKey(moduleNames[i]))
                {
                    Console.WriteLine($"Removing Module {moduleNames[i]} from Installed Modules..");
                    int idx = modules.IndexOf(moduleNames[i]);
                    ModuleInfo info = modules[idx];
                    modules.RemoveAt(idx);
                    if (File.Exists(info.ModulePath))
                        File.Delete(info.ModulePath);
                }
            }
            ModuleCommandLineInterface.SaveModuleList(modules);
        }

        private static void Remove(StartupInfo info, string[] args)
        {
            Remove(args);
        }

        public RemoveCommand() : base(Remove, new[] { "--remove", "-r" }, _HelpText, true)
        {

        }
    }
}

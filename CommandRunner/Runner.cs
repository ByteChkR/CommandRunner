using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandRunner;

namespace CommandRunner
{
    /// <summary>
    /// Contains the Logic for Running Commands
    /// </summary>
    public static class Runner
    {
        private static List<AbstractCommand> _commands = new List<AbstractCommand>();
        public static int CommandCount => _commands.Count;

        public static void AddAssembly(string path)
        {
            if (AssemblyHelper.TryLoadAssembly(path, out Assembly asm))
            {
                AddAssembly(asm);
            }
        }

        public static void AddAssembly(Assembly asm)
        {
            List<AbstractCommand> cmds = AssemblyHelper.LoadCommandsFromAssembly(asm);
            for (int i = 0; i < cmds.Count; i++)
            {
                AddCommand(cmds[i]);
            }
        }

        public static void AddCommand(AbstractCommand cmd)
        {
            if (IsInterfering(cmd)) Console.WriteLine("Command:" + cmd.GetType().FullName + " is interfering with other Commands.");
            _commands.Add(cmd);
        }

        private static bool IsInterfering(AbstractCommand cmd)
        {
            for (int i = 0; i < _commands.Count; i++)
            {
                if (cmd.IsInterfering(_commands[i])) return true;
            }

            return false;
        }


        public static AbstractCommand GetCommandAt(int index)
        {
            return _commands[index];
        }

        public static void RunCommands(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                for (int j = 0; j < _commands.Count; j++)
                {
                    if (_commands[j].CommandKeys.Contains(args[i]))
                    {
                        args[i] = _commands[j].CommandKeys[0]; //Make sure its the first command key.
                    }
                }
            }

            StartupInfo info = new StartupInfo(args);

            for (int i = 0; i < _commands.Count; i++)
            {
                if (info.GetCommandEntries(_commands[i].CommandKeys[0]) != 0)
                {
                    for (int j = 0; j < info.GetCommandEntries(_commands[i].CommandKeys[0]); j++)
                    {
                        _commands[i].CommandAction?.Invoke(info, info.GetValues(_commands[i].CommandKeys[0], j).ToArray());
                    }
                }
            }

            if (info.GetCommandEntries("noflag") != 0)
            {
                List<AbstractCommand> cmds = _commands.Where(x => x.DefaultCommand).ToList();
                if (cmds.Count == 0)
                {
                    Console.WriteLine("No Default Command Found");
                }
                else if (cmds.Count == 1)
                {
                    for (int j = 0; j < info.GetCommandEntries(cmds[0].CommandKeys[0]); j++)
                    {
                        cmds[0].CommandAction?.Invoke(info, info.GetValues(cmds[0].CommandKeys[0], j).ToArray());
                    }
                }
                else
                {
                    Console.WriteLine("Found more than one Default Command.");
                    Console.WriteLine("Using Command: " + cmds[0].CommandKeys[0]);
                    for (int j = 0; j < info.GetCommandEntries("noflag"); j++)
                    {
                        cmds[0].CommandAction?.Invoke(info, info.GetValues("noflag", j).ToArray());
                    }
                }
            }
        }
    }
}
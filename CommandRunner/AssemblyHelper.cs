using System;
using System.Collections.Generic;
using System.Reflection;

namespace CommandRunner
{
    public static class AssemblyHelper
    {
        public static List<AbstractCommand> LoadCommandsFromAssembly(Assembly asm)
        {

            List<AbstractCommand> ret = new List<AbstractCommand>();

            Type[] types = asm.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (typeof(AbstractCommand).IsAssignableFrom(types[i]) && types[i] != typeof(AbstractCommand))
                {
                    ret.Add((AbstractCommand)Activator.CreateInstance(types[i]));
                }
            }

            return ret;
        }

        public static bool TryLoadAssembly(string path, out Assembly asm)
        {
            try
            {
                asm = Assembly.LoadFile(path);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                asm = null;
                return false;
            }
        }
    }
}
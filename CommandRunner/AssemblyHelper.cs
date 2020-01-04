using System;
using System.Collections.Generic;
using System.Reflection;

namespace CommandRunner
{
    /// <summary>
    /// Small Helper Class that is Handling The Assembly Loading/Getting the Commands from assemblies.
    /// </summary>
    public static class AssemblyHelper
    {
        /// <summary>
        /// Returns all Commands inside the Assembly
        /// </summary>
        /// <param name="asm">The Assembly to search</param>
        /// <returns></returns>
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

        /// <summary>
        /// Loads an assembly from path
        /// </summary>
        /// <param name="path">Full Path to the Assembly.</param>
        /// <param name="asm">The Assembly that was loaded.</param>
        /// <returns>Bool that indicates the Success</returns>
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkTest
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandRunner.Runner.AddAssembly(Assembly.GetExecutingAssembly());
            string[] assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.AllDirectories);

            for (int i = 0; i < assemblies.Length; i++)
            {
                CommandRunner.Runner.AddAssembly(assemblies[i]);
            }
            CommandRunner.Runner.RunCommands(args);
        }
    }
}

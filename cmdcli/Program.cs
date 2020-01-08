using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace cmdcli
{
    internal static class Program
    {
        private static Assembly cliAssembly;

        private static Type cliType => cliAssembly.GetType("cmdcli_core.ModuleCommandLineInterface");
        private static MethodInfo cliMethod => cliType.GetMethod("RunArgs");
        private static void StartupCheck()
        {
            string loc = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath);
            string dir = Path.Combine(loc, "Modules");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            bool scan = false;
            string file = Path.Combine(dir, "cmdcli_add.dll");
            if (!File.Exists(file))
            {
                scan = true;
                CheckModule("add");
            }

            string coreAssembly = Path.Combine(dir, "cmdcli_core.dll");
            if (!File.Exists(coreAssembly))
            {
                scan = true;
                CheckModule("cli");
            }


            cliAssembly = Assembly.LoadFile(coreAssembly); //Load Add Command(required to download other commands)

            if (scan) Run(new[] { "--scan" });
        }

        private static void CheckModule(string moduleName)
        {
            string loc = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath);
            string dir = Path.Combine(loc, "Modules");
            WebClient wc = new WebClient();

            string tmpFile = Path.Combine(loc, "tmpfile.zip");

            wc.DownloadFile($"http://213.109.162.193/packages/cmdcli/modules/{moduleName}.zip", tmpFile);
            wc.Dispose();

            ZipArchive za = ZipFile.OpenRead(tmpFile);

            for (int i = 0; i < za.Entries.Count; i++)
            {
                string path = Path.Combine(dir, za.Entries[i].FullName);
                if (File.Exists(path)) File.Delete(path);
            }

            za.Dispose();

            ZipFile.ExtractToDirectory(tmpFile, dir);

            File.Delete(tmpFile);
        }


        private static void Run(string[] args)
        {
            //ModuleCommandLineInterface.RunArgs(args);
            try
            {
                cliMethod.Invoke(null, new[] { args });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error({e.GetType()}): {e.Message}");
            }
        }

        private static void Main(string[] args)
        {

            StartupCheck();

            Run(args);


        }


    }
}

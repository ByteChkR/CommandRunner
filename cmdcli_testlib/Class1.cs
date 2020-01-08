using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using CommandRunner;

namespace cmdcli_testlib
{


    public class ModuleInfo : AbstractCmdModuleInfo
    {
        public override string ModuleName => "cli";
        public override void RunArgs(string[] args)
        {
            Runner.AddAssembly(Assembly.GetExecutingAssembly());
            Runner.RunCommands(args);
        }
    }

    public class AddCommand : AbstractCommand
    {
        private static void AddModule(string name)
        {
            Console.WriteLine("Adding Module: " + name);
            string tmpDir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetTempFileName()));
            string tmpFile = Path.Combine(tmpDir, Path.GetTempFileName());
            string url = "http://213.109.162.193/packages/cmdcli/modules/" + name + ".zip";
            //Test
            if (IsUrlCorrect(url))
            {
                Directory.CreateDirectory(tmpDir);
                WebClient wc = new WebClient();
                Console.WriteLine("Downloading Module Package to: " + tmpFile);
                Console.WriteLine("Downloading Module Package from: " + url);

                wc.DownloadFile(url, tmpFile);
                ZipArchive za = ZipFile.OpenRead(tmpFile);
                string moduleDir = Path.GetDirectoryName(new Uri(Assembly.GetEntryAssembly().CodeBase).AbsolutePath) + "/Modules";
                Console.WriteLine("Extracting Package to " + moduleDir);
                for (int i = 0; i < za.Entries.Count; i++)
                {
                    string p = Path.Combine(moduleDir, za.Entries[i].FullName);
                    if (File.Exists(p))
                    {
                        Console.WriteLine("Deleting file : " + p);
                        File.Delete(p);
                    }
                }
                za.ExtractToDirectory(moduleDir);
                Directory.Delete(tmpDir, true);
            }
            else
            {
                Console.WriteLine("Package: \"" + name + "\" Could not be found");
            }
        }

        private static bool IsUrlCorrect(string addr)
        {
            HttpWebResponse response = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(addr);
            request.Method = "HEAD";

            bool ret = false;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                ret = true;
            }
            catch (Exception)
            {
                ret = false;
            }
            finally
            {
                // Don't forget to close your response.
                if (response != null)
                {
                    response.Close();
                    ret = true;
                }
            }

            return ret;
        }

        private static void Add(StartupInfo info, string[] args)
        {

            for (int i = 0; i < args.Length; i++)
            {
                AddModule(args[i]);
            }

        }
        public AddCommand() : base(Add, new[] { "--add", "-a" }, "--add <ModuleName> <ModuleName> ...\n Adds a Module to the cmdcli", true)
        {

        }
    }

    public class HelpCommand : AbstractCommand
    {
        private static void Help(StartupInfo info, string[] args)
        {
            Console.WriteLine("Commands:");
            for (int i = 0; i < Runner.CommandCount; i++)
            {
                Console.WriteLine(Runner.GetCommandAt(i));
            }
        }
        public HelpCommand() : base(Help, new[] { "--help", "-h", "-?" }, "Displays all Commands", true)
        {

        }
    }
}

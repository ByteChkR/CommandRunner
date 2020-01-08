using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CommandRunner;

namespace cmdcli
{
    public struct ModuleInfo
    {
        public string ModuleCommand;
        public string ModulePath;
        public string[] Dependencies;
    }
    internal class Program
    {


        private static string RootDir =
            Path.GetDirectoryName(new Uri(Assembly.GetEntryAssembly().CodeBase).AbsolutePath);
        private static string ModuleList = Path.Combine(RootDir, "ModuleList.xml");
        private static string ModulePath = Path.Combine(RootDir, "Modules/");
        private static List<ModuleInfo> moduleDatabase;

        private static bool ContainsKey(List<ModuleInfo> info, string key)
        {
            for (int i = 0; i < info.Count; i++)
            {
                if (info[i].ModuleCommand == key) return true;
            }

            return false;
        }

        private static int IndexOf(List<ModuleInfo> info, string key)
        {
            for (int i = 0; i < info.Count; i++)
            {
                if (info[i].ModuleCommand == key) return i;
            }

            return -1;
        }
        private static void Main(string[] args)
        {
            List<string> arg = args.ToList();

            if (arg.IndexOf("--scan") != -1)
            {
                File.Delete(ModuleList);
            }


            CreateModuleList();



            if (ContainsKey(moduleDatabase,args[0]))
            {
                List<string> arguments = new List<string>();
                for (int i = 1; i < args.Length; i++)
                {
                    arguments.Add(args[i]);
                }

                LoadModule(moduleDatabase[IndexOf(moduleDatabase, args[0])].ModulePath).RunArgs(arguments.ToArray());
            }
            else if (arg.IndexOf("--help") != -1)
            {
                Console.WriteLine("Available Modules:");
                foreach (ModuleInfo keyValuePair in moduleDatabase)
                {
                    Console.WriteLine("\t\t" + keyValuePair.ModuleCommand);
                }
            }
        }

        private static AbstractCmdModuleInfo LoadModule(string path)
        {
            string fp = Path.GetFullPath(path);
            try
            {
                Assembly asm = Assembly.LoadFile(fp);
                AbstractCmdModuleInfo info = GetInfo(asm);
                return info;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private static void UpdateModuleList()
        {
            List<ModuleInfo> remList = new List<ModuleInfo>();
            foreach (ModuleInfo keyValuePair in moduleDatabase)
            {
                if (!File.Exists(keyValuePair.ModulePath)) remList.Add(keyValuePair);
            }

            for (int i = 0; i < remList.Count; i++)
            {
                moduleDatabase.Remove(remList[i]);
            }
            SaveModuleList(moduleDatabase);
        }

        private static void SaveModuleList(List<ModuleInfo> moduleList)
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<ModuleInfo>));
            FileStream fs = new FileStream(ModuleList, FileMode.Create);
            xs.Serialize(fs, moduleList);
        }

        private static List<ModuleInfo> LoadModuleList()
        {
            FileStream fs = new FileStream(ModuleList, FileMode.Open);
            XmlSerializer xs = new XmlSerializer(typeof(List<ModuleInfo>));
            List<ModuleInfo> list = (List<ModuleInfo>)xs.Deserialize(fs);
            fs.Close();
            return list;
        }

        private static void CreateModuleList()
        {
            bool createNew = !File.Exists(ModuleList);
            if (!createNew)
            {
                moduleDatabase = LoadModuleList();
                UpdateModuleList();
                return;
            }
            moduleDatabase = new List<ModuleInfo>();
            string[] modules = Directory.GetFiles(ModulePath, "*.dll", SearchOption.AllDirectories);
            for (int i = 0; i < modules.Length; i++)
            {
                string fp = Path.GetFullPath(modules[i]);
                try
                {
                    Assembly asm = Assembly.LoadFile(fp);
                    AbstractCmdModuleInfo info = GetInfo(asm);
                    if (info == null) continue;
                    moduleDatabase.Add(new ModuleInfo{Dependencies = new string[0], ModulePath = fp, ModuleCommand = info.ModuleName});
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            SaveModuleList(moduleDatabase);
        }

        private static AbstractCmdModuleInfo GetInfo(Assembly asm)
        {

            Type[] types = asm.GetTypes();
            Type t = typeof(AbstractCmdModuleInfo);
            for (int i = 0; i < types.Length; i++)
            {
                if (t.IsAssignableFrom(types[i])) return (AbstractCmdModuleInfo)Activator.CreateInstance(types[i]);
            }

            return null;

        }
    }
}

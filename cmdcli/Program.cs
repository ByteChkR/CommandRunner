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
    }
    internal class Program
    {
        
        

        private static string ModuleList = "./ModuleList.xml";
        private static string ModulePath = "./Modules/";
        private static Dictionary<string, string> moduleDatabase;
        private static void Main(string[] args)
        {

            CreateModuleList();

            if (moduleDatabase.ContainsKey(args[0]))
            {
                List<string> arguments = new List<string>();
                for (int i = 1; i < args.Length; i++)
                {
                    arguments.Add(args[i]);
                }

                LoadModule(moduleDatabase[args[0]]).RunArgs(arguments.ToArray());
            }

        }

        private static AbstractCmdModuleInfo LoadModule(string path)
        {
            string fp = Path.GetFullPath(path);
            try
            {
                Assembly asm = Assembly.LoadFile(fp);
                AbstractCmdModuleInfo info = GetInfo(asm);
                if (info != null)
                    Runner.AddAssembly(asm);
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
            List<string> remList = new List<string>();
            foreach (KeyValuePair<string, string> keyValuePair in moduleDatabase)
            {
                if (!File.Exists(keyValuePair.Value)) remList.Add(keyValuePair.Key);
            }

            for (int i = 0; i < remList.Count; i++)
            {
                moduleDatabase.Remove(remList[i]);
            }
            SaveModuleList(moduleDatabase);
        }

        private static void SaveModuleList(Dictionary<string, string> moduleList)
        {
            List<ModuleInfo> list = new List<ModuleInfo>();
            foreach (KeyValuePair<string, string> keyValuePair in moduleList)
            {
                list.Add(new ModuleInfo { ModuleCommand = keyValuePair.Key, ModulePath = keyValuePair.Value});
            }
            XmlSerializer xs = new XmlSerializer(typeof(List<ModuleInfo>));
            FileStream fs = new FileStream(ModuleList, FileMode.Create);
            xs.Serialize(fs, list);
        }

        private static Dictionary<string, string> LoadModuleList()
        {
            FileStream fs = new FileStream(ModuleList, FileMode.Open);
            XmlSerializer xs = new XmlSerializer(typeof(List<ModuleInfo>));
            List<ModuleInfo> list = (List<ModuleInfo>)xs.Deserialize(fs);
            fs.Close();
            Dictionary<string, string> ret = new Dictionary<string, string>();
            for (int i = 0; i < list.Count; i++)
            {
                ret.Add(list[i].ModuleCommand, list[i].ModulePath);
            }

            return ret;
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
            moduleDatabase = new Dictionary<string, string>();
            string[] modules = Directory.GetFiles(ModulePath, "*.dll", SearchOption.AllDirectories);
            for (int i = 0; i < modules.Length; i++)
            {
                string fp = Path.GetFullPath(modules[i]);
                try
                {
                    Assembly asm = Assembly.LoadFile(fp);
                    AbstractCmdModuleInfo info = GetInfo(asm);
                    if (info == null) continue;
                    moduleDatabase.Add(info.ModuleName, fp);
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

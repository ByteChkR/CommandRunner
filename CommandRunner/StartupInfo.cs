using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommandRunner
{
    /// <summary>
    /// Contains the Logic that contains logic for CLI argument parsing
    /// </summary>
    public class StartupInfo
    {
        public static string LongCommmandPrefix = "--";
        public static string ShortCommandPrefix = "-";
        public static string FilePathPrefix = "@";

        public static bool HasCommandPrefix(string text)
        {
            return text.StartsWith(LongCommmandPrefix) || text.StartsWith(ShortCommandPrefix);
        }

        private List<KeyValuePair<string, List<string>>> _values = new List<KeyValuePair<string, List<string>>>();

        public StartupInfo(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (HasCommandPrefix(args[i]) || i == 0)
                {
                    List<string> argValues = new List<string>();
                    for (int j = i + 1; j < args.Length; j++)
                    {
                        if (HasCommandPrefix(args[i]))
                        {
                            break;
                        }

                        argValues.Add(args[j]);
                    }

                    if (i == 0 && !HasCommandPrefix(args[0]))
                    {
                        argValues.Add(args[0]);
                        _values.Add(new KeyValuePair<string, List<string>>("noflag", argValues));
                    }
                    else
                    {
                        _values.Add(new KeyValuePair<string, List<string>>(args[i], argValues));
                    }
                }
            }
        }

        public List<string> GetValues(string flag, int id = 0)
        {
           return  _values.Where(x => x.Key == flag).ElementAt(id).Value;
        }

        public int GetCommandEntries(string flag)
        {
            return _values.Count(x => x.Key == flag);
        }

        public static List<string> ResolveFileReferences(string arg)
        {
            if (arg.StartsWith(FilePathPrefix))
            {
                return File.ReadAllLines(arg.Remove(0, FilePathPrefix.Length)).ToList();
            }

            return new List<string> {arg};
        }
    }
}
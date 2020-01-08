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
        /// <summary>
        /// Defines the Prefix for Command Keys that are longer.
        /// </summary>
        public static string LongCommmandPrefix = "--";
        /// <summary>
        /// Defines the Prefix for Command Keys that act as shortcuts to longer commands.
        /// </summary>
        public static string ShortCommandPrefix = "-";

        /// <summary>
        /// When used as an Argument the Implementation will load the file after the symbols and use each line as parameter for the command.
        /// </summary>
        public static string FilePathPrefix = "@";

        public int CommandCount => _values.Count;


        /// <summary>
        /// Returns True if the Text is a short or long command.
        /// </summary>
        /// <param name="text">The text to check</param>
        /// <returns></returns>
        public static bool HasCommandPrefix(string text)
        {
            return text.StartsWith(LongCommmandPrefix) || text.StartsWith(ShortCommandPrefix);
        }

        /// <summary>
        /// All Values/Arguments Ordered by First command key.
        /// Multiple Keys Possible.
        /// </summary>
        private List<KeyValuePair<string, List<string>>> _values = new List<KeyValuePair<string, List<string>>>();

        /// <summary>
        /// Public constructors
        /// </summary>
        /// <param name="args">Command Line Input.</param>
        public StartupInfo(string[] args)
        {

            //Resolve File Refs
            List<string> argss = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith(FilePathPrefix))
                {
                    argss.AddRange(ResolveFileReferences(args[i]));
                }
                else
                {
                    argss.Add(args[i]);
                }
            }


            args = argss.ToArray();
            for (int i = 0; i < args.Length; i++)
            {
                if (HasCommandPrefix(args[i]) || i == 0)
                {
                    List<string> argValues = new List<string>();
                    for (int j = i + 1; j < args.Length; j++)
                    {
                        if (HasCommandPrefix(args[j]))
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

        /// <summary>
        /// returns the Values with the specified flag(key)
        /// </summary>
        /// <param name="flag">Flag to return</param>
        /// <param name="id">Index of the occurence</param>
        /// <returns></returns>
        public List<string> GetValues(string flag, int id = 0)
        {
           return  _values.Where(x => x.Key == flag).ElementAt(id).Value;
        }

        /// <summary>
        /// Returns the number of entries in the list with the specified flag
        /// </summary>
        /// <param name="flag">Flag to test</param>
        /// <returns>Count of the Entries with the Same Key</returns>
        public int GetCommandEntries(string flag)
        {
            return _values.Count(x => x.Key == flag);
        }

        /// <summary>
        /// Resolves the FileReference by reading all lines and adding them as arguments(one per line)
        /// </summary>
        /// <param name="arg">Argument with Format: @(PathToFile)</param>
        /// <returns>Lines of the File.</returns>
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
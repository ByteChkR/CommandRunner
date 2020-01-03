using System;
using System.Linq;
using System.Text;

namespace CommandRunner
{
    public abstract class AbstractCommand
    {
        public Action<StartupInfo, string[]> CommandAction { get; }
        public string[] CommandKeys { get; }
        public string HelpText { get; }
        public bool DefaultCommand { get; }

        protected AbstractCommand(Action<StartupInfo, string[]> action, string[] keys, string helpText = "No Help Text Available", bool defaultCommand = false)
        {
            CommandAction = action;
            CommandKeys = keys;
            HelpText = helpText;
            DefaultCommand = defaultCommand;
        }



        public bool IsInterfering(AbstractCommand other)
        {
            for (int i = 0; i < CommandKeys.Length; i++)
            {
                if (other.CommandKeys.Contains(CommandKeys[i])) return true;
            }

            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(CommandKeys[0]);
            for (int i = 1; i < CommandKeys.Length; i++)
            {
                sb.Append(" | " + CommandKeys[i]);
            }

            sb.AppendLine("\nDefault Command: " + DefaultCommand + "\n");
            string[] helpText = HelpText.Split(new[] { '\n' });
            for (int i = 0; i < helpText.Length; i++)
            {
                sb.AppendLine($"\t{helpText[i]}");
            }

            return sb.ToString();
        }
    }
}

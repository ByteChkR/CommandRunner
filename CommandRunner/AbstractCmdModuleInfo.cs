namespace CommandRunner
{
    public abstract class AbstractCmdModuleInfo
    {
        public abstract string ModuleName { get; }

        public abstract void RunArgs(string[] args);
    }
}
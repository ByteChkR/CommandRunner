namespace CommandRunner
{
    public abstract class AbstractCmdModuleInfo
    {
        public abstract string ModuleName { get; }
        public virtual string[] Dependencies => new string[0];
        public abstract void RunArgs(string[] args);
    }
}
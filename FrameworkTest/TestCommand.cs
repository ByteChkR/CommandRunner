using System;
using CommandRunner;

namespace FrameworkTest
{
    public class TestCommand : AbstractCommand
    {
        private static void _TestCommand(StartupInfo info, string[] args)
        {
            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        }
        public TestCommand() : base(_TestCommand, new[] { "--test1", "-t1" }, "Test 1 Command.", true)
        {

        }
    }
}
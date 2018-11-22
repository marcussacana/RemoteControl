using System;
using System.Linq;
using System.Reflection;

namespace RunAsX86 {
    static class Program {

        [STAThread]
        static void Main(string[] Args) {
            if (Args.Length == 0)
                return;

            var Executable = Assembly.LoadFile(Args[0]);

            object Arguments = new string[0];
            if (Args.Length > 1)
                Arguments = Args.Skip(1).ToArray();

            Executable.EntryPoint.Invoke(null, new object[] { Arguments });
        }
    }
}

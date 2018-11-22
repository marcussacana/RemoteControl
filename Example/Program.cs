using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using VNX;

namespace Example {
    public class Program {
        public static int ToInvoke(string Arg) {
            MessageBox.Show(Arg, "From Example.dll", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return int.MaxValue;
        }

        static void Main(string[] args) {
            if (args?.Length == 0) {
                Console.WriteLine("Drag&Drop an EXE File");
                return;
            }
            string Exe = args[0];
            if (!System.IO.File.Exists(Exe)) {
                Console.WriteLine("Drag&Drop an EXE File");
                return;
            }

            RemoteControl Controller = new RemoteControl(Exe, out Process Process);
            Console.WriteLine(Process.Is64Bits() ? "x64 Detected" : "x86 Detected");
            Console.WriteLine($"The process {(Controller.IsCompatibleProcess() ? "is" : "isn't")} compatible with this build of the Example");
            Controller.WaitInitialize();
            Controller.WaitModuleLoad("user32.dll");

            var Modules = Process.GetAllModulesNames();
            foreach (var Module in Modules)
                Console.WriteLine("Module Detected: " + Module);

            Controller.ResumeProcess();
            var Message = Process.AllocString("Wow, I'm called in the target process from the Example!", true);
            var Title = Process.AllocString("This is a test", true);
            var Rst = Controller.Invoke("user32.dll", "MessageBoxW", IntPtr.Zero, Message, Title, new IntPtr(0x00000020 | 0x00000004));
            Console.Write("MSG Reply: ");
            switch (Rst.ToInt32()) {
                case 1:
                    Console.WriteLine("OK");
                    break;
                case 6:
                    Console.WriteLine("YES");
                    break;
                case 7:
                    Console.WriteLine("NO");
                    break;
                case 2:
                    Console.WriteLine("CANCEL");
                    break;
            }


            int Ret = Controller.CLRInvoke(Assembly.GetExecutingAssembly().Location, "LOL, I'm a managed dll inside of the target process!");
            Console.WriteLine("MSG Reply: 0x" + Ret.ToString("X4"));
            Console.WriteLine("Press Any Key To Exit");
            Console.ReadKey();
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using VNX;

namespace Example {
    public class Program {

        //Valid EntryPoint to the managed assembly injector, (returns int, is public and have a single string parameter)
        public static int EntryPoint(string Arg) {
            MessageBox.Show(Arg, "Injected Assembly", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return int.MaxValue;
        }

        static void Main(string[] args) {
            if (args?.Length == 0 || !File.Exists(args[0])) {
                Console.WriteLine("Drag&Drop an EXE File");
                Console.ReadKey();
                return;
            }
            string Exe = args[0];

            //Create the remote control instance and the target process
            RemoteControl Control = new RemoteControl(Exe, out Process Process);

            //Ensure the target process is compatible with this build of the example
            bool Compatible = Control.IsCompatibleProcess();

            
            Console.WriteLine(Process.Is64Bits() ? "x64 Detected" : "x86 Detected");
            Console.WriteLine($"The process {(Compatible ? "is" : "isn't")} compatible with this build of the Example");


            //If are you using an Any CPU build, you can use this to force run as 32bits to make your injection compatible
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "RunAsX86.exe") && !Process.Is64Bits() && !Compatible) {
                Console.WriteLine("Restarting as x86...");
                Process.Start(AppDomain.CurrentDomain.BaseDirectory + "RunAsX86.exe", $"\"{Application.ExecutablePath}\" \"{Exe}\"");
                return;
            }

            //Wait the target process load the minimum requeriment to the RemoteControl works
            Control.WaitInitialize();

            //Wait the target process load the specified dll
            Control.WaitModuleLoad("user32.dll");


            //This is useless to the injection.
            //Shows all loaded modules in the target process
            var Modules = Process.GetAllModulesNames();
            foreach (var Module in Modules) {
                IntPtr Handler = Process.GetModuleByName(Module);
                IntPtr EntryPoint = Process.GetModuleEntryPoint(Handler);
                Console.WriteLine("Module Detected: " + Module);
                Console.WriteLine($"Handler: {Handler.ToString("X16")} | EntryPoint: {EntryPoint.ToString("X16")}");
            }

            //Allow the target process resume the startup
            Control.ResumeProcess();

            //Write the message and title in the target process memory
            var Message = Process.AllocString("Wow, I'm called in the target process from the Example!", true);
            var Title = Process.AllocString("This is a test", true);

            //Invoke the "MessageBoxW" in the target process
            //int MessageBoxW(IntPtr hWnd, string glpText, string lpCaption, uint uType);
            var Rst = Control.Invoke("user32.dll", "MessageBoxW", IntPtr.Zero, Message, Title, new IntPtr(0x20 | 0x04));//0x20 = MB_ICONQUESTION, 0x04 = MB_YESNO

            //Release the Message and Title from the target process memory
            Process.Free(Message);
            Process.Free(Title);

            //Shows the MessageBoxW Clicked Button
            Console.Write("MSG Reply: ");
            switch (Rst.ToInt32()) {
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


            //Here is an example how to inject a Managed Assembly in the process
            string CurrentAssembly = Assembly.GetExecutingAssembly().Location;

            //If you have more than one method like that, you need specify it
            //Since "EntryPoint" is inside the "Program" class, we use typeof(Program).FullName
            int Ret = Control.CLRInvoke(CurrentAssembly, typeof(Program).FullName, "EntryPoint", "LOL, I'm a managed dll inside of the target process!");

            //Show the managed assembly returned data
            Console.WriteLine("Returned: 0x" + Ret.ToString("X4"));
            Console.WriteLine("Press Any Key To Exit");
            Console.ReadKey();
        }
    }
}

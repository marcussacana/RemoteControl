using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
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
                Process.Kill();
                Console.WriteLine("Restarting as x86...");
                Process.Start(AppDomain.CurrentDomain.BaseDirectory + "RunAsX86.exe", $"\"{Application.ExecutablePath}\" \"{Exe}\"");
                return;
            }

            //Load the Minimal requeriment to the RemoteLoader works
            Control.WaitInitialize();

            Console.WriteLine("What you want do?");
            Console.WriteLine("1 - See the Sample Invoke");
            Console.WriteLine("2 - See the Sample Managed Assembly Injection 1");
            Console.WriteLine("3 - See the Sample Managed Assembly Injection 2");
            Console.WriteLine("4 - See the Sample Modules Info");
            Console.WriteLine("5 - See the Sample Read/Write in the target memory");
            Console.WriteLine("6 - See the Sample Auto-Alloc in the target memory");

            char R = Console.ReadKey().KeyChar;
            Console.WriteLine();

            switch (R) {
                case '1':
                    SampleExportInvoke(Control, Process);
                    break;
                case '2':
                    SampleManagedAssemblyInjection1(Control, Process);
                    break;
                case '3':
                    SampleManagedAssemblyInjection2(Control, Process);
                    break;
                case '4':
                    SampleModuleInfo(Control, Process);
                    break;
                case '5':
                    SampleReadWrite(Control, Process);
                    break;
                case '6':
                    SampleMAlloc(Control, Process);
                    break;
            }

            Console.WriteLine("Press Any Key To Exit");
            Console.ReadKey();
        }

        /// <summary>
        /// Sample Invoke a DLL Export in the target process and catch the return data
        /// </summary>
        public static void SampleExportInvoke(RemoteControl Control, Process Process) {
            //Lock the program in his entrypoint, Required to invoke methods or inject libraries
            Control.LockEntryPoint();

            //Write the message and title in the target process memory
            var Message = Process.MAllocString("Wow, I'm called in the target process from the Example!", true);
            var Title = Process.MAllocString("This is a test", true);


            //Invoke the "MessageBoxW" in the target process, if the user32.dll isn't loaded, he will be automatically loaded
            //int MessageBoxW(IntPtr hWnd, string glpText, string lpCaption, uint uType);
            var Rst = Control.Invoke("user32.dll", "MessageBoxW", IntPtr.Zero, Message, Title, new IntPtr(0x20 | 0x04));//0x20 = MB_ICONQUESTION, 0x04 = MB_YESNO

            //Release the Message and Title from the target process memory
            Process.MFree(Message);
            Process.MFree(Title);

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

            //Allow the program startup continue
            Control.UnlockEntryPoint();
        }

        /// <summary>
        /// Inject a Managed Assembly when the assembly have more than one entry point
        /// </summary>
        public static void SampleManagedAssemblyInjection1(RemoteControl Control, Process Process) {
            //Lock the program in his entrypoint, Required to invoke methods or inject libraries
            Control.LockEntryPoint();

            //Get Current Assembly Path
            string CurrentAssembly = Assembly.GetExecutingAssembly().Location;

            //If you have more than one method like that, you need specify it
            //Since "EntryPoint" is inside the "Program" class, we use typeof(Program).FullName
            int Ret = Control.CLRInvoke(CurrentAssembly, typeof(Program).FullName, "EntryPoint", "LOL, I'm a managed dll inside of the target process!");

            //Show the managed assembly returned data
            Console.WriteLine("Returned: 0x" + Ret.ToString("X4"));


            //Allow the program startup continue
            Control.UnlockEntryPoint();
        }

        /// <summary>
        /// Inject the managed assembly when only have a single valid entrypoint
        /// </summary>
        public static void SampleManagedAssemblyInjection2(RemoteControl Control, Process Process) {
            //Get Current Assembly Path
            string CurrentAssembly = Assembly.GetExecutingAssembly().Location;

            //If you have more than one method like that, you need specify it
            //If you only have one valid entrypoint to the injector, you don't need specifiy it 
            int Ret = Control.CLRInvoke(CurrentAssembly, "LOL, I'm a managed dll inside of the target process!");

            //Show the managed assembly returned data
            Console.WriteLine("Returned: 0x" + Ret.ToString("X4"));


            //Allow the program startup continue
            Control.UnlockEntryPoint();
        }

        /// <summary>
        /// Shows all modules in the target process
        /// </summary>
        public static void SampleModuleInfo(RemoteControl Control, Process Process) {
            //Remove the "SUSPENDED" state of the process
            Control.ResumeProcess();

            //Wait the main window open
            while (Process.MainWindowHandle == IntPtr.Zero)
                Thread.Sleep(100);

            //Log all modules
            var Modules = Process.GetAllModules();
            foreach (var Module in Modules) {
                bool Main = Module == Process.GetMainModule();
                if (Main)
                    Console.Write("Main ");

                string Parse = Process.Is64Bits() ? "X16" : "X8";

                Console.WriteLine("Module:" + (Main ? "\t" : "\t\t") + Process.GetModuleNameByHandler(Module));
                Console.WriteLine("Handler:\t0x" + Module.ToString(Parse));
                Console.WriteLine("EntryPoint:\t0x" + Process.GetModuleEntryPoint(Module).ToString(Parse));
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Read or Write data in the target process memory
        /// </summary>
        public static void SampleReadWrite(RemoteControl Control, Process Process) {
            //Remove the "SUSPENDED" state of the process
            Control.ResumeProcess();

            //Wait the main window open
            while (Process.MainWindowHandle == IntPtr.Zero)
                Thread.Sleep(100);


            bool? Read = null;
            while (Read == null) {
                Console.WriteLine("Type R to Read or W to Write");
                switch (Console.ReadKey().KeyChar) {
                    case 'R':
                    case 'r':
                        Read = true;
                        break;
                    case 'W':
                    case 'w':
                        Read = false;
                        break;
                }
                Console.WriteLine();
            }

            Console.WriteLine("Type the absolute address (in Hex)");
            string Reply = Console.ReadLine().ToUpper().Replace("0X", "");
            long Addr = Convert.ToInt64(Reply, 16);

            if (Read.Value) {
                Console.WriteLine("How many bytes you want read?");
                uint Count = uint.Parse(Console.ReadLine());

                //Here is where the magic occours
                byte[] Buffer = Process.Read(new IntPtr(Addr), Count);

                Console.Write("0x");
                foreach (byte Byte in Buffer) {
                    Console.Write(Byte.ToString("X2"));
                }

                Console.WriteLine();
            } else {
                Console.WriteLine("What you want write? (a string)");
                byte[] Buffer = Encoding.UTF8.GetBytes(Console.ReadLine());


                //Here is where the magic occours
                Process.Write(new IntPtr(Addr), Buffer);

                Console.WriteLine("Writed...");
            }
        }

        public static void SampleMAlloc(RemoteControl Control, Process Process) {
            //Remove the "SUSPENDED" state of the process
            Control.ResumeProcess();

            const string Message = "This is a test of the auto memory allocator of the RemoteProcess library, This string is big as try to fill the 10mb of space with less calls... Well, let's see how this will works, are you ready?";
            string Parse = Process.Is64Bits() ? "X16" : "X8";
            byte[] Data = Encoding.Unicode.GetBytes(Message + "\x0");

            Console.WriteLine("Writing");

            //Allocate 100x
            IntPtr[] Address = new IntPtr[100];
            for (uint i = 0; i < Address.Length; i++)
                Address[i] = Process.MAlloc(Data);

            Console.WriteLine("Validating...");
            for (uint i = 0; i < Address.Length; i++) {
                string Content = Process.ReadString(Address[i], true);
                if (Content != Message)
                    Console.WriteLine("Validation Failed at 0x"+ Address[i].ToString(Parse));
            }

            Console.WriteLine("Disposing...");

            //Release all 100 address
            for (uint i = 0; i < Address.Length; i++)
                Process.MFree(Address[i]);

            Console.WriteLine("Finished");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using static VNX.Debugger;
using static VNX.Imports;
using static VNX.Tools;

namespace VNX {
    public class RemoteControl {
        bool x64Bits;
        IntPtr hProcess;
        Process Target;

        IntPtr ExecuteInDefaultAppDomain = IntPtr.Zero;
        IntPtr CLRRuntimeHost = IntPtr.Zero;


        int OwnerThread = 0;
        IntPtr MainThread;
        List<LockInfo> Locks = new List<LockInfo>();
        List<uint> RemoteThreads = new List<uint>();

        bool Debugging = false;
        public bool IsManaged { get; private set; } = false;

        bool CLRNOTAVAILABLE = false;

        public RemoteControl(Process Proc) {
            Target = Proc;
            x64Bits = Is64Bit(Proc);

            hProcess = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, Proc.Id);
        }


        /// <summary>
        /// Create a new process suspended, call the <see cref="ResumeProcess"/> to begin the process execution
        /// </summary>
        /// <param name="Filename">Executable Path</param>
        /// <param name="Arguments">Command Line Arguments</param>
        /// <param name="CreatedProcess">The Created Process</param>
        /// <param name="WorkingDirectory">The working directory to the created process use</param>
        public RemoteControl(string Filename, out Process CreatedProcess, string Arguments = "", string WorkingDirectory = null) {
            var PI = CreateProcessSuspended(Filename, Arguments, WorkingDirectory);

            hProcess = PI.hProcess;
            Target = Process.GetProcessById(unchecked((int)PI.dwProcessId));
            CreatedProcess = Target;
            MainThread = PI.hThread;

            x64Bits = Is64Bit(Target);
            IsManaged = Tools.IsManaged(Filename);
            Debugging = true;

            OwnerThread = Thread.CurrentThread.ManagedThreadId;

        }


        /// <summary>
        /// Verify if you can use the <see cref="RemoteControl">RemoteLoader</see> with the target process
        /// </summary>
        /// <returns>If is an compatible proccess</returns>
        public bool IsCompatibleProcess() => x64Bits == Environment.Is64BitProcess;

        /// <summary>
        /// Wait the process Initialize the required data to the <see cref="RemoteControl"/> works
        /// </summary>
        public void WaitInitialize() => WaitModuleLoad("Kernel32.dll");

        /// <summary>
        /// Verify if the target process is an .net proccess
        /// </summary>
        /// <returns>If the CLR is present in the process, returns true, otherwise returns false</returns>
        public bool CLRAvaliable() => Target.LibraryLoaded("mscoree.dll");

        /// <summary>
        /// Lock the EntryPoint Execution
        /// </summary>
        public void LockEntryPoint() {
            if (IsManaged) {
                throw new Exception("The entry point locker isn't compatible with managed targets, use the WaitCLR instead.");
            }

            SuspendThreads();

            byte[] LockerData = new byte[] { 0xEB, 0xFE };//Infinite Loop (jmp -2)
            IntPtr LockAddress = Target.GetModuleEntryPoint(Target.GetMainModule());


            Locks.Add(new LockInfo() {
                Address = LockAddress,
                Locker = LockerData,
                Unlocker = Target.Read(LockAddress, (uint)LockerData.Length),
                Target = Target
            });

            foreach (LockInfo Locker in Locks) {
                Locker.Install();
                if (!Locker.IsInstalled())
                    throw new Exception("Failed to lock the entrypoint...");
            }

            SetThreadPriority(MainThread, THREAD_PRIORITY.LOWEST);
            ResumeProcess();
        }

        /// <summary>
        /// Unlock the EntryPoint allowing his startup
        /// </summary>
        public void UnlockEntryPoint() {
            SuspendThreads();

            foreach (LockInfo Locker in Locks) {
                Locker.Uninstall();
                if (Locker.IsInstalled())
                    throw new Exception("Failed to unlock the entrypoint...");
            }


            Locks = new List<LockInfo>();

            SetThreadPriority(MainThread, THREAD_PRIORITY.NORMAL);
            ResumeThreads();
        }

        /// <summary>
        /// If the process has created using the <see cref="RemoteControl"/> you need call this function to start the process
        /// </summary>
        public void ResumeProcess() {
            if (Debugging)
                Detach();
            ResumeThreads();
        }

        /// <summary>
        /// Resume the target process if you have suspended it
        /// </summary>
        /// <param name="SkipInjectedThreads">When true all threads of the process that isn't created by the <see cref="RemoteControl">RemoteControl</see> will be Resumed</param>
        public void ResumeThreads(bool SkipInjectedThreads = true) {
            foreach (var Thread in Target.Threads.Cast<ProcessThread>()) {
                uint TID = (uint)Thread.Id;
                var Handle = OpenThread(ThreadAccess.SUSPEND_RESUME, false, TID);
                if (RemoteThreads.Contains(TID))
                    continue;

                while (ResumeThread(Handle) > 0)
                    continue;

                CloseHandle(Handle);
            }
        }

        /// <summary>
        /// Suspend the target process if you have suspended it
        /// </summary>
        /// <param name="SkipInjectedThreads">When true all threads of the process that isn't created by the <see cref="RemoteControl">RemoteControl</see> will be Suspended</param>
        public void SuspendThreads(bool SkipInjectedThreads = true) {
            foreach (var Thread in Target.Threads.Cast<ProcessThread>()) {
                uint TID = (uint)Thread.Id;
                var Handle = OpenThread(ThreadAccess.SUSPEND_RESUME, false, TID);
                if (RemoteThreads.Contains(TID))
                    continue;

                SuspendThread(Handle);
                CloseHandle(Handle);
            }
        }

        /// <summary>
        /// Wait the target process load a certain module
        /// </summary>
        public void WaitModuleLoad(string Module) {
            if (Target.LibraryLoaded(Module))
                return;

            if (MainThread == IntPtr.Zero)
                throw new Exception("You need create the process with the RemoteLoader to wait an module load event");

            if (!Debugging)
                throw new Exception("You need call this function before the ResumeProcess or LockEntryPoint");

            if (OwnerThread != Thread.CurrentThread.ManagedThreadId)
                throw new Exception("You need call this function using the same thread when you created this instance of the RemoteControl");


            new Thread(() => {
                Thread.Sleep(100);
                ResumeThreads();
            }).Start();

            bool Loaded = false;
            while (!Loaded && WaitForDebugEvent(out DEBUG_EVENT Event, INFINITE)) {
                switch (Event.dwDebugEventCode) {
                    case DebugEvent.LOAD_DLL_DEBUG_EVENT:
                        Loaded = Target.LibraryLoaded(Module);
                        break;
                }
                ContinueDebugEvent(Event.dwProcessId, Event.dwThreadId, DEBUG_CONTINUE.DBG_CONTINUE);
            }
            SuspendThreads();
        }

        /// <summary>
        /// Wait the target process load the CLR
        /// </summary>
        public void WaitCLR() {
            if (!IsManaged)
                throw new Exception("The target process isn't an managed assembly");

            WaitInitialize();

            if (MainThread == IntPtr.Zero)
                throw new Exception("You need create the process with the RemoteLoader to wait an module load event");

            if (!Debugging)
                throw new Exception("You need call this function before the ResumeProcess or LockEntryPoint");

            if (OwnerThread != Thread.CurrentThread.ManagedThreadId)
                throw new Exception("You need call this function using the same thread when you created this instance of the RemoteControl");


            new Thread(() => {
                Thread.Sleep(100);
                ResumeThreads();
            }).Start();

            bool Loaded = false;
            while (!Loaded && WaitForDebugEvent(out DEBUG_EVENT Event, INFINITE)) {
                switch (Event.dwDebugEventCode) {
                    case DebugEvent.EXCEPTION_DEBUG_EVENT:
                        if (CLRAvaliable())
                            Loaded = true;                        
                        break;
                }
                ContinueDebugEvent(Event.dwProcessId, Event.dwThreadId, DEBUG_CONTINUE.DBG_CONTINUE);
            }
            SuspendThreads();
        }

        /// <summary>
        /// Detach the RemoteLoader debugger from the process
        /// <para>After call this function you can't use the WaitModuleLoad Function more</para>
        /// </summary>
        public void Detach() {
            if (!Debugging)
                return;

            Debugging = false;
            DebugActiveProcessStop((uint)Target.Id);
        }

        /// <summary>
        /// Inject the CLR in the target process
        /// </summary>
        /// <param name="Version">Version to load in the target process</param>
        /// <returns>A Pointer to the <a href="https://docs.microsoft.com/en-us/dotnet/framework/unmanaged-api/hosting/iclrruntimehost-executeindefaultappdomain-method">ExecuteInDefaultAppDomain</a> function</returns>
        public void StartCLR(FrameworkVersion Version = FrameworkVersion.DotNet40) {
            if (!IsCompatibleProcess())
                throw new Exception($"The Target Process isn't an {(Environment.Is64BitProcess ? "x64" : "x32")} Process");

            if (Debugging)
                WaitInitialize();

            if (IsManaged)
                ResumeProcess();
            

            IntPtr ICLRMetaHost    = Target.MAlloc(new byte[IntPtr.Size]);//http://source.roslyn.codeplex.com/#Microsoft.CodeAnalysis/Interop/IClrMetaHost.cs
            IntPtr ICLRRuntimeInfo = Target.MAlloc(new byte[IntPtr.Size]);//http://source.roslyn.codeplex.com/#microsoft.codeanalysis/Interop/IClrRuntimeInfo.cs,485a48c96d61baeb,references
            IntPtr ICLRRuntimeHost = Target.MAlloc(new byte[IntPtr.Size]);//https://github.com/CentroEPiaggio/SoftLEGS-Project/blob/5b2eb55b8f43f07189118e477bd4e5c0114f6c8e/casadi-matlabR2013a-v3.1.1/casadi/jit/mingw/mscoree.h#L1230

            IntPtr CLSID_CLRMetaHost    = Target.MAlloc(new Guid("9280188D-0E8E-4867-B30C-7FA83884E8DE").ToByteArray());
            IntPtr CLSID_CLRRuntimeHost = Target.MAlloc(new Guid("90F1A06E-7712-4762-86B5-7A5EBA6BDB02").ToByteArray());
            IntPtr IID_ICLRMetaHost     = Target.MAlloc(new Guid("D332DB9E-B9B3-4125-8207-A14884F53216").ToByteArray());
            IntPtr IID_ICLRRuntimeInfo  = Target.MAlloc(new Guid("BD39D1D2-BA2F-486A-89B0-B4B0CB466891").ToByteArray());
            IntPtr IID_ICLRRuntimeHost  = Target.MAlloc(new Guid("90F1A06C-7712-4762-86B5-7A5EBA6BDB02").ToByteArray());

            HResult Result = (MIntPtr<HResult>)Invoke("mscoree.dll", "CLRCreateInstance", CLSID_CLRMetaHost, IID_ICLRMetaHost, ICLRMetaHost);
            if (Result != HResult.S_OK)
                throw new Exception($"Error: {Result.ToString()} (0x{((uint)Result).ToString("X8")})");

            byte[] Data = Target.Read(ICLRMetaHost, (uint)IntPtr.Size);
            IntPtr GetRuntime = Data.ToIntPtr(x64Bits);
            IntPtr MetaHostIntance = GetRuntime;

            Data = Target.Read(GetRuntime, (uint)IntPtr.Size);
            GetRuntime = Data.ToIntPtr(x64Bits).Sum(IntPtr.Size * 3);//1st function, The vTable have 3 pointers before the first function

            Data = Target.Read(GetRuntime, (uint)IntPtr.Size);
            GetRuntime = Data.ToIntPtr(x64Bits);

            IntPtr pVersion = Target.MAllocString(DotNetVerName(FrameworkVersion.DotNet40), true);


            Result = (MIntPtr<HResult>)Invoke(GetRuntime, MetaHostIntance, pVersion, IID_ICLRRuntimeInfo, ICLRRuntimeInfo);
            if (Result != HResult.S_OK)
                throw new Exception($"Error: {Result.ToString()} (0x{((uint)Result).ToString("X8")})");

            Data = Target.Read(ICLRRuntimeInfo, (uint)IntPtr.Size);
            IntPtr GetInterface = Data.ToIntPtr(x64Bits);
            IntPtr RuntimeInfoInstance = GetInterface;

            Data = Target.Read(GetInterface, (uint)IntPtr.Size);
            GetInterface = Data.ToIntPtr(x64Bits);

            Data = Target.Read(GetInterface.Sum(IntPtr.Size * 9), (uint)IntPtr.Size);//6th Function + vTable Prefix
            GetInterface = Data.ToIntPtr(x64Bits);

            Result = (MIntPtr<HResult>)Invoke(GetInterface, RuntimeInfoInstance, CLSID_CLRRuntimeHost, IID_ICLRRuntimeHost, ICLRRuntimeHost);
            if (Result != HResult.S_OK)
                throw new Exception($"Error: {Result.ToString()} (0x{((uint)Result).ToString("X8")})");

            Data = Target.Read(ICLRRuntimeHost, (uint)IntPtr.Size);
            IntPtr Start = Data.ToIntPtr(x64Bits);
            IntPtr HostInstance = Start;

            Data = Target.Read(Start, (uint)IntPtr.Size);
            Start = Data.ToIntPtr(x64Bits);

            if (IsManaged || (CLRAvaliable() && !CLRNOTAVAILABLE)) {
                Data = Target.Read(Start.Sum(IntPtr.Size * 11), (uint)IntPtr.Size);
                ExecuteInDefaultAppDomain = Data.ToIntPtr(x64Bits);

                Target.MFree(ICLRMetaHost);
                Target.MFree(ICLRRuntimeInfo);
                Target.MFree(ICLRRuntimeHost);
                Target.MFree(CLSID_CLRMetaHost);
                Target.MFree(CLSID_CLRRuntimeHost);
                Target.MFree(IID_ICLRMetaHost);
                Target.MFree(IID_ICLRRuntimeHost);
                Target.MFree(pVersion);

                return;
            }

            Data = Target.Read(Start.Sum(IntPtr.Size * 3), (uint)IntPtr.Size);
            Start = Data.ToIntPtr(x64Bits);

            Result = (MIntPtr<HResult>)Invoke(Start, HostInstance);
            if (Result != HResult.S_OK)
                throw new Exception($"Error: {Result.ToString()} (0x{((uint)Result).ToString("X8")})");

            Data = Target.Read(ICLRRuntimeHost, (uint)IntPtr.Size);
            ExecuteInDefaultAppDomain = Data.ToIntPtr(x64Bits);
            CLRRuntimeHost = ExecuteInDefaultAppDomain;
            Data = Target.Read(ExecuteInDefaultAppDomain, (uint)IntPtr.Size);
            ExecuteInDefaultAppDomain = Data.ToIntPtr(x64Bits);
            Data = Target.Read(ExecuteInDefaultAppDomain.Sum(IntPtr.Size * 11), (uint)IntPtr.Size);
            ExecuteInDefaultAppDomain = Data.ToIntPtr(x64Bits);            

            Target.MFree(ICLRMetaHost);
            Target.MFree(ICLRRuntimeInfo);
            Target.MFree(ICLRRuntimeHost);
            Target.MFree(CLSID_CLRMetaHost);
            Target.MFree(CLSID_CLRRuntimeHost);
            Target.MFree(IID_ICLRMetaHost);
            Target.MFree(IID_ICLRRuntimeHost);
            Target.MFree(pVersion);
        }

        /// <summary>
        /// Inject a <see cref="Assembly">Assembly</see> in the target process and
        /// invoke the specified method.
        /// </summary>
        /// <param name="AssemblyPath">The <see cref="Assembly">Assembly</see> file path</param>
        /// <param name="TypeName">The Full Name to the method class</param>
        /// <param name="MethodName">The Method Name in the Class</param>
        /// <param name="Argument">A Argument to give to the invoke method</param>
        /// <returns>The returned value from the invoked method</returns>
        public int CLRInvoke(string AssemblyPath, string TypeName, string MethodName, string Argument) {
            if (!(MIntPtr)ExecuteInDefaultAppDomain)
                StartCLR();

            IntPtr Ret = Target.MAlloc(new byte[4]);

            IntPtr AsmPath = Target.MAllocString(AssemblyPath, true);
            IntPtr TpName = Target.MAllocString(TypeName, true);
            IntPtr MthdName = Target.MAllocString(MethodName, true);
            IntPtr Arg = Target.MAllocString(Argument, true);

            HResult Failed = (MIntPtr<HResult>)Invoke(ExecuteInDefaultAppDomain, CLRRuntimeHost, AsmPath, TpName, MthdName, Arg, Ret);
            switch (Failed) {
                case HResult.S_OK:
                    break;

                case HResult.HOST_E_CLRNOTAVAILABLE:
                    if (CLRNOTAVAILABLE)
                        goto default;

                    CLRNOTAVAILABLE = true;
                    ExecuteInDefaultAppDomain = IntPtr.Zero;
                    return CLRInvoke(AssemblyPath, TypeName, MethodName, Argument);

                default:
                    throw new Exception($"Error: {Failed.ToString()} (0x{((uint)Failed).ToString("X8")})");
            }
            int Result = Target.Read(Ret, 4).ToInt32();

            Target.MFree(AsmPath);
            Target.MFree(TpName);
            Target.MFree(MthdName);
            Target.MFree(Arg);
            Target.MFree(Ret);

            return Result;
        }

        /// <summary>
        /// Inject a <see cref="Assembly">Assembly</see> in the target process
        /// and Invoke the first matched method with 'public static int EntryPoint(string Arg)' in the <see cref="Assembly">Assembly</see>
        /// </summary>
        /// <param name="AssemblyPath">The <see cref="Assembly">Assembly</see> file path</param>
        /// <returns>The returned value from the invoked method</returns>
        public int CLRInvoke(string AssemblyPath, string Argument = null) {
            if (!(MIntPtr)ExecuteInDefaultAppDomain)
                StartCLR();

            AppDomain Domain = AppDomain.CreateDomain("Assembly Loader");
            var Asm = Domain.Load(File.ReadAllBytes(AssemblyPath));
            var Functions = (from x in Asm.GetExportedTypes() select x.GetMethods()).ToArray();
            for (uint x = 0; x < Functions.LongLength; x++)
                for (uint y = 0; y < Functions[x].LongLength; y++) {
                    var Method = Functions[x][y];
                    if (Method.ReturnType != typeof(int))
                        continue;
                    if (!Method.IsStatic || !Method.IsPublic)
                        continue;
                    var Parameters = Method.GetParameters();
                    long Required = (from z in Parameters where !z.IsOptional select z).LongCount();
                    if (Required != 1)
                        continue;
                    if (Parameters[0].ParameterType != typeof(string))
                        continue;
                    AppDomain.Unload(Domain);
                    return CLRInvoke(AssemblyPath, Method.DeclaringType.FullName, Method.Name, Argument);
                }

            AppDomain.Unload(Domain);
            throw new Exception("No Valid EntryPoint Found in the target assembly");
        }

        /// <summary>
        /// Verify if the specified DLL is compatible with the target process
        /// </summary>
        /// <param name="Path">Path to an module</param>
        /// <returns>If compatible, returns true otherwise returns false</returns>
        public bool IsCompatibleModule(string Path) => Is64Bit(Path) == x64Bits;

        /// <summary>
        /// Load an Library in the target proccess
        /// <para>This will cause the module to be loaded in the current process</para>
        /// </summary>
        /// <param name="Library">Library Name or Path</param>
        /// <returns>The Module Handler</returns>
        public IntPtr LoadLibrary(string Library) {
            IntPtr ModuleName = Target.MAllocString(Library, true);
            IntPtr Result = Invoke("kernel32.dll", "LoadLibraryW", ModuleName);
            Target.MFree(ModuleName);
            return Result;
        }

        /// <summary>
        /// Invoke a function with arguments and get the return value
        /// </summary>
        /// <param name="Module">The module of the function</param>
        /// <param name="Function">The function to be called</param>
        /// <param name="Arguments">Arguments to be given to the function</param>
        /// <returns>The return function value</returns>
        public IntPtr Invoke(string Module, string Function, params IntPtr[] Arguments) => Invoke(GetProcAddress(Module, Function), Arguments);

        /// <summary>
        /// Invoke an function without wait the execution
        /// </summary>
        /// <param name="Module">The module of the function</param>
        /// <param name="Function">The function to be called</param>
        /// <param name="Arguments">Arguments to be given to the function</param>
        public void BeginInvoke(string Module, string Function, params IntPtr[] Arguments) => BeginInvoke(GetProcAddress(Module, Function), Arguments);


        /// <summary>
        /// Invoke a function with arguments and get the return value
        /// </summary>
        /// <param name="FuncAddr">The function address to be called</param>
        /// <param name="Arguments">Arguments to be given to the function</param>
        /// <returns>The return function value</returns>
        public IntPtr Invoke(IntPtr FuncAddr, params IntPtr[] Arguments) {
            if (!IsCompatibleProcess())
                throw new Exception($"The Target Process isn't an {(Environment.Is64BitProcess ? "x64" : "x32")} Process");

            if (x64Bits)
                return x64Invoke(FuncAddr, Arguments);
            return x32Invoke(FuncAddr, Arguments);
        }

        /// <summary>
        /// Invoke an function without wait the execution
        /// </summary>
        /// <param name="FuncAddr">The function address to be called</param>
        /// <param name="Arguments">Arguments to be given to the function</param>
        public void BeginInvoke(IntPtr FuncAddr, params IntPtr[] Arguments) {
            if (!IsCompatibleProcess())
                throw new Exception($"The Target Process isn't an {(Environment.Is64BitProcess ? "x64" : "x32")} Process");

            if (x64Bits)
                x64BeginInvoke(FuncAddr, Arguments);
            else
                x32BeginInvoke(FuncAddr, Arguments);
        }

        /// <summary>
        /// Get the pointer to an function in the target process
        /// <para>This will cause the module to be loaded in the current process</para>
        /// </summary>
        /// <param name="Module">The module of the function</param>
        /// <param name="Function">The function to be searched</param>
        /// <returns>The function address</returns>
        public IntPtr GetProcAddress(string Module, string Function) => GetProcAddressEx(Module, Function);

        private bool LibraryLoadedLocal(string Library) {
            string Module = Path.GetFileName(Library).ToLower();
            var Modules = Process.GetCurrentProcess().Modules.Cast<ProcessModule>();
            var Rst = (from x in Modules where x.ModuleName.ToLower() == Module select x).ToArray();

            return Rst.Length != 0;
        }

        private IntPtr GetProcAddressEx(string Module, string Function) {
            if (!IsCompatibleProcess())
                throw new Exception($"The Target Process isn't an {(Environment.Is64BitProcess ? "x64" : "x32")} Process");

            if (!Target.LibraryLoaded("kernel32.dll"))
                return IntPtr.Zero;

            if (!Target.LibraryLoaded(Module))
                if (LoadLibrary(Module) == IntPtr.Zero)
                    return IntPtr.Zero;

            if (LibraryLoadedLocal(Module))
                return GetRemoteProcAddress(Module, Function);


            IntPtr GetModuleHld = GetRemoteProcAddress("kernel32.dll", "GetModuleHandleW");
            IntPtr GetProcAddr = GetRemoteProcAddress("kernel32.dll", "GetProcAddress");
            IntPtr ModuleName = Target.MAllocString(Path.GetFileName(Module), true);
            IntPtr FuncName = Target.MAllocString(Function, false);

            IntPtr hModule = Invoke(GetModuleHld, ModuleName);
            IntPtr Result = Invoke(GetProcAddr, hModule, FuncName);

            Target.MFree(ModuleName);
            Target.MFree(FuncName);

            return Result;
        }

        private MIntPtr GetRemoteProcAddress(string Module, string Function) {
            Target.Refresh();

            var Handle = LoadLibraryA(Module);
            MIntPtr func = Imports.GetProcAddress(Handle, Function);
            if (!func)
                return IntPtr.Zero;

            ulong offset = func.ToUInt64() - Handle.ToUInt64();            

            return Target.GetModuleByName(Module).Sum(offset);
        }

        private void x32BeginInvoke(IntPtr FuncAddr, params IntPtr[] Arguments) => x32Invoke(FuncAddr, false, Arguments);
        private IntPtr x32Invoke(IntPtr FuncAddr, params IntPtr[] Arguments) => x32Invoke(FuncAddr, true, Arguments);
        private IntPtr x32Invoke(IntPtr FuncAddr, bool CatchReturn, params IntPtr[] Arguments) {
            using (MemoryStream Stream = new MemoryStream()) {

                //Move CreateThread paramter to EAX and fix stack
                byte[][] Commands = new byte[][] {
                    //pop eax
                    new byte[] { 0x58 },
                    //xchg [esp], eax
                    new byte[] { 0x87, 0x04,  0x24 }
                };
                foreach (byte[] Command in Commands)
                    Stream.Write(Command, 0, Command.Length);


                //Push All Arguments
                foreach (IntPtr Argument in Arguments.Reverse()) {
                    Stream.WriteByte(0x68);//Push DW
                    Stream.Write(BitConverter.GetBytes(Argument.ToInt32()), 0, 4);
                }

                Commands = new byte[][] {
                    //mov eax, FuncAddr
                    new byte[] { 0xB8 }.Append(FuncAddr.ToUInt32().GetBytes()),
                    //call eax
                    new byte[] { 0xFF, 0xD0 },
                };

                if (CatchReturn)
                    Commands = Commands.Append(new byte[][] {
                        //push ebx
                        new byte[] { 0x53 },                    
                        //Call NextInstruction
                        new byte[] { 0xE8, 0x00, 0x00, 0x00, 0x00 },
                        //pop ebx
                        new byte[] { 0x5B },
                        //mov dword [ebx+0xB], eax
                        new byte[] { 0x89, 0x43, 0x0B },
                        //mov byte  [ebx+0xA], 0x1
                        new byte[] { 0xC6, 0x43, 0x0A, 0x01 },
                        //pop ebx
                        new byte[] { 0x5B }
                    });

                Commands = Commands.Append(new byte[][] {
                    //ret
                    new byte[] { 0xC3 }
                });

                if (CatchReturn)
                    Commands = Commands.Append(new byte[][] {
                        //EBX+A
                        new byte[] { 0x00 },
                        //EBX+B
                        new byte[] { 0x00, 0x00, 0x00, 0x00 }
                    });

                foreach (byte[] Command in Commands)
                    Stream.Write(Command, 0, Command.Length);


                IntPtr Function = Target.MAlloc(Stream.ToArray(), true);
                IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, Function, IntPtr.Zero, 0, IntPtr.Zero);

                uint TID = GetThreadId(hThread);
                RemoteThreads.Add(TID);

                while (ResumeThread(hThread) > 0) {
                    GetExitCodeThread(hThread, out uint Status);
                    if (Status != THREAD_STILL_ACTIVE)
                        break;

                    Thread.Sleep(10);
                }

                if (CatchReturn) {
                    IntPtr RetData = Function.Sum(Stream.Length - 5);

                    uint Status = 0;
                    byte[] Buffer = new byte[5];
                    while (true) {
                        Thread.Sleep(10);
                        Buffer = Target.Read(RetData, (uint)Buffer.LongLength);
                        if (Buffer[0] != 0x1) {
                            GetExitCodeThread(hThread, out Status);
                            if (Status == THREAD_STILL_ACTIVE)
                                continue;
                            else
                                throw new Exception("An Unmanaged Exception in the target process has occured");
                        }
                        break;
                    }

                    do {
                        GetExitCodeThread(hThread, out Status);
                    } while (Status == THREAD_STILL_ACTIVE);

                    RemoteThreads.Remove(TID);

                    Target.MFree(Function);

                    return BitConverter.ToUInt32(Buffer, 1).ToIntPtr();
                } else
                    return IntPtr.Zero;
            }
        }

        private void x64BeginInvoke(IntPtr FuncAddr, params IntPtr[] Arguments) => x64Invoke(FuncAddr, false, Arguments);
        private IntPtr x64Invoke(IntPtr FuncAddr, params IntPtr[] Arguments) => x64Invoke(FuncAddr, true, Arguments);
        private IntPtr x64Invoke(IntPtr FuncAddr, bool CatchReturn, params IntPtr[] Arguments) {
            using (MemoryStream Stream = new MemoryStream()) {
                byte[][] Commands = new byte[][] {
                    //push rbp
                    new byte [] { 0x55 },
                    //mov rbp, rsp
                    new byte[] { 0x48, 0x89, 0xE5 },
                    
                    //(2 times to keep the stack padding)
                    //push rbx 
                    new byte[] { 0x53 },
                    //push rbx
                    new byte[] { 0x53 }
                };
                foreach (byte[] Command in Commands)
                    Stream.Write(Command, 0, Command.Length);

                uint MinStack = 0x30;
                uint StackLen = MinStack;
                if (Arguments.Length > 4)
                    StackLen += ((uint)((Arguments.LongLength - 4) + (Arguments.LongLength % 2))) * 8;


                Commands = new byte[][] {
                    //sub rsp, MinStack
                    new byte[] { 0x48, 0x81, 0xEC }.Append(MinStack.GetBytes())
                };
                foreach (byte[] Command in Commands)
                    Stream.Write(Command, 0, Command.Length);


                int ID = Arguments.Length;
                foreach (IntPtr Argument in Arguments.Reverse()) {
                    switch (--ID) {
                        case 0:
                            Commands = new byte[][] {
                                //movabs rcx, Argument
                                new byte[] { 0x48, 0xB9 }.Append(Argument.ToUInt64().GetBytes())
                            };
                            break;
                        case 1:
                            Commands = new byte[][] {
                                //movabs rdx, Argument
                                new byte[] { 0x48, 0xBA }.Append(Argument.ToUInt64().GetBytes())
                            };
                            break;
                        case 2:
                            Commands = new byte[][] {
                                //movabs r8, Argument
                                new byte[] { 0x49, 0xB8 }.Append(Argument.ToUInt64().GetBytes())
                            };
                            break;
                        case 3:
                            Commands = new byte[][] {
                                //movabs r9, Argument
                                new byte[] { 0x49, 0xB9 }.Append(Argument.ToUInt64().GetBytes())
                            };
                            break;
                        default:
                            Commands = new byte[][] {
                                //push "4 last bytes from Argument"
                                new byte[] { 0x68 }.Append(Argument.ToUInt32().GetBytes()),
                                //mov [rsp+4], "4 frist bytes from Argument"
                                new byte[] { 0xC7, 0x44, 0x24, 0x04 }.Append(Argument.HighToUInt32().GetBytes())
                            };
                            break;
                    }
                    foreach (byte[] Command in Commands)
                        Stream.Write(Command, 0, Command.Length);
                }


                if (Arguments.LongLength % 2 != 0 && Arguments.Length > 4) {
                    Commands = new byte[][] {
                        //sub rsp, 0x8
                        new byte[] { 0x48, 0x81, 0xEC }.Append(0x8.GetBytes())
                    };
                    foreach (byte[] Command in Commands)
                        Stream.Write(Command, 0, Command.Length);
                }

                if (Arguments.Length > 4) {
                    for (int i = 0; i < 4; i++) {
                        StackLen += 8;
                        Commands = new byte[][] {
                            //push 0x00
                            new byte[] { 0x6A, 0x00 }
                        };
                        foreach (byte[] Command in Commands)
                            Stream.Write(Command, 0, Command.Length);
                    }
                }


                Commands = new byte[][] {
                    //xor rax, rax
                    new byte[] { 0x48, 0x31, 0xC0 },
                    //movabs rbx, FuncAddr
                    new byte[] { 0x48, 0xBB }.Append(FuncAddr.ToUInt64().GetBytes()),
                    //call rbx
                    new byte[] { 0xFF, 0xD3 },                    
                    //add rsp, DW
                    new byte[] { 0x48, 0x81, 0xC4 }.Append(StackLen.GetBytes())
                };

                if (CatchReturn)
                    Commands = Commands.Append(new byte[][] {
                        //Call NextInstruction
                        new byte[] { 0xE8, 0x00, 0x00, 0x00, 0x00 },
                        //pop rbx
                        new byte[] { 0x5B },
                        //mov qword [rbx+0x0E], rax
                        new byte[] { 0x48, 0x89, 0x43, 0x0E  },
                        //mov byte  [rbx+0x0D], 1
                        new byte[] { 0xC6, 0x43, 0x0D, 0x01 }
                    });

                Commands = Commands.Append(new byte[][] {
                    //pop rbx
                    new byte[] { 0x5B },
                    //pop rbx
                    new byte[] { 0x5B },
                    //leave
                    new byte[] { 0xC9 },
                    //ret
                    new byte[] { 0xC3 }
                });

                if (CatchReturn)
                    Commands = Commands.Append(new byte[][] {                    
                        //RBX+0xD
                        new byte[] { 0x00 },
                        //RBX+0xE
                        new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
                    });

                foreach (byte[] Command in Commands)
                    Stream.Write(Command, 0, Command.Length);


                IntPtr Function = Target.MAlloc(Stream.ToArray(), true);
                IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, Function, IntPtr.Zero, 0, IntPtr.Zero);

                uint TID = GetThreadId(hThread);
                RemoteThreads.Add(TID);

                while (ResumeThread(hThread) > 0) {
                    GetExitCodeThread(hThread, out uint Status);
                    if (Status != THREAD_STILL_ACTIVE)
                        break;

                    Thread.Sleep(10);
                }

                if (CatchReturn) {
                    IntPtr RetData = Function.Sum(Stream.Length - 9);

                    uint Status = 0;
                    byte[] Buffer = new byte[9];
                    while (GetExitCodeThread(hThread, out Status)) {
                        if (Status != THREAD_STILL_ACTIVE)
                            break;

                        Thread.Sleep(10);
                        Buffer = Target.Read(RetData, (uint)Buffer.LongLength);
                        if (Buffer[0] != 0x1)
                            continue;
                        break;
                    }

                    do {
                        GetExitCodeThread(hThread, out Status);
                    } while (Status == THREAD_STILL_ACTIVE);

                    RemoteThreads.Remove(TID);

                    Target.MFree(Function);

                    return BitConverter.ToUInt64(Buffer, 1).ToIntPtr();
                } else
                    return IntPtr.Zero;
            }
        }  

    }    

}
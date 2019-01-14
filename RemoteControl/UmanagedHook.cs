//#define ILDEBUG
using System;
using System.Runtime.InteropServices;
using System.Reflection.Emit;
using System.Linq;
using System.Reflection;
using static VNX.UnmanagedImports;
using System.Collections.Generic;
using System.Diagnostics;

namespace VNX {

    public class UnmanagedHook : UnmanagedHook<Delegate> {

        /// <summary>
        /// Create a new hook to the function in given pointer
        /// </summary>
        /// <param name="Original">The pointer of the unmanaged function to hook</param>
        /// <param name="Hook">The delegate with the hook function</param>
        public UnmanagedHook(IntPtr Original, Delegate Hook) : base(Original, Hook, false) { }

        /// Create a new Hook
        /// </summary>
        /// <param name="Library">The Library name, Ex: Kernel32.dll</param>
        /// <param name="Function">The Function name, Ex: CreateFile</param>
        /// <param name="Hook">The delegate wit the hook function</param>
        public UnmanagedHook(string Library, string Function, Delegate Hook) : base(Library, Function, Hook, false) {}
        
    }

    public class UnmanagedHook<T> : IDisposable, Hook where T : Delegate {

        static int nBytes = IntPtr.Size == 8 ? 12 : 5;

        IntPtr destination;
        IntPtr addr;
        Protection old;
        byte[] src = new byte[nBytes];
        byte[] dst = new byte[nBytes];
        bool AutoHook = false;

        List<dynamic> Followers = new List<dynamic>();

        T RealDestination;       
        T HookDestination;


        /// <summary>
        /// Create a new hook to the function in given pointer
        /// </summary>
        /// <param name="Original">The pointer of the unmanaged function to hook</param>
        /// <param name="Hook">The delegate with the hook function</param>
        /// <param name="AutoHook">When true the hook will be automatically uninstalled during he execution</param>
        public UnmanagedHook(IntPtr Original, T Hook, bool AutoHook) {
            this.AutoHook = AutoHook;
            RealDestination = Hook;

            if (AutoHook)
                GenerateMethod();

            destination = Marshal.GetFunctionPointerForDelegate(AutoHook ? HookDestination : RealDestination);

            VirtualProtect(Original, nBytes, Protection.PAGE_EXECUTE_READWRITE, out old);
            Marshal.Copy(Original, src, 0, nBytes);
            if (IntPtr.Size == 8) {
                //x64
                new byte[] { 0x48, 0xb8 }.CopyTo(dst, 0);
                BitConverter.GetBytes(unchecked((ulong)destination.ToInt64())).CopyTo(dst, 2);
                new byte[] { 0xFF, 0xE0 }.CopyTo(dst, 10);
            } else {
                //x86
                dst[0] = 0xE9;
                var Result = (int)(destination.ToInt64() - Original.ToInt64() - nBytes);
                var dx = BitConverter.GetBytes(Result);
                Array.Copy(dx, 0, dst, 1, nBytes - 1);
            }
            addr = Original;
        }


        /// <summary>
        /// Create a new hook to the function in given pointer
        /// </summary>
        /// <param name="Original">The pointer of the unmanaged function to hook</param>
        /// <param name="Hook">The delegate with the hook function</param>
        public UnmanagedHook(IntPtr Original, T Hook) : this(Original, Hook, true) { }

        /// Create a new Hook
        /// </summary>
        /// <param name="Library">The Library name, Ex: Kernel32.dll</param>
        /// <param name="Function">The Function name, Ex: CreateFile</param>
        /// <param name="Hook">The delegate wit the hook function</param>
        public UnmanagedHook(string Library, string Function, T Hook) : this(GetProcAddress(LoadLibrary(Library), Function), Hook) { }


        /// Create a new Hook
        /// </summary>
        /// <param name="Library">The Library name, Ex: Kernel32.dll</param>
        /// <param name="Function">The Function name, Ex: CreateFile</param>
        /// <param name="Hook">The delegate wit the hook function</param>
        /// <param name="AutoHook">When true the hook will be automatically uninstalled during he execution</param>
        public UnmanagedHook(string Library, string Function, T Hook, bool AutoHook) : this(GetProcAddress(LoadLibrary(Library), Function), Hook, AutoHook) { }

        void GenerateMethod() {            
            string ID = CurrentID++.ToString();
            SetInstance(ID, this);

            var ParametersInfo = RealDestination.Method.GetParameters();
            var Parameters = (from x in ParametersInfo select x.ParameterType).ToArray();

            List<Instruction> IL = new List<Instruction>();
            //Uninstall(IID);
            IL.Add(new Instruction(OpCodes.Ldstr, ID));
            IL.Add(new Instruction(OpCodes.Call, UninstallMI));

            //Invoke(Parameters);
            switch (Parameters.Length + 1) {
                case 1:
                    IL.Add(new Instruction(OpCodes.Ldc_I4_1));
                    break;
                case 2:
                    IL.Add(new Instruction(OpCodes.Ldc_I4_2));
                    break;
                case 3:
                    IL.Add(new Instruction(OpCodes.Ldc_I4_3));
                    break;
                case 4:
                    IL.Add(new Instruction(OpCodes.Ldc_I4_4));
                    break;
                case 5:
                    IL.Add(new Instruction(OpCodes.Ldc_I4_5));
                    break;
                case 6:
                    IL.Add(new Instruction(OpCodes.Ldc_I4_6));
                    break;
                case 7:
                    IL.Add(new Instruction(OpCodes.Ldc_I4_7));
                    break;
                case 8:
                    IL.Add(new Instruction(OpCodes.Ldc_I4_8));
                    break;
                default:
                    int Count = Parameters.Length + 1;
                    if (Count <= byte.MaxValue)
                        IL.Add(new Instruction(OpCodes.Ldc_I4_S, (byte)Count));
                    else
                        IL.Add(new Instruction(OpCodes.Ldc_I4, Count));
                    break;
            }
            IL.Add(new Instruction(OpCodes.Newarr, typeof(object)));

            for (int i = 0, ind = -1; i < Parameters.Length + 1; i++, ind++) {
                bool IsOut = ind >= 0 && ParametersInfo[ind].IsOut;
                bool IsRef = ind >= 0 && ParametersInfo[ind].ParameterType.IsByRef & !IsOut;
                if (IsOut)
                    continue;

                IL.Add(new Instruction(OpCodes.Dup));
                switch (i) {
                    case 0:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_0));
                        break;
                    case 1:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_1));
                        break;
                    case 2:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_2));
                        break;
                    case 3:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_3));
                        break;
                    case 4:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_4));
                        break;
                    case 5:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_5));
                        break;
                    case 6:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_6));
                        break;
                    case 7:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_7));
                        break;
                    case 8:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_8));
                        break;
                    default:
                        if (i <= byte.MaxValue)
                            IL.Add(new Instruction(OpCodes.Ldc_I4_S, (byte)i));
                        else
                            IL.Add(new Instruction(OpCodes.Ldc_I4, i));                        
                        break;
                }


                if (ind >= 0 && (ParametersInfo[ind].IsIn || ParametersInfo[ind].IsOut)) {
                    if (ind <= byte.MaxValue)
                        IL.Add(new Instruction(OpCodes.Ldarga_S, (byte)ind));
                    else
                        IL.Add(new Instruction(OpCodes.Ldarga, (short)ind));
                } else {
                    switch (i) {
                        case 0:
                            IL.Add(new Instruction(OpCodes.Ldstr, ID));
                            break;
                        case 1:
                            IL.Add(new Instruction(OpCodes.Ldarg_0));
                            break;
                        case 2:
                            IL.Add(new Instruction(OpCodes.Ldarg_1));
                            break;
                        case 3:
                            IL.Add(new Instruction(OpCodes.Ldarg_2));
                            break;
                        case 4:
                            IL.Add(new Instruction(OpCodes.Ldarg_3));
                            break;
                        default:
                            if (ind <= byte.MaxValue)
                                IL.Add(new Instruction(OpCodes.Ldarg_S, (byte)ind));
                            else
                                IL.Add(new Instruction(OpCodes.Ldarg, (short)ind));
                            break;
                    }
                }

                var Type = ind >= 0 ? Parameters[ind] : null;
                if (Type != null && IsRef)
                      Type = Parameters[ind].GetElementType();                

                bool Cast = true;
                if (IsRef) {
                    var PType = Type;
                    if (PType.IsEnum && PType.IsValueType)
                        PType = Enum.GetUnderlyingType(PType);

                    switch (PType.FullName) {
                        case "System.SByte":
                            IL.Add(new Instruction(OpCodes.Ldind_I1));
                            break;
                        case "System.Byte":
                            IL.Add(new Instruction(OpCodes.Ldind_U1));
                            break;
                        case "System.Int16":
                            IL.Add(new Instruction(OpCodes.Ldind_I2));
                            break;
                        case "System.Char":
                        case "System.UInt16":
                            IL.Add(new Instruction(OpCodes.Ldind_U2));
                            break;
                        case "System.Int32":
                            IL.Add(new Instruction(OpCodes.Ldind_I4));
                            break;
                        case "System.UInt32":
                            IL.Add(new Instruction(OpCodes.Ldind_U4));
                            break;
                        case "System.Int64":
                        case "System.UInt64":
                            IL.Add(new Instruction(OpCodes.Ldind_I8));
                            break;
                        case "System.Single":
                            IL.Add(new Instruction(OpCodes.Ldind_R4));
                            break;
                        case "System.Double":
                            IL.Add(new Instruction(OpCodes.Ldind_R8));
                            break;
                        case "System.IntPtr":
                        case "System.UIntPtr":
                            IL.Add(new Instruction(OpCodes.Ldind_I));
                            break;
                        default:
                            if ((PType.IsValueType && !PType.IsEnum) || PType.IsClass) {
                                IL.Add(new Instruction(OpCodes.Ldobj, PType));
                            } else {
                                IL.Add(new Instruction(OpCodes.Ldind_Ref));
                                Cast = false;
                            }
                            break;
                    }
                }

                if (ind >= 0 && Cast )
                    IL.Add(new Instruction(OpCodes.Box, Type));
                
                IL.Add(new Instruction(OpCodes.Stelem_Ref));
            }

            IL.Add(new Instruction(OpCodes.Stloc_0));
            IL.Add(new Instruction(OpCodes.Ldloc_0));
            IL.Add(new Instruction(OpCodes.Call, InvokeMI));

            //Cast Return Type
            var RetType = RealDestination.Method.ReturnType;
            if (RetType.IsInterface)
                IL.Add(new Instruction(OpCodes.Castclass, RetType));
            else { 
                IL.Add(new Instruction(OpCodes.Unbox_Any, RetType));
            }

            for (int i = 0, ind = -1; i < Parameters.Length + 1; i++, ind++) {
                bool IsOut = ind >= 0 && ParametersInfo[ind].IsOut;
                bool IsRef = ind >= 0 && ParametersInfo[ind].ParameterType.IsByRef & !IsOut;

                if (!IsRef && !IsOut || ind < 0)
                    continue;


                switch (ind) {
                    case 0:
                        IL.Add(new Instruction(OpCodes.Ldarg_0));
                        break;
                    case 1:
                        IL.Add(new Instruction(OpCodes.Ldarg_1));
                        break;
                    case 2:
                        IL.Add(new Instruction(OpCodes.Ldarg_2));
                        break;
                    case 3:
                        IL.Add(new Instruction(OpCodes.Ldarg_3));
                        break;
                    default:

                        if (ind <= byte.MaxValue)
                            IL.Add(new Instruction(OpCodes.Ldarg_S, (byte)ind));
                        else
                            IL.Add(new Instruction(OpCodes.Ldarg, (short)ind));
                        break;
                }

                IL.Add(new Instruction(OpCodes.Ldloc_0));
                switch (i) {
                    case 0:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_0));
                        break;
                    case 1:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_1));
                        break;
                    case 2:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_2));
                        break;
                    case 3:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_3));
                        break;
                    case 4:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_4));
                        break;
                    case 5:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_5));
                        break;
                    case 6:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_6));
                        break;
                    case 7:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_7));
                        break;
                    case 8:
                        IL.Add(new Instruction(OpCodes.Ldc_I4_8));
                        break;
                    default:
                        if (ind <= byte.MaxValue)
                            IL.Add(new Instruction(OpCodes.Ldc_I4_S, (byte)ind));
                        else
                            IL.Add(new Instruction(OpCodes.Ldc_I4, ind));
                        break;
                }

                IL.Add(new Instruction(OpCodes.Ldelem_Ref));

                var Type = Parameters[ind];
                if (IsRef || IsOut)
                    Type = Type.GetElementType();

                if (Type.IsInterface) {
                    IL.Add(new Instruction(OpCodes.Castclass, Type));
                    IL.Add(new Instruction(OpCodes.Stind_Ref));
                } else {                 
                    IL.Add(new Instruction(OpCodes.Unbox_Any, Type));

                    if (Type.IsEnum && Type.IsValueType)
                        Type = Enum.GetUnderlyingType(Type);

                    switch (Type.FullName) {
                        case "System.Byte":
                        case "System.SByte":
                            IL.Add(new Instruction(OpCodes.Stind_I1));
                            break;
                        case "System.Char":
                        case "System.UInt16":
                        case "System.Int16":
                            IL.Add(new Instruction(OpCodes.Stind_I2));
                            break;
                        case "System.UInt32":
                        case "System.Int32":
                            IL.Add(new Instruction(OpCodes.Stind_I4));
                            break;
                        case "System.Int64":
                        case "System.UInt64":
                            IL.Add(new Instruction(OpCodes.Stind_I8));
                            break;
                        case "System.Single":
                            IL.Add(new Instruction(OpCodes.Stind_R4));
                            break;
                        case "System.Double":
                            IL.Add(new Instruction(OpCodes.Stind_R8));
                            break;
                        case "System.IntPtr":
                        case "System.UIntPtr":
                            IL.Add(new Instruction(OpCodes.Stind_I));
                            break;
                        default:
                            if (Type.IsValueType && !Type.IsEnum) {
                                IL.Add(new Instruction(OpCodes.Stobj, Type));
                            } else {
                                IL.Add(new Instruction(OpCodes.Stind_Ref));
                            }
                            break;
                    }
                }

            }


            //Install(IID);
            IL.Add(new Instruction(OpCodes.Ldstr, ID));
            IL.Add(new Instruction(OpCodes.Call, InstallMI));

            //return;
            IL.Add(new Instruction(OpCodes.Ret));

#if ILDEBUG
            var NewMethod = GenerateAssembly(ID, IL, ParametersInfo);
            HookDestination = (T)Delegate.CreateDelegate(typeof(T), NewMethod);
#else
            DynamicMethod Method = new DynamicMethod("UnmanagedHook", RealDestination.Method.ReturnType, Parameters, typeof(UnmanagedImports), true);

            

            var ILGen = Method.GetILGenerator();
            ILGen.DeclareLocal(typeof(object[]));

            foreach (var Pair in IL) {
                if (Pair.Value == null)
                    ILGen.Emit(Pair.Key);
                else
                    ILGen.Emit(Pair.Key, Pair.Value);
            }

            HookDestination = (T)Method.CreateDelegate(typeof(T));
#endif

        }
        

#if ILDEBUG
        private MethodInfo GenerateAssembly(string ID, List<Instruction> IL, ParameterInfo[] Info) {
            var AsmName = new AssemblyName("UnmanagedHook." + ID);
            var AsmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(AsmName, AssemblyBuilderAccess.RunAndSave);
            var ModBuilder = AsmBuilder.DefineDynamicModule(AsmName.Name, AsmName.Name + ".dll");

            TypeBuilder builder = ModBuilder.DefineType("HookClass", TypeAttributes.Public);
            var Attributes = MethodAttributes.Static | MethodAttributes.Public | MethodAttributes.HideBySig;
            var MethBuilder = builder.DefineMethod("HookMethod", Attributes, RealDestination.Method.ReturnType, (from x in Info select x.ParameterType).ToArray());


            string Result = string.Empty;
            var ILGen = MethBuilder.GetILGenerator();
            ILGen.DeclareLocal(typeof(object[]));
            foreach (var Pair in IL) {
                try {
                    if (Pair.Value == null)
                        ILGen.Emit(Pair.Key);
                    else
                        ILGen.Emit(Pair.Key, Pair.Value);

                    Result += Pair.Value == null ? $"{Pair.Key.ToString()}\n" : $"{Pair.Key.ToString()}\t\t{Pair.Value.ToString()}\n";

                } catch (Exception ex) {
                    if (System.Diagnostics.Debugger.IsAttached)
                        System.Diagnostics.Debugger.Break();
                    else
                        throw ex;
                }
            }

            var Class = builder.CreateType();


            AsmBuilder.Save(AsmName.Name + ".dll");


            return Class.GetMethod("HookMethod", BindingFlags.Static | BindingFlags.Public);
        }
#endif
        MethodInfo InstallMI => (from x in typeof(UnmanagedImports).GetMethods(BindingFlags.Static | BindingFlags.Public) where x.Name == "Install" select x).First();
        MethodInfo UninstallMI => (from x in typeof(UnmanagedImports).GetMethods(BindingFlags.Static | BindingFlags.Public) where x.Name == "Uninstall" select x).First();
        MethodInfo InvokeMI => (from x in typeof(UnmanagedImports).GetMethods(BindingFlags.Static | BindingFlags.Public) where x.Name == "Invoke" select x).First();

        /// <summary>
        /// Install the hook
        /// </summary>
        public void Install() {
            Marshal.Copy(dst, 0, addr, nBytes);
        }


        /// <summary>
        /// Uninstall the hook
        /// </summary>
        public void Uninstall() {
            Marshal.Copy(src, 0, addr, nBytes);
        }

        Delegate Hook.GetDelegate() {
            return RealDestination;
        }
        
        /// <summary>
        /// Adds a hook to be disabled during the execution of the this hook
        /// </summary>
        /// <param name="Follower">The hook to be disabled</param>
        public void AddFollower(params object[] Followers) {
            if (!AutoHook)
                throw new Exception("The Auto Hook must be enabled");

            foreach (object Follower in Followers) {
                if (!(Follower is Hook))
                    throw new Exception(Follower.GetType().Name + " Isn't an UnmanagedHook Class");

                this.Followers.Add(Follower);
            }
        }

        /// <summary>
        /// Remove a hook from the Follower list
        /// </summary>
        /// <param name="Follower">The hook to be removed</param>
        public void RemoveFollower(params object[] Followers) {
            if (!AutoHook)
                throw new Exception("The Auto Hook must be enabled");

            foreach (object Follower in Followers) {
                if (!(Follower is Hook))
                    throw new Exception(Follower.GetType().Name + " Isn't an UnmanagedHook Class");

                this.Followers.Remove(Follower);
            }
        }

        object[] Hook.GetFollowers() {
            return Followers.ToArray();
        }

        public void Dispose() {
            Uninstall();
            Protection x;
            VirtualProtect(addr, nBytes, old, out x);
        }

    }

#if ILDEBUG
    public static class UnmanagedImports {
#else
    static class UnmanagedImports { 
#endif
        [DebuggerDisplay("{Key}      {Value}")]
        internal struct Instruction {
            public dynamic Key;
            public dynamic Value;

            public Instruction(dynamic Key, dynamic Value) {
                this.Key = Key;
                this.Value = Value;
            }
            public Instruction(dynamic Key) {
                this.Key = Key;
                Value = null;
            }
        }

        internal interface Hook {
            void Install();
            void Uninstall();

            dynamic[] GetFollowers();

            Delegate GetDelegate();
        }


        static Dictionary<string, object> InstanceMap = new Dictionary<string, object>();

        internal static long CurrentID = 0;

        /// <summary>
        /// INTERNAL UNMANAGED HOOK USAGE, DON'T TOUCH ME
        /// </summary>
        public static void Install(string ID) {
            Hook Hook = (Hook)InstanceMap[ID];
            Hook.Install();

            foreach (object dFollower in Hook.GetFollowers()) {
                Hook Follower = (Hook)dFollower;
                Follower.Install();
            }
        }

        /// <summary>
        /// INTERNAL UNMANAGED HOOK USAGE, DON'T TOUCH ME
        /// </summary>
        public static void Uninstall(string ID) {
            Hook Hook = (Hook)InstanceMap[ID];
            Hook.Uninstall();

            foreach (object dFollower in Hook.GetFollowers()) {
                Hook Follower = (Hook)dFollower;
                Follower.Uninstall();
            }
        }

        /// <summary>
        /// INTERNAL UNMANAGED HOOK USAGE, DON'T TOUCH ME
        /// </summary>
        public static object Invoke(object[] Parameters) {
            if (Parameters.Length == 0)
                throw new Exception("No Arguments Found");

            string ID = (string)Parameters.First();
            object[] Args = Parameters.Skip(1).ToArray();

            Hook Hook = (Hook)InstanceMap[ID];

            return Hook.GetDelegate().DynamicInvoke(Args);
        }


        internal static void SetInstance(string ID, object Instance) {
            InstanceMap[ID] = Instance;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool VirtualProtect(IntPtr lpAddress, int dwSize, Protection flNewProtect, out Protection lpflOldProtect);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        internal static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);
        internal enum Protection {
            PAGE_NOACCESS = 0x01,
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            PAGE_WRITECOPY = 0x08,
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_EXECUTE_WRITECOPY = 0x80,
            PAGE_GUARD = 0x100,
            PAGE_NOCACHE = 0x200,
            PAGE_WRITECOMBINE = 0x400
        }
    }
}
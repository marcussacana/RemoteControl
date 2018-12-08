using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using static VNX.Imports;
using static VNX.Tools;

namespace VNX {
    internal static class InternalExtensions {
        public static ulong ToUInt64(this IntPtr Ptr) => unchecked((ulong)Ptr.ToInt64());
        public static uint ToUInt32(this IntPtr Ptr) => unchecked((uint)(Ptr.ToInt64() & 0xFFFFFFFF));
        public static uint HighToUInt32(this IntPtr Ptr) => unchecked((uint)(Ptr.ToInt64() >> 32));

        public static ulong ToUInt64(this MIntPtr Ptr) => unchecked((ulong)((IntPtr)Ptr).ToInt64());
        public static uint ToUInt32(this MIntPtr Ptr) => unchecked((uint)(((IntPtr)Ptr).ToInt64() & 0xFFFFFFFF));
        public static uint HighToUInt32(this MIntPtr Ptr) => unchecked((uint)((IntPtr)Ptr) >> 32);

        public static IntPtr ToIntPtr(this uint Int) => new IntPtr(unchecked((int)Int));
        public static IntPtr ToIntPtr(this ulong Int) => new IntPtr(unchecked((long)Int));

        public static IntPtr Sum(this IntPtr Pointer, ulong Value) => (Pointer.ToUInt64() + Value).ToIntPtr();
        public static IntPtr Sum(this IntPtr Pointer, IntPtr Value) => (Pointer.ToUInt64() + Value.ToUInt64()).ToIntPtr();
        public static IntPtr Sum(this IntPtr Pointer, long Value) => (Pointer.ToUInt64() + (ulong)Value).ToIntPtr();
        public static MIntPtr Sum(this MIntPtr Pointer, ulong Value) => (Pointer.ToUInt64() + Value).ToIntPtr();
        public static MIntPtr Sum(this MIntPtr Pointer, long Value) => (Pointer.ToUInt64() + (ulong)Value).ToIntPtr();

        public static byte[] GetBytes(this uint Value) => BitConverter.GetBytes(Value);
        public static byte[] GetBytes(this int Value) => BitConverter.GetBytes(Value);
        public static byte[] GetBytes(this ulong Value) => BitConverter.GetBytes(Value);
        public static byte[] GetBytes(this long Value) => BitConverter.GetBytes(Value);

        public static uint ToUInt32(this byte[] Data, int Address = 0) => BitConverter.ToUInt32(Data, Address);
        public static int ToInt32(this byte[] Data, int Address = 0) => BitConverter.ToInt32(Data, Address);
        public static ulong ToUInt64(this byte[] Data, int Address = 0) => BitConverter.ToUInt64(Data, Address);
        public static long ToInt64(this byte[] Data, int Address = 0) => BitConverter.ToInt64(Data, Address);

        public static uint SizeOf<T>(this T Struct) where T : struct => (uint)Marshal.SizeOf(typeof(T));

        public static IntPtr ToIntPtr(this byte[] Data, bool? x64 = null) {
            if (x64.HasValue)
                return new IntPtr(x64.Value ? Data.ToInt64() : Data.ToInt32());
            if (Data.Length >= 8)
                return new IntPtr(IntPtr.Size == 8 ? Data.ToInt64() : Data.ToInt32());
            return new IntPtr(Data.ToInt32());
        }

        public static void Install(this LockInfo Locker) => Locker.Target.Write(Locker.Address, Locker.Locker);
        public static void Uninstall(this LockInfo Locker) => Locker.Target.Write(Locker.Address, Locker.Unlocker);

        public static bool IsInstalled(this LockInfo Locker) {
            byte[] Memory = Locker.Target.Read(Locker.Address, (uint)Locker.Locker.Length);
            bool Locked = true;
            for (int i = 0; i < Locker.Locker.Length; i++)
                if (Memory[i] != Locker.Locker[i])
                    Locked = false;

            return Locked;
        }

        public static T[] Append<T>(this T[] Ori, T[] New) => Ori.Concat(New).ToArray();

    }

    public static class Extensions {
        /// <summary>
        /// Create an Suspended Process
        /// <para>Only the executale path and arguments will be used</para>
        /// </summary>
        /// <param name="StartInfo">The StartInfo to create the process</param>
        /// <returns>The Created Process</returns>
        public static Process StartSuspended(this ProcessStartInfo StartInfo) => Process.GetProcessById((int)CreateProcessSuspended(StartInfo.FileName, StartInfo.Arguments).dwProcessId);

        /// <summary>
        /// Create an process
        /// </summary>
        /// <param name="StartInfo">The StartInfo to create the process</param>
        /// <returns>The Created Process</returns>
        public static Process Start(this ProcessStartInfo StartInfo) => Process.Start(StartInfo);

        /// <summary>
        /// Checks if the process is a x64 Process
        /// </summary>
        /// <param name="Process">The process to verify</param>
        /// <returns>If true, then is a x64 Process, if false, is a x86 Process</returns>
        public static bool Is64Bits(this Process Process) => Is64Bit(Process);

        /// <summary>
        /// Checks whether the process is an x86 module being emulated within an x64 architecture
        /// </summary>
        /// <param name="Process">The process to verify</param>
        /// <returns>If true, then the process is being emulated, if false, it's a native process</returns>
        public static bool IsWOW64(this Process Process)  => IsWow64(Process);


        /// <summary>
        /// Enumerate all modules in the target process
        /// <para>Supports WOW64 Process's</para>
        /// </summary>
        /// <param name="Process">The process to get the modules</param>
        /// <returns>Module Name Array</returns>
        public static string[] GetAllModulesNames(this Process Process) => Tools.GetAllModulesName(Process);


        /// <summary>
        /// Enumerate all modules in the target process
        /// <para>Supports WOW64 Process's</para>
        /// </summary>
        /// <param name="Process">The process to get the modules</param>
        /// <returns>Module Name Array</returns>
        public static IntPtr[] GetAllModules(this Process Process) => Tools.GetAllModules(Process);

        /// <summary>
        /// Get the module handler from his name
        /// <para>Supports WOW64 Process's</para>
        /// </summary>
        /// <param name="Process">The Process to search for the module</param>
        /// <param name="Name">The Module Name</param>
        /// <returns>The Module Handler</returns>
        public static IntPtr GetModuleByName(this Process Process, string Name) => Tools.GetModuleByName(Process, Name);

        /// <summary>
        /// Get the module handler from his name
        /// <para>Supports WOW64 Process's</para>
        /// </summary>
        /// <param name="Process">The Process to search for the module</param>
        /// <param name="Handler">The Module Handler</param>
        /// <returns>The Module Name</returns>
        public static string GetModuleNameByHandler(this Process Process, IntPtr Handler) => Tools.GetModuleNameByHandler(Process, Handler);

        /// <summary>
        /// Get the main module of an specified process
        /// <para>Supports WOW64 Process's</para>
        /// </summary>
        /// <param name="Process">The Process to Catch the Module</param>
        /// <returns>The Module Handler</returns>
        public static IntPtr GetMainModule(this Process Process) => Tools.GetMainModule(Process);


        /// <summary>
        /// Alloc an null-terminated <paramref name="String"/> in the target process memory
        /// </summary>
        /// <param name="Process">The Process to alloc the data</param>
        /// <param name="String">The string to alloc</param>
        /// <param name="Unicode">Switch bettewen Unicode or ANSI encoding</param>
        /// <returns>The pointer to the string</returns>
        public static IntPtr AllocString(this Process Process, string String, bool Unicode) => Tools.AllocString(Process.Handle, String, Unicode);


        /// <summary>
        /// Read a null-terminated string in the target Process
        /// </summary>
        /// <param name="Process">The process to read the memory</param>
        /// <param name="Address">The Address of the string to Read</param>
        /// <param name="Unicode">Determine if is an Unicode or ANSI string</param>
        /// <returns></returns>
        public static string ReadString(this Process Process, IntPtr Address, bool Unicode) => Tools.ReadString(Process.Handle, Address, Unicode);

        /// <summary>
        /// Alloc the <paramref name="Content"/> to the target process
        /// </summary>
        /// <param name="Content">The content to alloc</param>
        /// <param name="Executable">Make this allocated content executable</param>
        /// <returns>The pointer to the allocated content</returns>
        public static IntPtr Alloc(this Process Process, byte[] Content, bool Executable = false) => Tools.Alloc(Process.Handle, Content, Executable);
        

        /// <summary>
        /// Write bytes in the target process memory
        /// </summary>
        /// <param name="Process">The Process to write the data</param>
        /// <param name="Address">Address to write</param>
        /// <param name="Content">Content to write</param>
        /// <returns>if writed with sucess, will return true</returns>
        public static bool Write(this Process Process, IntPtr Address, byte[] Content) => Tools.Write(Process.Handle, Address, Content);


        /// <summary>
        /// Read bytes in the target process memory
        /// </summary>
        /// <param name="Address">Address to read</param>
        /// <param name="Length">Length in bytes to read</param>
        /// <returns>The readed content</returns>
        public static byte[] Read(this Process Process, IntPtr Address, uint Length) => Tools.Read(Process.Handle, Address, Length);



        /// <summary>
        /// Free a allocated region of the memory or page
        /// </summary>v
        /// <param name="Process">The process to release the memory</param>
        /// <param name="Address">The address to be released</param>
        /// <param name="Length">The length to be released</param>
        /// <returns>Return true if sucess</returns>
        public static bool Free(this Process Process, IntPtr Address, uint Length = 0) => Tools.Free(Process.Handle, Address, Length);


        /// <summary>
        /// Change memory protection for a given range
        /// </summary>
        /// <param name="Address">The Begin Range Address</param>
        /// <param name="Range">The Range Length</param>
        /// <param name="Protection">The Protection to be applied in the range</param>
        /// <param name="OriginalProtection">The Original Range Protection</param>
        /// <returns>If protection has changed, returns true, if failed, returns false</returns>
        public static void ChangeProtection(this Process Process, IntPtr Address, uint Range, ProtectionType Protection, out ProtectionType OriginalProtection) =>
            Tools.ChangeProtection(Process.Handle, Address, Range, Protection, out OriginalProtection);

        /// <summary>
        /// Change memory protection for a given range
        /// </summary>
        /// <param name="Address">The Begin Range Address</param>
        /// <param name="Range">The Range Length</param>
        /// <param name="Protection">The Protection to be applied in the range</param>
        /// <param name="OriginalProtection">The Original Range Protection</param>
        /// <returns>If protection has changed, returns true, if failed, returns false</returns>
        public static void ChangeProtection(this Process Process, IntPtr Address, uint Range, ProtectionType Protection) =>
            Tools.ChangeProtection(Process.Handle, Address, Range, Protection);

        /// <summary>
        /// Get the entry point in the memory of an specified module
        /// </summary>
        /// <param name="Process">The process of the target module</param>
        /// <param name="Module">The Module Handler</param>
        /// <returns>The Pointer to the Module EntryPoint</returns>
        public static IntPtr GetModuleEntryPoint(this Process Process, IntPtr Module) => Tools.GetModuleEntryPoint(Process, Module);


        /// <summary>
        /// Parse a MIntPtr to your generic type
        /// </summary>
        /// <typeparam name="T">The MIntPtr Type</typeparam>
        /// <param name="Pointer">The Pointer to Parse</param>
        /// <returns>The Converted Type</returns>
        public static T Parse<T>(this MIntPtr<T> Pointer) => Pointer;

        /// <summary>
        /// Checks if an library is already loaded in the target process
        /// </summary>
        /// <param name="Process">The process to verify</param>
        /// <param name="Library">Library name</param>
        /// <returns></returns>
        public static bool LibraryLoaded(this Process Process, string Library) => Tools.LibraryLoaded(Process, Library);

    }
}

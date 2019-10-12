﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using static VNX.Imports;

namespace VNX {
    internal static class Tools {
        public static PROCESS_INFORMATION CreateProcessSuspended(string Filename, string Arguments, string CurrentDirectory) {
            STARTUPINFO si = new STARTUPINFO();
            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();
            bool success = CreateProcess(Arguments != null ? null : Filename, Arguments != null ? $"\"{Filename}\" {Arguments}" : null, IntPtr.Zero, IntPtr.Zero, false, ProcessCreationFlags.CREATE_SUSPENDED | ProcessCreationFlags.DEBUG_ONLY_THIS_PROCESS, IntPtr.Zero, CurrentDirectory, ref si, out pi);
            if (!success)
                throw new Exception("Failed to create the process");

            return pi;
        }


        /// <summary>
        /// Checks if an library is already loaded in the target process
        /// </summary>
        /// <param name="Library">Library name</param>
        /// <returns></returns>
        public static bool LibraryLoaded(Process Target, string Library) {
            try {
                string Module = Path.GetFileName(Library).ToLower();
                string[] Modules = Target.GetAllModulesNames();
                var Rst = (from x in Modules where x.ToLower() == Module select x).ToArray();

                return Rst.Length != 0;
            } catch {
                return false;
            }
        }

        public static bool Is64Bit(Process Proc) {
            try {
                if (IsWow64Process(Proc.Handle, out bool WOW64)) {
                    if (Environment.Is64BitOperatingSystem && !WOW64)
                        return true;
                    else
                        return false;
                }
            } catch { }

            return Is64Bit(Proc.MainModule.FileName);
        }
        public static bool IsWow64(Process Target) {
            try {
                if (IsWow64Process(Target.Handle, out bool WOW64)) {
                    if (Environment.Is64BitOperatingSystem && WOW64)
                        return true;
                }
            } catch { }
            return false;
        }
        public static bool Is64Bit(string FilePath) {
            switch (GetMachineType(FilePath)) {
                case MachineType.IMAGE_FILE_MACHINE_AMD64:
                case MachineType.IMAGE_FILE_MACHINE_IA64:
                    return true;
                case MachineType.IMAGE_FILE_MACHINE_I386:
                    return false;
                default:
                    throw new NotImplementedException();
            }
        }
        public static MachineType GetMachineType(string FilePath) {
            FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            fs.Seek(0x3c, SeekOrigin.Begin);
            int peOffset = br.ReadInt32();
            fs.Seek(peOffset, SeekOrigin.Begin);
            uint peHead = br.ReadUInt32();

            if (peHead != 0x00004550) // "PE\0\0", little-endian
                throw new Exception("Can't find PE header");

            MachineType machineType = (MachineType)br.ReadUInt16();
            br.Close();
            fs.Close();
            return machineType;
        }

        public static string DotNetVerName(FrameworkVersion Version) {
            switch (Version) {
                case FrameworkVersion.DotNet10:
                    return "v1.0.3705";
                case FrameworkVersion.DotNet11:
                    return "v1.1.4322";
                case FrameworkVersion.DotNet20:
                    return "v2.0.50727";
                case FrameworkVersion.DotNet30:
                    return "v3.0";
                case FrameworkVersion.DotNet35:
                    return "v3.5";
                case FrameworkVersion.Lastest:
                    return null;
                default:
                    return "v4.0.30319";
            }
        }

        public static string[] GetAllModulesName(Process Target) {
            IntPtr[] Modules = GetAllModules(Target);
            List<string> ModulesName = new List<string>();
            foreach (var hModule in Modules) {
                StringBuilder SB = new StringBuilder(1024);
                GetModuleFileNameEx(Target.Handle, hModule, SB, 1024);
                ModulesName.Add(Path.GetFileName(SB.ToString()));
            }

            return ModulesName.ToArray();
        }

        public static IntPtr[] GetAllModules(Process Target) {
            Target.Refresh();
            if (!Target.IsWOW64())
                return (from x in Target.Modules.Cast<ProcessModule>() select x.BaseAddress).ToArray();

            IntPtr[] Ptrs = new IntPtr[0];
            while (true) {
                IntPtr pArr = Marshal.AllocHGlobal(IntPtr.Size * Ptrs.Length);
                EnumProcessModulesEx(Target.Handle, pArr, (uint)(IntPtr.Size * Ptrs.LongLength), out uint Required, DwFilterFlag.LIST_MODULES_ALL);
                if (Required / IntPtr.Size > Ptrs.Length) {
                    Ptrs = new IntPtr[Required / IntPtr.Size];
                    Marshal.FreeHGlobal(pArr);
                    continue;
                }
                Marshal.Copy(pArr, Ptrs, 0, Ptrs.Length);
                Marshal.FreeHGlobal(pArr);
                break;
            }

            return Ptrs;
        }

        public static bool IsManaged(string File) {
            if (!System.IO.File.Exists(File))
                return false;
            try {
                AssemblyName Name = AssemblyName.GetAssemblyName(File);
                return true;
            } catch (BadImageFormatException Ex){
                return false;
            } catch (Exception Ex){
                throw Ex;
            }
        }


        public static IntPtr GetModuleByName(Process Target, string Name) {
            IntPtr[] Modules = GetAllModules(Target);
            List<string> ModulesName = new List<string>();
            foreach (var hModule in Modules) {
                StringBuilder SB = new StringBuilder(1024);
                GetModuleFileNameEx(Target.Handle, hModule, SB, 1024);
                string CName = Path.GetFileName(SB.ToString());
                if (CName.ToLower() == Name.ToLower())
                    return hModule;
            }

            throw new KeyNotFoundException();
        }

        public static string GetModuleNameByHandler(Process Target, IntPtr Handler) {
            IntPtr[] Modules = GetAllModules(Target);
            List<string> ModulesName = new List<string>();
            foreach (var hModule in Modules) {
                if (Handler != hModule)
                    continue;
                StringBuilder SB = new StringBuilder(1024);
                GetModuleFileNameEx(Target.Handle, hModule, SB, 1024);
                string CName = Path.GetFileName(SB.ToString());
                return CName;
            }

            throw new KeyNotFoundException();
        }

        public static IntPtr GetMainModule(Process Target) {
            IntPtr[] Modules = GetAllModules(Target);
            List<string> ModulesName = new List<string>();
            foreach (var hModule in Modules) {
                StringBuilder SB = new StringBuilder(1024);
                GetModuleFileNameEx(Target.Handle, hModule, SB, 1024);
                string CName = Path.GetFileNameWithoutExtension(SB.ToString());
                if (CName.ToLower() == Target.ProcessName.ToLower())
                    return hModule;
            }

            throw new EntryPointNotFoundException();
        }

        public static IntPtr GetModuleEntryPoint(Process Target, IntPtr Module)
        {
            ulong PEStart = Read(Target.Handle, Module.Sum(0x3C), 4).ToUInt32();
            ulong OptionalHeader = PEStart + 0x18;
            IntPtr EntryPointPtr = (OptionalHeader + 0x10).ToIntPtr();

            byte[] EP = Read(Target.Handle, Module.Sum(EntryPointPtr), 4);
            return Module.Sum(EP.ToUInt32());
        }

        public static ImportEntry[] GetModuleImports(Process Target, IntPtr Module)
        {
            uint PtrSize = Target.Is64Bits() ? 8u : 4u;

            ulong OrdinalFlag = (1ul << (int)((8 * PtrSize) - 1));

            IntPtr Handle = Target.Handle;
            ulong PEStart = Read(Handle, Module.Sum(0x3C), 4).ToUInt32();
            ulong OptionalHeader = PEStart + 0x18;

            ulong ImageDataDirectoryPtr = OptionalHeader + (PtrSize == 8 ? 0x70u : 0x60u);

            ulong ImportTableEntry = ImageDataDirectoryPtr + 0x8;

            IntPtr RVA = ImportTableEntry.ToIntPtr();

            IntPtr ImportDesc = Module.Sum(Read(Handle, Module.Sum(RVA), 4).ToUInt32());

            if (ImportDesc == Module)
                return new ImportEntry[0];

            List<ImportEntry> Entries = new List<ImportEntry>();

            while (true)
            {
                uint OriginalFirstThunk = Read(Handle, ImportDesc.Sum(4 * 0), 4).ToUInt32();
                //uint TimeDateStamp  =     Read(Handle, ImportDesc.Sum(4 * 1), 4).ToUInt32();
                //uint ForwarderChain =     Read(Handle, ImportDesc.Sum(4 * 2), 4).ToUInt32();
                uint Name =               Read(Handle, ImportDesc.Sum(4 * 3), 4).ToUInt32();
                uint FirstThunk =         Read(Handle, ImportDesc.Sum(4 * 4), 4).ToUInt32();

                if (OriginalFirstThunk == 0x00)
                    break;

                string ModuleName = ReadString(Handle, Module.Sum(Name), false);

                IntPtr DataAddr = Module.Sum(OriginalFirstThunk);
                IntPtr IATAddr  = Module.Sum(FirstThunk);
                while (true)
                {
                    IntPtr EntryPtr = Read(Handle, DataAddr, PtrSize).ToIntPtr();

                    if (EntryPtr == IntPtr.Zero)
                        break;

                    bool ImportByOrdinal = false;
                    if ((EntryPtr.ToUInt64() & OrdinalFlag) == OrdinalFlag)
                    {
                        EntryPtr = ((PtrSize == 8 ? EntryPtr.ToUInt64() : EntryPtr.ToUInt32()) ^ OrdinalFlag).ToIntPtr();
                        ImportByOrdinal = true;
                    }
                    else
                        EntryPtr = Module.Sum(EntryPtr);

                    ushort Hint = ImportByOrdinal ? (ushort)EntryPtr.ToUInt32() : Read(Handle, EntryPtr, 2).ToUInt16();
                    string ExportName = ImportByOrdinal ? null : ReadString(Handle, EntryPtr.Sum(2), false);

                    Entries.Add(new ImportEntry() {
                        Function = ExportName,
                        Ordinal = Hint,
                        Module = ModuleName,
                        ImportAddress = IATAddr,
                        FunctionAddress = Read(Handle, IATAddr, PtrSize).ToIntPtr()
                    });

                    DataAddr = DataAddr.Sum(PtrSize);
                    IATAddr = IATAddr.Sum(PtrSize);
                }


                ImportDesc = ImportDesc.Sum(0x14);//sizeof(_IMAGE_IMPORT_DESCRIPTOR)
            }

            return Entries.ToArray();
        }

        public static IntPtr AllocString(IntPtr hProcess, string String, bool Unicode) {
            if (String == null)
                return IntPtr.Zero;

            var Enco = Unicode ? Encoding.Unicode : Encoding.Default;
            return Alloc(hProcess, Enco.GetBytes(String + "\x0"));
        }

        public static IntPtr MAllocString(Process Target, string String, bool Unicode) {
            if (String == null)
                return IntPtr.Zero;

            var Enco = Unicode ? Encoding.Unicode : Encoding.Default;
            return MemoryManagement.MAlloc(Target, Enco.GetBytes(String + "\x0"));
        }

        public static IntPtr Alloc(IntPtr hProcess, byte[] Content, bool Executable = false) {
            IntPtr Ptr = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)Content.LongLength, AllocationType.Commit | AllocationType.Reserve, Executable ? ProtectionType.PAGE_EXECUTE_READWRITE : ProtectionType.PAGE_READWRITE);
            if (!Write(hProcess, Ptr, Content))
                throw new Exception("Failed to Write in the target process memory");
            return Ptr;
        }

        public static IntPtr Alloc(IntPtr hProcess, uint Size, bool Executable = false) {
            return VirtualAllocEx(hProcess, IntPtr.Zero, Size, AllocationType.Commit | AllocationType.Reserve, Executable ? ProtectionType.PAGE_EXECUTE_READWRITE : ProtectionType.PAGE_READWRITE);
        }

        public static bool Write(IntPtr hProcess, IntPtr Address, byte[] Content, ProtectionType? NewProtection = null) {
            ChangeProtection(hProcess, Address, (uint)Content.LongLength, ProtectionType.PAGE_READWRITE, out ProtectionType Original);
            
            uint Saved = (uint)Content.LongLength;

            if (hProcess == IntPtr.Zero)
                Marshal.Copy(Content, 0, Address, Content.Length);
            else
                WriteProcessMemory(hProcess, Address, Content, (uint)Content.LongLength, out Saved);

            if (NewProtection.HasValue)
                ChangeProtection(hProcess, Address, (uint)Content.LongLength, NewProtection.Value);
            else
                ChangeProtection(hProcess, Address, (uint)Content.LongLength, Original);

            if (Saved != Content.Length)
                return false;
            return true;
        }

        public static byte[] Read(IntPtr hProcess, IntPtr Address, uint Length) {
            byte[] Buffer = new byte[Length];
            ChangeProtection(hProcess, Address, (uint)Buffer.LongLength, ProtectionType.PAGE_READWRITE, out ProtectionType Original);
            
            uint Readed = (uint)Buffer.LongLength;
            if (hProcess == IntPtr.Zero)
                Marshal.Copy(Address, Buffer, 0, Buffer.Length);
            else
                ReadProcessMemory(hProcess, Address, Buffer, (uint)Buffer.LongLength, out Readed);

            ChangeProtection(hProcess, Address, (uint)Buffer.LongLength, Original);

            if (Readed != Buffer.Length)
                throw new Exception("Failed to Read the Process Memory");

            return Buffer;
        }

        public static string ReadString(IntPtr hProcess, IntPtr Address, bool Unicode) {
            List<byte> Buffer = new List<byte>();
            IntPtr CPos = Address;
            do {
                byte[] Char = Read(hProcess, CPos, Unicode ? 2u : 1u);
                if (Unicode && Char[0] == 0x00 && Char[1] == 0x00)
                    break;
                if (!Unicode && Char[0] == 0x00)
                    break;


                Buffer.AddRange(Char);

                CPos = CPos.Sum(Unicode ? 2u : 1u);
            } while (true);

            return Unicode ? Encoding.Unicode.GetString(Buffer.ToArray()) : Encoding.Default.GetString(Buffer.ToArray());
        }

        public static bool Free(IntPtr hProcess, IntPtr Address, uint Length = 0)
        {
            if (hProcess == IntPtr.Zero)
                return VirtualFree(Address, Length, AllocationType.Release);
            return VirtualFreeEx(hProcess, Address, Length, AllocationType.Release);
        }

        public static bool ChangeProtection(IntPtr hProcess, IntPtr Address, uint Range, ProtectionType Protection, out ProtectionType OriginalProtection) {
            if (hProcess == IntPtr.Zero)
                return VirtualProtect(Address, Range, Protection, out OriginalProtection);
            return VirtualProtectEx(hProcess, Address, Range, Protection, out OriginalProtection);
        }

        public static bool ChangeProtection(IntPtr hProcess, IntPtr Address, uint Range, ProtectionType Protection) {
            if (hProcess == IntPtr.Zero)
                return VirtualProtect(Address, Range, Protection, out _);
            return VirtualProtectEx(hProcess, Address, Range, Protection, out _);
        }

    }
}

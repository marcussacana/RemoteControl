using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace VNX {
    internal static class MemoryManagement {
        internal const uint Padding = 4;
        internal const ulong RegionSize = (1024 * 1024) * 10;//10mb
        //internal const ulong RegionSize = 1024 * 10;//10kb

        static Dictionary<string, List<MemoryRegion>> Regions = new Dictionary<string, List<MemoryRegion>>();
        static Dictionary<string, Process> ProcMap = new Dictionary<string, Process>();

        public static IntPtr MAlloc(Process Process, byte[] Data, bool Executable = false) {
            string Hash = GetHash(Process);
            ProcMap[Hash] = Process;
            if (!Regions.ContainsKey(Hash))
                Regions[Hash] = new List<MemoryRegion>();

            var Protection = Executable ? (ProtectionType?)ProtectionType.PAGE_EXECUTE_READWRITE : null;
            IntPtr Address = CreateBlock(Hash, (uint)Data.LongLength);
            Tools.Write(Process.Handle, Address, Data, Protection);

            return Address;
        }

        public static void MFree(Process Process, IntPtr Address) {
            string Hash = GetHash(Process);
            ProcMap[Hash] = Process;
            if (!Regions.ContainsKey(Hash))
                Regions[Hash] = new List<MemoryRegion>();

            int RIndex = GetRegionIndex(Hash, Address);
            if (RIndex < 0)
                throw new Exception("The given address isn't allocated by the MAlloc or is already disposed");

            ulong Addr = Address.ToUInt64();
            var Region = Regions[Hash][RIndex];

            int BIndex = GetBlockIndex(Region.Blocks, Address);
            Region.Blocks.RemoveAt(BIndex);

            if (Region.Blocks.Count != 0) {
                Regions[Hash][RIndex] = Region;
                return;
            }


            Tools.Free(Process.Handle, Region.Address);
            Regions[Hash].RemoveAt(RIndex);
        }

        static IntPtr CreateBlock(string Hash, uint Length) {
            if (!Regions.ContainsKey(Hash))
                return IntPtr.Zero;

            var PRegions = Regions[Hash];

            for (int i = 0; i < PRegions.Count; i++) {
                var Region = PRegions[i];

                IntPtr Address = NextFreeAddress(Region, Region.Address);
                if (Address == IntPtr.Zero) 
                    continue;

                while (MaxBlockLen(Region, Address) < Length) {
                    Address = NextFreeAddress(Region, Address.Sum(Padding));
                    if (Address == IntPtr.Zero)
                        break;
                }
                if (Address == IntPtr.Zero)
                    continue;

                Regions[Hash][i].Blocks.Add(new RegionBlock(Address, Length));
                return Address;
            }

            var NRegion = CreateRegion(Hash, Length);
            Regions[Hash][GetRegionIndex(Hash, NRegion)].Blocks.Add(new RegionBlock(NRegion.Address, Length));

            return NRegion.Address;
        }

        static MemoryRegion CreateRegion(string Hash, uint Length) {
            uint Size = (uint)RegionSize;
            if (Size < Length)
                Size = Length + (Padding - (Length % Padding));

            IntPtr Addr = Tools.Alloc(ProcMap[Hash].Handle, Size);

            var Region = new MemoryRegion(Addr, Size);
            Regions[Hash].Add(Region);

            return Region;
        }

        static int GetRegionIndex(string Hash, MemoryRegion Region) {
            var PRegions = Regions[Hash];
            for (int i = 0; i < PRegions.Count; i++)
                if (PRegions[i].Address == Region.Address)
                    return i;
            return -1;
        }
        static int GetRegionIndex(string Hash, IntPtr Address) {
            var PRegions = Regions[Hash];
            ulong Addr = Address.ToUInt64();
            for (int i = 0; i < PRegions.Count; i++) {
                ulong RegionAddr = PRegions[i].Address.ToUInt64();
                if (RegionAddr <= Addr && (RegionAddr + PRegions[i].Size) >= Addr)
                    return i;
            }
            return -1;
        }

        static int GetBlockIndex(List<RegionBlock> Blocks, IntPtr Address) {
            ulong Addr = Address.ToUInt64();
            for (int i = 0; i < Blocks.Count; i++) {
                ulong BlockAddr = Blocks[i].Address.ToUInt64();
                if (BlockAddr <= Addr && (BlockAddr + Blocks[i].Size) >= Addr)
                    return i;
            }
            return -1;
        }

        static IntPtr NextFreeAddress(MemoryRegion Region, IntPtr Address) {
            ulong Addr = Address.ToUInt64();

            while (true) {
                bool? Rst = IsFreeAddress(Region, Addr.ToIntPtr());
                if (!Rst.HasValue)
                    return IntPtr.Zero;
                if (!Rst.Value)
                    Addr += Padding;
                else
                    break;
            }

            return Addr.ToIntPtr();
        }
        static bool? IsFreeAddress(MemoryRegion Region, IntPtr Address) {
            ulong RAddr = Region.Address.ToUInt64();
            ulong Addr = Address.ToUInt64();
            if (Addr < RAddr || Addr > RAddr + Region.Size)
                return null;

            return BlockEnd(Region.Blocks, Address) == Address;
        }
        static IntPtr BlockEnd(List<RegionBlock> Blocks, IntPtr Address) {
            ulong Addr = Address.ToUInt64();

            var Result = (from x in Blocks where x.Address.ToUInt64() <= Addr && Addr <= (x.Address.ToUInt64() + x.Size) select x);
            int Count = Result.Count();
            if (Count == 0)
                return Address;

            if (Count != 1)
                throw new InsufficientMemoryException();

            RegionBlock Block = Result.First();
            return (Block.Address.ToUInt64() + Block.Size + Padding).ToIntPtr();

        }
        static ulong MaxBlockLen(MemoryRegion Region, IntPtr Address) {
            var Blocks = Region.Blocks;

            ulong Addr = Address.ToUInt64();
            int Matchs = (from x in Blocks where x.Address.ToUInt64() <= Addr && Addr <= (x.Address.ToUInt64() + x.Size) select x).Count();
            if (Matchs != 0)
                return 0;

            var After = (from x in Blocks where x.Address.ToUInt64() > Addr select x);
            if (After.Count() == 0)
                return Region.Size - (Addr - Region.Address.ToUInt64());
            List<IntPtr> Ignore = new List<IntPtr>();

            while (true) {
                RegionBlock? NextBlock = null;
                foreach (var Block in After) {
                    if (Ignore.Contains(Block.Address))
                        continue;

                    if (!NextBlock.HasValue)
                        NextBlock = Block;

                    if (Block.Address.ToUInt64() < NextBlock?.Address.ToUInt64())
                        NextBlock = Block;
                }

                if (!NextBlock.HasValue)
                    return 0;

                ulong FreeSpace = NextBlock.Value.Address.ToUInt64() - Addr;

                if (FreeSpace < Padding) {
                    Ignore.Add(NextBlock.Value.Address);
                    continue;
                }

                FreeSpace -= Padding % FreeSpace;

                return FreeSpace;
            }
        }

        static string GetHash(int PID) => GetHash(Process.GetProcessById(PID));

        static string GetHash(Process Process) {
            var Info = Process.ProcessName;
            Info += Process.StartTime.ToBinary();
            Info += Process.Id;
            return Hash(Encoding.ASCII.GetBytes(Info));
        }

        static string Hash(byte[] data) {
            using (SHA1Managed Sha1 = new SHA1Managed()) {
                var Hash = Sha1.ComputeHash(data);
                return Convert.ToBase64String(Hash);
            }
        }
    }

    struct MemoryRegion {
        public IntPtr Address;
        public ulong Size;

        public List<RegionBlock> Blocks;

        public MemoryRegion(IntPtr Address, ulong Size) {
            this.Size = Size;
            this.Address = Address;

            Blocks = new List<RegionBlock>();
        }
    }

     struct RegionBlock {
        public IntPtr Address;
        public ulong Size;

        public RegionBlock(IntPtr Address, ulong Size) {
            if (Size % MemoryManagement.Padding != 0)
                Size += MemoryManagement.Padding - (Size % MemoryManagement.Padding);

            this.Size = Size;
            this.Address = Address;
        }
    }
}

using System;
using System.Runtime.InteropServices;

namespace VNX {
    
    /// <summary>
    /// A Pointer with Managed Types Conversion Support 
    /// </summary>
    /// <typeparam name="T">Used to non-common type conversion, you can left with null</typeparam>
    public class MIntPtr<T> : MIntPtr {
        public MIntPtr(IntPtr Pointer) : base(Pointer) { }

        public MIntPtr(int Value) : base(Value) { }

        public MIntPtr(long Value) : base(Value) { }

        public MIntPtr(IntPtr Pointer, bool Unicode) : base(Pointer, Unicode) { }

        public MIntPtr(int Value, bool Unicode) : base(Value, Unicode) { }

        public MIntPtr(long Value, bool Unicode) : base(Value, Unicode) { }



        public static implicit operator bool(MIntPtr<T> Ptr) {
            return Ptr.Pointer.ToInt64() != 0;
        }

        public static implicit operator short(MIntPtr<T> Ptr) {
            return Marshal.ReadInt16(Ptr.Pointer, 0);
        }

        public static implicit operator int(MIntPtr<T> Ptr) {
            return Marshal.ReadInt32(Ptr.Pointer, 0);
        }

        public static implicit operator float(MIntPtr<T> Ptr) {
            return unchecked((float)Marshal.ReadInt32(Ptr.Pointer, 0));
        }

        public static implicit operator long(MIntPtr<T> Ptr) {
            return Marshal.ReadInt64(Ptr.Pointer, 0);
        }

        public static implicit operator double(MIntPtr<T> Ptr) {
            return unchecked((double)Marshal.ReadInt64(Ptr.Pointer, 0));
        }

        public static implicit operator ushort(MIntPtr<T> Ptr) {
            return unchecked((ushort)Marshal.ReadInt16(Ptr.Pointer, 0));
        }

        public static implicit operator uint(MIntPtr<T> Ptr) {
            return unchecked((uint)Marshal.ReadInt32(Ptr.Pointer, 0));
        }

        public static implicit operator ulong(MIntPtr<T> Ptr) {
            return unchecked((ulong)Marshal.ReadInt64(Ptr.Pointer, 0));
        }

        public static implicit operator IntPtr(MIntPtr<T> Ptr) {
            return Ptr.Pointer;
        }

        public static implicit operator MIntPtr<T>(IntPtr Ptr) {
            return new MIntPtr<T>(Ptr);
        }

        public static implicit operator MIntPtr<T>(Guid ID) {
            byte[] Data = ID.ToByteArray();
            IntPtr Pointer = Marshal.AllocHGlobal(Data.Length);
            Marshal.Copy(Data, 0, Pointer, Data.Length);
            return new MIntPtr<T>(Pointer);
        }

        public static implicit operator string(MIntPtr<T> Ptr) {
            return Ptr.Unicode ? Marshal.PtrToStringUni(Ptr.Pointer) : Marshal.PtrToStringAnsi(Ptr.Pointer);
        }

        public static implicit operator T(MIntPtr<T> Ptr) {
            Type Type = typeof(T);
            if (Type.IsValueType && !Type.IsPrimitive && !Type.IsEnum){
                return (T)Marshal.PtrToStructure(Ptr.Pointer, Type);
            }
            if (Type.IsValueType && Type.IsEnum) {
                Type VT = Enum.GetUnderlyingType(Type);
                return (T)Convert.ChangeType(Ptr.Pointer.ToUInt64(), VT);
            }
            if (Type.IsSubclassOf(typeof(Delegate))) {
                return (T)(object)Marshal.GetDelegateForFunctionPointer(Ptr.Pointer, Type);
            }

            if (Type == typeof(bool))
                return (T)Convert.ChangeType(Ptr.Pointer != IntPtr.Zero, typeof(bool));
            if (Type == typeof(int))
                return (T)Convert.ChangeType(Ptr.Pointer.ToInt32(), typeof(int));
            if (Type == typeof(uint))
                return (T)Convert.ChangeType(Ptr.Pointer.ToUInt32(), typeof(uint));
            if (Type == typeof(long))
                return (T)Convert.ChangeType(Ptr.Pointer.ToInt64(), typeof(long));
            if (Type == typeof(ulong))
                return (T)Convert.ChangeType(Ptr.Pointer.ToUInt64(), typeof(ulong));
            if (Type == typeof(double))
                return (T)Convert.ChangeType(Ptr.Pointer.ToInt64(), typeof(double));
            if (Type == typeof(float))
                return (T)Convert.ChangeType(Ptr.Pointer.ToInt32(), typeof(float));
            if (Type == typeof(IntPtr))
                return (T)Convert.ChangeType(Ptr.Pointer.ToInt64(), typeof(IntPtr));

            throw new TypeLoadException("Unexpected Generic Type");
        }
    }

    /// <summary>
    /// A Pointer with Managed Types Conversion Support 
    /// </summary>
    public class MIntPtr {
        internal bool Unicode = true;
        internal IntPtr Pointer;
        public MIntPtr(IntPtr Pointer) {
            this.Pointer = Pointer;
        }
        public MIntPtr(IntPtr Pointer, bool Unicode) {
            this.Pointer = Pointer;
            this.Unicode = Unicode;
        }

        public MIntPtr(int Value) {
            Pointer = new IntPtr(Value);
        }
        public MIntPtr(int Value, bool Unicode) {
            Pointer = new IntPtr(Value);
            this.Unicode = Unicode;
        }
        public MIntPtr(long Value) {
            Pointer = new IntPtr(Value);
        }
        public MIntPtr(long Value, bool Unicode) {
            Pointer = new IntPtr(Value);
            this.Unicode = Unicode;
        }

        public static implicit operator bool(MIntPtr Ptr) {
            return Ptr.Pointer.ToInt64() != 0;
        }

        public static implicit operator short(MIntPtr Ptr) {
            return Marshal.ReadInt16(Ptr.Pointer, 0);
        }

        public static implicit operator int(MIntPtr Ptr) {
            return Marshal.ReadInt32(Ptr.Pointer, 0);
        }

        public static implicit operator float(MIntPtr Ptr) {
            return unchecked((float)Marshal.ReadInt32(Ptr.Pointer, 0));
        }

        public static implicit operator long(MIntPtr Ptr) {
            return Marshal.ReadInt64(Ptr.Pointer, 0);
        }

        public static implicit operator double(MIntPtr Ptr) {
            return unchecked((double)Marshal.ReadInt64(Ptr.Pointer, 0));
        }

        public static implicit operator ushort(MIntPtr Ptr) {
            return unchecked((ushort)Marshal.ReadInt16(Ptr.Pointer, 0));
        }

        public static implicit operator uint(MIntPtr Ptr) {
            return unchecked((uint)Marshal.ReadInt32(Ptr.Pointer, 0));
        }

        public static implicit operator ulong(MIntPtr Ptr) {
            return unchecked((ulong)Marshal.ReadInt64(Ptr.Pointer, 0));
        }

        public static implicit operator IntPtr(MIntPtr Ptr) {
            return Ptr.Pointer;
        }

        public static implicit operator MIntPtr(IntPtr Ptr) {
            return new MIntPtr(Ptr);
        }

        public static implicit operator MIntPtr(Guid ID) {
            byte[] Data = ID.ToByteArray();
            IntPtr Pointer = Marshal.AllocHGlobal(Data.Length);
            Marshal.Copy(Data, 0, Pointer, Data.Length);
            return new MIntPtr(Pointer);
        }

        public static implicit operator string(MIntPtr Ptr) {
            return Ptr.Unicode ? Marshal.PtrToStringUni(Ptr.Pointer) : Marshal.PtrToStringAnsi(Ptr.Pointer);
        }
    }

}

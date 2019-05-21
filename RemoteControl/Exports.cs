using System;

namespace VNX
{
    public struct ImportEntry
    {

        /// <summary>
        /// The Imported Module Name
        /// </summary>
        public string Module;

        /// <summary>
        /// The Imported Function Name
        /// </summary>
        public string Function;

        /// <summary>
        /// The Import Ordinal Hint
        /// </summary>
        public ushort Ordinal;

        /// <summary>
        /// The Address of this Import in the IAT (Import Address Table)
        /// </summary>
        public IntPtr ImportAddress;

        /// <summary>
        /// The Address of the Imported Function
        /// </summary>
        public IntPtr FunctionAddress;
    }
}

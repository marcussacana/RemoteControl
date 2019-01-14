using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using VNX;

namespace Example {
    static class ExampleHook {
        /// <summary>
        /// Sample Font Hook 
        /// <para> Warning: This is just a sample, don't works with any application, tested with notepad.
        /// This only hooks the CreateFontIndirectW, we have it CreateFontA, CreateFontW and CreateFontIndirectA to handle with others programs</para>
        /// </summary>
        public static void HookFont() {
            //Attachs the Debugger
            Debugger.Launch();

            //Create the delegate and assign the hook function to him
            dCreateFontIndirect = new CreateFontIndirectWDelegate(CreateFontIndirectWHook);

            //Create to the given dll export with the given delegate
            hCreateFontIndirect = new UnmanagedHook<CreateFontIndirectWDelegate>("gdi32.dll", "CreateFontIndirectW", dCreateFontIndirect);

            //Install the Hook
            hCreateFontIndirect.Install();

            //Allow the process Execution and test the hook
            new Thread(() => {
                Thread.Sleep(1000);

                LOGFONTW Font = new LOGFONTW();
                Font.lfFaceName = "Times New Roman";
                CreateFontIndirectW(ref Font);

                MessageBox.Show("Allways Comic Sans MS!\nChanged? If not, test it with the notepad...", "Injected Assembly", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }).Start();
        }

        //A method that matchs with the hook delegate
        public static IntPtr CreateFontIndirectWHook(ref LOGFONTW lplf) {
            lplf.lfFaceName = "Comic Sans MS";

            //Call the original CreateFontIndirect
            return CreateFontIndirectW(ref lplf);
        }

        static CreateFontIndirectWDelegate dCreateFontIndirect;
        static UnmanagedHook<CreateFontIndirectWDelegate> hCreateFontIndirect;

        //The Example DLL Export to be hooked
        [DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        static extern IntPtr CreateFontIndirectW([In, Out] ref LOGFONTW lplf);

        //Example of a valid delegate to host the hook 
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]//Required attribute
        delegate IntPtr CreateFontIndirectWDelegate([In, Out] ref LOGFONTW lplf);


        //LOGFONT structure
        #region LOGFONT
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LOGFONTW {
            public const int LF_FACESIZE = 32;
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public FontWeight lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public FontCharSet lfCharSet;
            public FontPrecision lfOutPrecision;
            public FontClipPrecision lfClipPrecision;
            public FontQuality lfQuality;
            public FontPitchAndFamily lfPitchAndFamily;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LF_FACESIZE)]
            public string lfFaceName;

        }

        [Flags]
        public enum FontWeight : int {
            FW_DONTCARE = 0,
            FW_THIN = 100,
            FW_EXTRALIGHT = 200,
            FW_LIGHT = 300,
            FW_NORMAL = 400,
            FW_MEDIUM = 500,
            FW_SEMIBOLD = 600,
            FW_BOLD = 700,
            FW_EXTRABOLD = 800,
            FW_HEAVY = 900,
        }

        [Flags]
        public enum FontCharSet : byte {
            ANSI_CHARSET = 0,
            DEFAULT_CHARSET = 1,
            SYMBOL_CHARSET = 2,
            SHIFTJIS_CHARSET = 128,
            HANGEUL_CHARSET = 129,
            HANGUL_CHARSET = 129,
            GB2312_CHARSET = 134,
            CHINESEBIG5_CHARSET = 136,
            OEM_CHARSET = 255,
            JOHAB_CHARSET = 130,
            HEBREW_CHARSET = 177,
            ARABIC_CHARSET = 178,
            GREEK_CHARSET = 161,
            TURKISH_CHARSET = 162,
            VIETNAMESE_CHARSET = 163,
            THAI_CHARSET = 222,
            EASTEUROPE_CHARSET = 238,
            RUSSIAN_CHARSET = 204,
            MAC_CHARSET = 77,
            BALTIC_CHARSET = 186,
        }

        [Flags]
        public enum FontPrecision : byte {
            OUT_DEFAULT_PRECIS = 0,
            OUT_STRING_PRECIS = 1,
            OUT_CHARACTER_PRECIS = 2,
            OUT_STROKE_PRECIS = 3,
            OUT_TT_PRECIS = 4,
            OUT_DEVICE_PRECIS = 5,
            OUT_RASTER_PRECIS = 6,
            OUT_TT_ONLY_PRECIS = 7,
            OUT_OUTLINE_PRECIS = 8,
            OUT_SCREEN_OUTLINE_PRECIS = 9,
            OUT_PS_ONLY_PRECIS = 10,
        }

        [Flags]
        public enum FontClipPrecision : byte {
            CLIP_DEFAULT_PRECIS = 0,
            CLIP_CHARACTER_PRECIS = 1,
            CLIP_STROKE_PRECIS = 2,
            CLIP_MASK = 0xf,
            CLIP_LH_ANGLES = (1 << 4),
            CLIP_TT_ALWAYS = (2 << 4),
            CLIP_DFA_DISABLE = (4 << 4),
            CLIP_EMBEDDED = (8 << 4),
        }

        [Flags]
        public enum FontQuality : byte {
            DEFAULT_QUALITY = 0,
            DRAFT_QUALITY = 1,
            PROOF_QUALITY = 2,
            NONANTIALIASED_QUALITY = 3,
            ANTIALIASED_QUALITY = 4,
            CLEARTYPE_QUALITY = 5,
            CLEARTYPE_NATURAL_QUALITY = 6,
        }

        [Flags]
        public enum FontPitchAndFamily : byte {
            DEFAULT_PITCH = 0,
            FIXED_PITCH = 1,
            VARIABLE_PITCH = 2,
            FF_DONTCARE = (0 << 4),
            FF_ROMAN = (1 << 4),
            FF_SWISS = (2 << 4),
            FF_MODERN = (3 << 4),
            FF_SCRIPT = (4 << 4),
            FF_DECORATIVE = (5 << 4),
        }
        #endregion
    }
}
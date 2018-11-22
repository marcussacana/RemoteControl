namespace VNX {
    internal enum HResult : uint {
        /// <summary>
        /// STATUS: OK
        /// </summary>
        S_OK = 0x00000000,
        ///<Summary>
        ///STATUS: Data value was truncated. 
        ///</Summary>
        CLDB_S_TRUNCATION = 0x131106,
        ///<Summary>
        ///Attempt to define an object that already exists in valid scenerios. 
        ///</Summary>
        META_S_DUPLICATE = 0x131197,
        ///<Summary>
        ///Attempt to SetIP not at a sequence point sequence point. 
        ///</Summary>
        CORDBG_S_BAD_START_SEQUENCE_POINT = 0x13130b,
        ///<Summary>
        ///Attempt to SetIP when not going to a sequence point. If both this and CORDBG_E_BAD_START_SEQUENCE_POINT are true, only CORDBG_E_BAD_START_SEQUENCE_POINT will be reported.
        ///</Summary>
        CORDBG_S_BAD_END_SEQUENCE_POINT = 0x13130c,
        ///<Summary>
        ///Some Func evals will lack a return value, 
        ///</Summary>
        CORDBG_S_FUNC_EVAL_HAS_NO_RESULT = 0x131316,
        ///<Summary>
        ///The Debugging API doesn't support dereferencing void pointers. 
        ///</Summary>
        CORDBG_S_VALUE_POINTS_TO_VOID = 0x131317,
        ///<Summary>
        ///The func eval completed, but was aborted. 
        ///</Summary>
        CORDBG_S_FUNC_EVAL_ABORTED = 0x131319,
        ///<Summary>
        ///The stack walk has reached the end of the stack.  There are no more frames to walk. 
        ///</Summary>
        CORDBG_S_AT_END_OF_STACK = 0x131324,
        ///<Summary>
        ///Not all bits specified were successfully applied 
        ///</Summary>
        CORDBG_S_NOT_ALL_BITS_SET = 0x131c13,
        ///<Summary>
        ///cannot find cvtres.exe 
        ///</Summary>
        CEE_E_CVTRES_NOT_FOUND = 0x80131001,
        ///<Summary>
        ///The type had been unloaded.
        ///</Summary>
        COR_E_TYPEUNLOADED = 0x80131013,
        ///<Summary>
        ///access unloaded appdomain 
        ///</Summary>
        COR_E_APPDOMAINUNLOADED = 0x80131014,
        ///<Summary>
        ///Error while unloading an appdomain 
        ///</Summary>
        COR_E_CANNOTUNLOADAPPDOMAIN = 0x80131015,
        ///<Summary>
        ///Assembly is being currently being loaded 
        ///</Summary>
        MSEE_E_ASSEMBLYLOADINPROGRESS = 0x80131016,
        ///<Summary>
        ///The module was expected to contain an assembly manifest.
        ///</Summary>
        COR_E_ASSEMBLYEXPECTED = 0x80131018,
        ///<Summary>
        ///Attempt to load an unverifiable exe with fixups (IAT with more than 2 sections or a TLS section) 
        ///</Summary>
        COR_E_FIXUPSINEXE = 0x80131019,
        ///<Summary>
        ///The assembly is built by a runtime newer than the currently loaded runtime, and cannot be loaded. 
        ///</Summary>
        COR_E_NEWER_RUNTIME = 0x8013101b,
        ///<Summary>
        ///The module cannot be loaded because only single file assemblies are supported. 
        ///</Summary>
        COR_E_MULTIMODULEASSEMBLIESDIALLOWED = 0x8013101e,
        ///<Summary>
        ///Host detects deadlock on a blocking operation 
        ///</Summary>
        HOST_E_DEADLOCK = 0x80131020,
        ///<Summary>
        ///The operation is invalid 
        ///</Summary>
        HOST_E_INVALIDOPERATION = 0x80131022,
        ///<Summary>
        ///CLR has been disabled due to unrecoverable error 
        ///</Summary>
        HOST_E_CLRNOTAVAILABLE = 0x80131023,
        ///<Summary>
        ///ExitProcess due to ThreadAbort escalation 
        ///</Summary>
        HOST_E_EXITPROCESS_THREADABORT = 0x80131027,
        ///<Summary>
        ///ExitProcess due to AD Unload escalation 
        ///</Summary>
        HOST_E_EXITPROCESS_ADUNLOAD = 0x80131028,
        ///<Summary>
        ///ExitProcess due to Timeout escalation 
        ///</Summary>
        HOST_E_EXITPROCESS_TIMEOUT = 0x80131029,
        ///<Summary>
        ///ExitProcess due to OutOfMemory escalation 
        ///</Summary>
        HOST_E_EXITPROCESS_OUTOFMEMORY = 0x8013102a,
        ///<Summary>
        ///The check of the module's hash failed. 
        ///</Summary>
        COR_E_MODULE_HASH_CHECK_FAILED = 0x80131039,
        ///<Summary>
        ///The located assembly's manifest definition does not match the assembly reference. 
        ///</Summary>
        FUSION_E_REF_DEF_MISMATCH = 0x80131040,
        ///<Summary>
        ///The private assembly was located outside the appbase directory. 
        ///</Summary>
        FUSION_E_INVALID_PRIVATE_ASM_LOCATION = 0x80131041,
        ///<Summary>
        ///A module specified in the manifest was not found. 
        ///</Summary>
        FUSION_E_ASM_MODULE_MISSING = 0x80131042,
        ///<Summary>
        ///A strongly-named assembly is required. 
        ///</Summary>
        FUSION_E_PRIVATE_ASM_DISALLOWED = 0x80131044,
        ///<Summary>
        ///The check of the signature failed. 
        ///</Summary>
        FUSION_E_SIGNATURE_CHECK_FAILED = 0x80131045,
        ///<Summary>
        ///The given assembly name or codebase was invalid. 
        ///</Summary>
        FUSION_E_INVALID_NAME = 0x80131047,
        ///<Summary>
        ///HTTP download of assemblies has been disabled for this appdomain. 
        ///</Summary>
        FUSION_E_CODE_DOWNLOAD_DISABLED = 0x80131048,
        ///<Summary>
        ///Assembly in host store has a different signature than assembly in GAC 
        ///</Summary>
        FUSION_E_HOST_GAC_ASM_MISMATCH = 0x80131050,
        ///<Summary>
        ///Hosted environment doesn't permit loading by location 
        ///</Summary>
        FUSION_E_LOADFROM_BLOCKED = 0x80131051,
        ///<Summary>
        ///Failed to add file to AppDomain cache 
        ///</Summary>
        FUSION_E_CACHEFILE_FAILED = 0x80131052,
        ///<Summary>
        ///The requested assembly version conflicts with what is already bound in the app domain or specified in the manifest 
        ///</Summary>
        FUSION_E_APP_DOMAIN_LOCKED = 0x80131053,
        ///<Summary>
        ///The requested assembly name was neither found in the GAC nor in the manifest or the manifest's specified location is wrong 
        ///</Summary>
        FUSION_E_CONFIGURATION_ERROR = 0x80131054,
        ///<Summary>
        ///Unexpected error while parsing the specified manifest 
        ///</Summary>
        FUSION_E_MANIFEST_PARSE_ERROR = 0x80131055,
        ///<Summary>
        ///Reference assemblies should not be loaded for execution.  They can only be loaded in the Reflection-only loader context. 
        ///</Summary>
        COR_E_LOADING_REFERENCE_ASSEMBLY = 0x80131058,
        ///<Summary>
        ///The native image could not be loaded, because it was generated for use by a different version of the runtime. 
        ///</Summary>
        COR_E_NI_AND_RUNTIME_VERSION_MISMATCH = 0x80131059,
        ///<Summary>
        ///Contract Windows Runtime assemblies cannot be loaded for execution.  Make sure your application only contains non-contract Windows Runtime assemblies 
        ///</Summary>
        COR_E_LOADING_WINMD_REFERENCE_ASSEMBLY = 0x80131069,
        ///<Summary>
        ///Error occurred during a read. 
        ///</Summary>
        CLDB_E_FILE_BADREAD = 0x80131100,
        ///<Summary>
        ///Error occurred during a write. 
        ///</Summary>
        CLDB_E_FILE_BADWRITE = 0x80131101,
        ///<Summary>
        ///Old version error. 
        ///</Summary>
        CLDB_E_FILE_OLDVER = 0x80131107,
        ///<Summary>
        ///Create of shared memory failed.  A memory mapping of the same name already exists. 
        ///</Summary>
        CLDB_E_SMDUPLICATE = 0x8013110a,
        ///<Summary>
        ///There isn't .CLB data in the memory or stream. 
        ///</Summary>
        CLDB_E_NO_DATA = 0x8013110b,
        ///<Summary>
        ///The importing scope is not comptabile with the emitting scope 
        ///</Summary>
        CLDB_E_INCOMPATIBLE = 0x8013110d,
        ///<Summary>
        ///File is corrupt. 
        ///</Summary>
        CLDB_E_FILE_CORRUPT = 0x8013110e,
        ///<Summary>
        ///cannot open a incrementally build scope for full update 
        ///</Summary>
        CLDB_E_BADUPDATEMODE = 0x80131110,
        ///<Summary>
        ///Index %s not found. 
        ///</Summary>
        CLDB_E_INDEX_NOTFOUND = 0x80131124,
        ///<Summary>
        ///Record wasn't found on lookup. 
        ///</Summary>
        CLDB_E_RECORD_NOTFOUND = 0x80131130,
        ///<Summary>
        ///Record is emitted out of order. 
        ///</Summary>
        CLDB_E_RECORD_OUTOFORDER = 0x80131135,
        ///<Summary>
        ///A blob or string was too big. 
        ///</Summary>
        CLDB_E_TOO_BIG = 0x80131154,
        ///<Summary>
        ///A token of the wrong type passed to a metadata function. 
        ///</Summary>
        META_E_INVALID_TOKEN_TYPE = 0x8013115f,
        ///<Summary>
        ///Merge: Inconsistency in meta data import scope 
        ///</Summary>
        META_E_BADMETADATA = 0x8013118a,
        ///<Summary>
        ///Bad binary signature 
        ///</Summary>
        META_E_BAD_SIGNATURE = 0x80131192,
        ///<Summary>
        ///Bad input parameters 
        ///</Summary>
        META_E_BAD_INPUT_PARAMETER = 0x80131193,
        ///<Summary>
        ///Cannot resolve typeref 
        ///</Summary>
        META_E_CANNOTRESOLVETYPEREF = 0x80131196,
        ///<Summary>
        ///No logical space left to create more user strings. 
        ///</Summary>
        META_E_STRINGSPACE_FULL = 0x80131198,
        ///<Summary>
        ///Unmark all has been called already 
        ///</Summary>
        META_E_HAS_UNMARKALL = 0x8013119a,
        ///<Summary>
        ///Must call UnmarkAll first before marking. 
        ///</Summary>
        META_E_MUST_CALL_UNMARKALL = 0x8013119b,
        ///<Summary>
        ///Known custom attribute on invalid target. 
        ///</Summary>
        META_E_CA_INVALID_TARGET = 0x801311c0,
        ///<Summary>
        ///Known custom attribute had invalid value. 
        ///</Summary>
        META_E_CA_INVALID_VALUE = 0x801311c1,
        ///<Summary>
        ///Known custom attribute blob is bad format. 
        ///</Summary>
        META_E_CA_INVALID_BLOB = 0x801311c2,
        ///<Summary>
        ///Known custom attribute blob has repeated named argument. 
        ///</Summary>
        META_E_CA_REPEATED_ARG = 0x801311c3,
        ///<Summary>
        ///Known custom attrubte named arg not recognized. 
        ///</Summary>
        META_E_CA_UNKNOWN_ARGUMENT = 0x801311c4,
        ///<Summary>
        ///Known attribute parser found unexpected type. 
        ///</Summary>
        META_E_CA_UNEXPECTED_TYPE = 0x801311c7,
        ///<Summary>
        ///Known attribute parser only handles fields -- no properties. 
        ///</Summary>
        META_E_CA_INVALID_ARGTYPE = 0x801311c8,
        ///<Summary>
        ///Known attribute parser found an argument that is invalid for the object it is applied to. 
        ///</Summary>
        META_E_CA_INVALID_ARG_FOR_TYPE = 0x801311c9,
        ///<Summary>
        ///The format of the UUID was invalid. 
        ///</Summary>
        META_E_CA_INVALID_UUID = 0x801311ca,
        ///<Summary>
        ///The MarshalAs attribute has fields set that are not valid for the specified unmanaged type. 
        ///</Summary>
        META_E_CA_INVALID_MARSHALAS_FIELDS = 0x801311cb,
        ///<Summary>
        ///The specified unmanaged type is only valid on fields. 
        ///</Summary>
        META_E_CA_NT_FIELDONLY = 0x801311cc,
        ///<Summary>
        ///The parameter index cannot be negative. 
        ///</Summary>
        META_E_CA_NEGATIVE_PARAMINDEX = 0x801311cd,
        ///<Summary>
        ///The constant size cannot be negative. 
        ///</Summary>
        META_E_CA_NEGATIVE_CONSTSIZE = 0x801311cf,
        ///<Summary>
        ///A fixed string requires a size. 
        ///</Summary>
        META_E_CA_FIXEDSTR_SIZE_REQUIRED = 0x801311d0,
        ///<Summary>
        ///A custom marshaler requires the custom marshaler type. 
        ///</Summary>
        META_E_CA_CUSTMARSH_TYPE_REQUIRED = 0x801311d1,
        ///<Summary>
        ///SaveDelta was called without being in EnC mode 
        ///</Summary>
        META_E_NOT_IN_ENC_MODE = 0x801311d4,
        ///<Summary>
        ///InternalsVisibleTo can't have a version, culture, or processor architecture. 
        ///</Summary>
        META_E_CA_BAD_FRIENDS_ARGS = 0x801311e5,
        ///<Summary>
        ///Strong-name signed assemblies can only grant friend access to strong name-signed assemblies 
        ///</Summary>
        META_E_CA_FRIENDS_SN_REQUIRED = 0x801311e6,
        ///<Summary>
        ///Rid is out of range. 
        ///</Summary>
        VLDTR_E_RID_OUTOFRANGE = 0x80131203,
        ///<Summary>
        ///String offset is invalid. 
        ///</Summary>
        VLDTR_E_STRING_INVALID = 0x80131206,
        ///<Summary>
        ///GUID offset is invalid. 
        ///</Summary>
        VLDTR_E_GUID_INVALID = 0x80131207,
        ///<Summary>
        ///Blob offset if invalid. 
        ///</Summary>
        VLDTR_E_BLOB_INVALID = 0x80131208,
        ///<Summary>
        ///MemberRef has invalid calling convention. 
        ///</Summary>
        VLDTR_E_MR_BADCALLINGCONV = 0x80131224,
        ///<Summary>
        ///Signature specified is zero-sized. 
        ///</Summary>
        VLDTR_E_SIGNULL = 0x80131237,
        ///<Summary>
        ///Method signature has invalid calling convention. 
        ///</Summary>
        VLDTR_E_MD_BADCALLINGCONV = 0x80131239,
        ///<Summary>
        ///Method is marked static but has HASTHIS/implicitTHIS set on the calling convention. 
        ///</Summary>
        VLDTR_E_MD_THISSTATIC = 0x8013123a,
        ///<Summary>
        ///Method is not marked static but is not HASTHIS/implicitTHIS. 
        ///</Summary>
        VLDTR_E_MD_NOTTHISNOTSTATIC = 0x8013123b,
        ///<Summary>
        ///Method signature is missing the argument count. 
        ///</Summary>
        VLDTR_E_MD_NOARGCNT = 0x8013123c,
        ///<Summary>
        ///Signature missing element type. 
        ///</Summary>
        VLDTR_E_SIG_MISSELTYPE = 0x8013123d,
        ///<Summary>
        ///Signature missing token. 
        ///</Summary>
        VLDTR_E_SIG_MISSTKN = 0x8013123e,
        ///<Summary>
        ///Signature has bad token. 
        ///</Summary>
        VLDTR_E_SIG_TKNBAD = 0x8013123f,
        ///<Summary>
        ///Signature is missing function pointer. 
        ///</Summary>
        VLDTR_E_SIG_MISSFPTR = 0x80131240,
        ///<Summary>
        ///Signature has function pointer missing argument count. 
        ///</Summary>
        VLDTR_E_SIG_MISSFPTRARGCNT = 0x80131241,
        ///<Summary>
        ///Signature is missing rank specification. 
        ///</Summary>
        VLDTR_E_SIG_MISSRANK = 0x80131242,
        ///<Summary>
        ///Signature is missing count of sized dimensions. 
        ///</Summary>
        VLDTR_E_SIG_MISSNSIZE = 0x80131243,
        ///<Summary>
        ///Signature is missing size of dimension. 
        ///</Summary>
        VLDTR_E_SIG_MISSSIZE = 0x80131244,
        ///<Summary>
        ///Signature is missing count of lower bounds. 
        ///</Summary>
        VLDTR_E_SIG_MISSNLBND = 0x80131245,
        ///<Summary>
        ///Signature is missing a lower bound. 
        ///</Summary>
        VLDTR_E_SIG_MISSLBND = 0x80131246,
        ///<Summary>
        ///Signature has bad element type. 
        ///</Summary>
        VLDTR_E_SIG_BADELTYPE = 0x80131247,
        ///<Summary>
        ///TypeDef not nested has encloser. 
        ///</Summary>
        VLDTR_E_TD_ENCLNOTNESTED = 0x80131256,
        ///<Summary>
        ///Field/method is PInvoke but is not marked Static. 
        ///</Summary>
        VLDTR_E_FMD_PINVOKENOTSTATIC = 0x80131277,
        ///<Summary>
        ///E_T_SENTINEL in MethodDef signature 
        ///</Summary>
        VLDTR_E_SIG_SENTINMETHODDEF = 0x801312df,
        ///<Summary>
        ///E_T_SENTINEL &lt;=&gt; VARARG 
        ///</Summary>
        VLDTR_E_SIG_SENTMUSTVARARG = 0x801312e0,
        ///<Summary>
        ///Multiple E_T_SENTINELs 
        ///</Summary>
        VLDTR_E_SIG_MULTSENTINELS = 0x801312e1,
        ///<Summary>
        ///Signature missing argument 
        ///</Summary>
        VLDTR_E_SIG_MISSARG = 0x801312e3,
        ///<Summary>
        ///Field of ByRef type 
        ///</Summary>
        VLDTR_E_SIG_BYREFINFIELD = 0x801312e4,
        ///<Summary>
        ///Unrecoverable API error. 
        ///</Summary>
        CORDBG_E_UNRECOVERABLE_ERROR = 0x80131300,
        ///<Summary>
        ///Process was terminated. 
        ///</Summary>
        CORDBG_E_PROCESS_TERMINATED = 0x80131301,
        ///<Summary>
        ///Process not synchronized. 
        ///</Summary>
        CORDBG_E_PROCESS_NOT_SYNCHRONIZED = 0x80131302,
        ///<Summary>
        ///A class is not loaded. 
        ///</Summary>
        CORDBG_E_CLASS_NOT_LOADED = 0x80131303,
        ///<Summary>
        ///An IL variable is not available at the 
        ///</Summary>
        CORDBG_E_IL_VAR_NOT_AVAILABLE = 0x80131304,
        ///<Summary>
        ///A reference value was found to be bad 
        ///</Summary>
        CORDBG_E_BAD_REFERENCE_VALUE = 0x80131305,
        ///<Summary>
        ///A field in a class is not available, 
        ///</Summary>
        CORDBG_E_FIELD_NOT_AVAILABLE = 0x80131306,
        ///<Summary>
        ///"Native frame only" operation on 
        ///</Summary>
        CORDBG_E_NON_NATIVE_FRAME = 0x80131307,
        ///<Summary>
        ///The code is currently unavailable 
        ///</Summary>
        CORDBG_E_CODE_NOT_AVAILABLE = 0x80131309,
        ///<Summary>
        ///Attempt to get a ICorDebugFunction for 
        ///</Summary>
        CORDBG_E_FUNCTION_NOT_IL = 0x8013130a,
        ///<Summary>
        ///SetIP isn't possible, because SetIP would 
        ///</Summary>
        CORDBG_E_CANT_SET_IP_INTO_FINALLY = 0x8013130e,
        ///<Summary>
        ///SetIP isn't possible because it would move 
        ///</Summary>
        CORDBG_E_CANT_SET_IP_OUT_OF_FINALLY = 0x8013130f,
        ///<Summary>
        ///SetIP isn't possible, because SetIP would 
        ///</Summary>
        CORDBG_E_CANT_SET_IP_INTO_CATCH = 0x80131310,
        ///<Summary>
        ///Setip cannot be done on any frame except 
        ///</Summary>
        CORDBG_E_SET_IP_NOT_ALLOWED_ON_NONLEAF_FRAME = 0x80131311,
        ///<Summary>
        ///SetIP isn't allowed. For example, there is 
        ///</Summary>
        CORDBG_E_SET_IP_IMPOSSIBLE = 0x80131312,
        ///<Summary>
        ///Func eval can't work if we're, for example, 
        ///</Summary>
        CORDBG_E_FUNC_EVAL_BAD_START_POINT = 0x80131313,
        ///<Summary>
        ///This object value is no longer valid. 
        ///</Summary>
        CORDBG_E_INVALID_OBJECT = 0x80131314,
        ///<Summary>
        ///If you call CordbEval::GetResult before the 
        ///</Summary>
        CORDBG_E_FUNC_EVAL_NOT_COMPLETE = 0x80131315,
        ///<Summary>
        ///A static variable isn't available because 
        ///</Summary>
        CORDBG_E_STATIC_VAR_NOT_AVAILABLE = 0x8013131a,
        ///<Summary>
        ///SetIP can't leave or enter a filter 
        ///</Summary>
        CORDBG_E_CANT_SETIP_INTO_OR_OUT_OF_FILTER = 0x8013131c,
        ///<Summary>
        ///You can't change JIT settings for ZAP 
        ///</Summary>
        CORDBG_E_CANT_CHANGE_JIT_SETTING_FOR_ZAP_MODULE = 0x8013131d,
        ///<Summary>
        ///SetIP isn't possible because it would move 
        ///</Summary>
        CORDBG_E_CANT_SET_IP_OUT_OF_FINALLY_ON_WIN64 = 0x8013131e,
        ///<Summary>
        ///SetIP isn't possible because it would move 
        ///</Summary>
        CORDBG_E_CANT_SET_IP_OUT_OF_CATCH_ON_WIN64 = 0x8013131f,
        ///<Summary>
        ///Can't use JMC on this code (likely wrong jit settings). 
        ///</Summary>
        CORDBG_E_CANT_SET_TO_JMC = 0x80131323,
        ///<Summary>
        ///Internal frame markers have no associated context. 
        ///</Summary>
        CORDBG_E_NO_CONTEXT_FOR_INTERNAL_FRAME = 0x80131325,
        ///<Summary>
        ///The current frame is not a child frame. 
        ///</Summary>
        CORDBG_E_NOT_CHILD_FRAME = 0x80131326,
        ///<Summary>
        /// The provided CONTEXT does not match the specified thread.  
        //    The stack pointer in the provided CONTEXT must match the cached stack base and stack limit of the thread. 
        ///</Summary>
        CORDBG_E_NON_MATCHING_CONTEXT = 0x80131327,


        ///<Summary>
        ///The stackwalker is now past the end of stack.  No information is available. 
        ///</Summary>
        CORDBG_E_PAST_END_OF_STACK = 0x80131328,
        ///<Summary>
        ///Func eval cannot update a variable stored in a register on a non-leaf frame.  The most likely cause is that such a variable is passed as a ref/out argument. 
        ///</Summary>
        CORDBG_E_FUNC_EVAL_CANNOT_UPDATE_REGISTER_IN_NONLEAF_FRAME = 0x80131329,
        ///<Summary>
        ///The state of the thread is invalid. 
        ///</Summary>
        CORDBG_E_BAD_THREAD_STATE = 0x8013132d,
        ///<Summary>
        ///This process has already been attached to 
        ///</Summary>
        CORDBG_E_DEBUGGER_ALREADY_ATTACHED = 0x8013132e,
        ///<Summary>
        ///Returned from a call to Continue that was 
        ///</Summary>
        CORDBG_E_SUPERFLOUS_CONTINUE = 0x8013132f,
        ///<Summary>
        ///Can't perfrom SetValue on non-leaf frames. 
        ///</Summary>
        CORDBG_E_SET_VALUE_NOT_ALLOWED_ON_NONLEAF_FRAME = 0x80131330,
        ///<Summary>
        ///Tried to do EnC on a module that wasn't 
        ///</Summary>
        CORDBG_E_ENC_MODULE_NOT_ENC_ENABLED = 0x80131332,
        ///<Summary>
        ///Setip cannot be done on any exception 
        ///</Summary>
        CORDBG_E_SET_IP_NOT_ALLOWED_ON_EXCEPTION = 0x80131333,
        ///<Summary>
        ///The 'variable' doesn't exist because it is a 
        ///</Summary>
        CORDBG_E_VARIABLE_IS_ACTUALLY_LITERAL = 0x80131334,
        ///<Summary>
        ///Process has been detached from 
        ///</Summary>
        CORDBG_E_PROCESS_DETACHED = 0x80131335,
        ///<Summary>
        ///Adding a field to a value or layout class is prohibitted, 
        ///</Summary>
        CORDBG_E_ENC_CANT_ADD_FIELD_TO_VALUE_OR_LAYOUT_CLASS = 0x80131338,
        ///<Summary>
        ///Returned if someone tries to call GetStaticFieldValue 
        ///</Summary>
        CORDBG_E_FIELD_NOT_STATIC = 0x8013133b,
        ///<Summary>
        ///Returned if someone tries to call GetStaticFieldValue 
        ///</Summary>
        CORDBG_E_FIELD_NOT_INSTANCE = 0x8013133c,
        ///<Summary>
        ///The JIT is unable to update the method. 
        ///</Summary>
        CORDBG_E_ENC_JIT_CANT_UPDATE = 0x8013133f,
        ///<Summary>
        ///Generic message for "Something user doesn't control went wrong" message. 
        ///</Summary>
        CORDBG_E_ENC_INTERNAL_ERROR = 0x80131341,
        ///<Summary>
        ///The field was added via EnC after the class was loaded, and so instead of the the field being contiguous with the other fields, it's 'hanging' off the instance or type.  This error is used to indicate that either the storage for this field is not yet available and so the field value cannot be read, or the debugger needs to use an EnC specific code path to get the value.
        ///</Summary>
        CORDBG_E_ENC_HANGING_FIELD = 0x80131342,
        ///<Summary>
        ///If the module isn't loaded, including if it's been unloaded. 
        ///</Summary>
        CORDBG_E_MODULE_NOT_LOADED = 0x80131343,
        ///<Summary>
        ///Can't set a breakpoint here. 
        ///</Summary>
        CORDBG_E_UNABLE_TO_SET_BREAKPOINT = 0x80131345,
        ///<Summary>
        ///Debugging isn't possible due to an incompatibility within the CLR implementation. 
        ///</Summary>
        CORDBG_E_DEBUGGING_NOT_POSSIBLE = 0x80131346,
        ///<Summary>
        ///Debugging isn't possible because a kernel debugger is enabled on the system. 
        ///</Summary>
        CORDBG_E_KERNEL_DEBUGGER_ENABLED = 0x80131347,
        ///<Summary>
        ///Debugging isn't possible because a kernel debugger is present on the system. 
        ///</Summary>
        CORDBG_E_KERNEL_DEBUGGER_PRESENT = 0x80131348,
        ///<Summary>
        ///The debugger's protocol is incompatible with the debuggee. 
        ///</Summary>
        CORDBG_E_INCOMPATIBLE_PROTOCOL = 0x8013134b,
        ///<Summary>
        ///The debugger can only handle a finite number of debuggees. 
        ///</Summary>
        CORDBG_E_TOO_MANY_PROCESSES = 0x8013134c,

        ///<Summary>
        ///Interop debugging is not supported 
        ///</Summary>
        CORDBG_E_INTEROP_NOT_SUPPORTED = 0x8013134d,
        ///<Summary>
        ///Cannot call RemapFunction until have received RemapBreakpoint 
        ///</Summary>
        CORDBG_E_NO_REMAP_BREAKPIONT = 0x8013134e,
        ///<Summary>
        ///Object has been neutered (it's in a zombie state). 
        ///</Summary>
        CORDBG_E_OBJECT_NEUTERED = 0x8013134f,
        ///<Summary>
        ///Function not yet compiled. 
        ///</Summary>
        CORPROF_E_FUNCTION_NOT_COMPILED = 0x80131350,
        ///<Summary>
        ///The ID is not fully loaded/defined yet. 
        ///</Summary>
        CORPROF_E_DATAINCOMPLETE = 0x80131351,
        ///<Summary>
        ///The Method has no associated IL 
        ///</Summary>
        CORPROF_E_FUNCTION_NOT_IL = 0x80131354,
        ///<Summary>
        ///The thread has never run managed code before 
        ///</Summary>
        CORPROF_E_NOT_MANAGED_THREAD = 0x80131355,
        ///<Summary>
        ///The function may only be called during profiler init 
        ///</Summary>
        CORPROF_E_CALL_ONLY_FROM_INIT = 0x80131356,

        ///<Summary>
        ///This is a general error used to indicated that the information 
        ///</Summary>
        CORPROF_E_NOT_YET_AVAILABLE = 0x8013135b,
        ///<Summary>
        ///The given type is a generic and cannot be used with this method. 
        ///</Summary>
        CORPROF_E_TYPE_IS_PARAMETERIZED = 0x8013135c,
        ///<Summary>
        ///The given function is a generic and cannot be used with this method. 
        ///</Summary>
        CORPROF_E_FUNCTION_IS_PARAMETERIZED = 0x8013135d,
        ///<Summary>
        ///A profiler tried to walk the stack of an invalid thread 
        ///</Summary>
        CORPROF_E_STACKSNAPSHOT_INVALID_TGT_THREAD = 0x8013135e,
        ///<Summary>
        ///A profiler can not walk a thread that is currently executing unmanaged code 
        ///</Summary>
        CORPROF_E_STACKSNAPSHOT_UNMANAGED_CTX = 0x8013135f,
        ///<Summary>
        ///A stackwalk at this point may cause dead locks or data corruption 
        ///</Summary>
        CORPROF_E_STACKSNAPSHOT_UNSAFE = 0x80131360,
        ///<Summary>
        ///Stackwalking callback requested the walk to abort 
        ///</Summary>
        CORPROF_E_STACKSNAPSHOT_ABORTED = 0x80131361,
        ///<Summary>
        ///Returned when asked for the address of a static that is a literal. 
        ///</Summary>
        CORPROF_E_LITERALS_HAVE_NO_ADDRESS = 0x80131362,
        ///<Summary>
        ///A call was made at an unsupported time.  Examples include illegally calling a profiling API method asynchronously, calling a method that might trigger a GC at an unsafe time, and calling a method at a time that could cause locks to be taken out of order.  
        ///</Summary>
        CORPROF_E_UNSUPPORTED_CALL_SEQUENCE = 0x80131363,
        ///<Summary>
        ///A legal asynchronous call was made at an unsafe time (e.g., CLR locks are held)  
        ///</Summary>
        CORPROF_E_ASYNCHRONOUS_UNSAFE = 0x80131364,
        ///<Summary>
        ///The specified ClassID cannot be inspected by this function because it is an array 
        ///</Summary>
        CORPROF_E_CLASSID_IS_ARRAY = 0x80131365,
        ///<Summary>
        ///The specified ClassID is a non-array composite type (e.g., ref) and cannot be inspected 
        ///</Summary>
        CORPROF_E_CLASSID_IS_COMPOSITE = 0x80131366,
        ///<Summary>
        ///The profiler's call into the CLR is disallowed because the profiler is attempting to detach. 
        ///</Summary>
        CORPROF_E_PROFILER_DETACHING = 0x80131367,
        ///<Summary>
        ///The profiler does not support attaching to a live process. 
        ///</Summary>
        CORPROF_E_PROFILER_NOT_ATTACHABLE = 0x80131368,
        ///<Summary>
        ///The message sent on the profiling API attach pipe is in an unrecognized format. 
        ///</Summary>
        CORPROF_E_UNRECOGNIZED_PIPE_MSG_FORMAT = 0x80131369,
        ///<Summary>
        ///The request to attach a profiler was denied because a profiler is already loaded. 
        ///</Summary>
        CORPROF_E_PROFILER_ALREADY_ACTIVE = 0x8013136A,
        ///<Summary>
        ///Unable to request a profiler attach because the target profilee's runtime is of a version incompatible with the current process calling AttachProfiler(). 
        ///</Summary>
        CORPROF_E_PROFILEE_INCOMPATIBLE_WITH_TRIGGER = 0x8013136B,
        ///<Summary>
        ///AttachProfiler() encountered an error while communicating on the pipe to the target profilee.  This is often caused by a target profilee that is shutting down or killed while AttachProfiler() is reading or writing the pipe. 
        ///</Summary>
        CORPROF_E_IPC_FAILED = 0x8013136C,
        ///<Summary>
        ///AttachProfiler() was unable to find a profilee with the specified process ID. 
        ///</Summary>
        CORPROF_E_PROFILEE_PROCESS_NOT_FOUND = 0x8013136D,
        ///<Summary>
        ///Profiler must implement ICorProfilerCallback3 interface for this call to be supported. 
        ///</Summary>
        CORPROF_E_CALLBACK3_REQUIRED = 0x8013136E,
        ///<Summary>
        ///This call was attempted by a profiler that attached to the process after startup, but this call is only supported by profilers that are loaded into the process on startup.
        ///</Summary>
        CORPROF_E_UNSUPPORTED_FOR_ATTACHING_PROFILER = 0x8013136F,


        ///<Summary>
        ///Detach is impossible because the profiler has either instrumented IL or inserted enter/leave hooks. Detach was not attempted; the profiler is still fully attached. 
        ///</Summary>
        CORPROF_E_IRREVERSIBLE_INSTRUMENTATION_PRESENT = 0x80131370,
        ///<Summary>
        ///The profiler called a function that cannot complete because the CLR is not yet fully initialized.  The profiler may try again once the CLR has fully started. 
        ///</Summary>
        CORPROF_E_RUNTIME_UNINITIALIZED = 0x80131371,
        ///<Summary>
        ///Detach is impossible because immutable flags were set by the profiler at startup. Detach was not attempted; the profiler is still fully attached. 
        ///</Summary>
        CORPROF_E_IMMUTABLE_FLAGS_SET = 0x80131372,
        ///<Summary>
        ///The profiler called a function that cannot complete because the profiler is not yet fully initialized. 
        ///</Summary>
        CORPROF_E_PROFILER_NOT_YET_INITIALIZED = 0x80131373,
        ///<Summary>
        ///The profiler called a function that first requires additional flags to be set in the event mask.  This HRESULT may also indicate that the profiler called a function that first requires that some of the flags currently set in the event mask be reset. 
        ///</Summary>
        CORPROF_E_INCONSISTENT_WITH_FLAGS = 0x80131374,
        ///<Summary>
        ///The profiler has requested that the CLR instance not load the profiler into this process. 
        ///</Summary>
        CORPROF_E_PROFILER_CANCEL_ACTIVATION = 0x80131375,
        ///<Summary>
        ///Concurrent GC mode is enabled, which prevents use of COR_PRF_MONITOR_GC 
        ///</Summary>
        CORPROF_E_CONCURRENT_GC_NOT_PROFILABLE = 0x80131376,
        ///<Summary>
        ///This functionality requires CoreCLR debugging to be enabled. 
        ///</Summary>
        CORPROF_E_DEBUGGING_DISABLED = 0x80131378,
        ///<Summary>
        ///Timed out on waiting for concurrent GC to finish during attach. 
        ///</Summary>
        CORPROF_E_TIMEOUT_WAITING_FOR_CONCURRENT_GC = 0x80131379,
        ///<Summary>
        ///The specified module was dynamically generated (e.g., via Reflection.Emit API), and is thus not supported by this API method. 
        ///</Summary>
        CORPROF_E_MODULE_IS_DYNAMIC = 0x8013137A,
        ///<Summary>
        ///Profiler must implement ICorProfilerCallback4 interface for this call to be supported. 
        ///</Summary>
        CORPROF_E_CALLBACK4_REQUIRED = 0x8013137B,
        ///<Summary>
        ///This call is not supported unless ReJIT is first enabled during initialization by setting COR_PRF_ENABLE_REJIT via SetEventMask. 
        ///</Summary>
        CORPROF_E_REJIT_NOT_ENABLED = 0x8013137C,
        ///<Summary>
        ///The specified function is instantiated into a collectible assembly, and is thus not supported by this API method. 
        ///</Summary>
        CORPROF_E_FUNCTION_IS_COLLECTIBLE = 0x8013137E,
        ///<Summary>
        ///Profiler must implement ICorProfilerCallback6 interface for this call to be supported. 
        ///</Summary>
        CORPROF_E_CALLBACK6_REQUIRED = 0x80131380,


        ///<Summary>
        ///This call can't be completed safely because the runtime is not suspended 
        ///</Summary>
        CORPROF_E_RUNTIME_SUSPEND_REQUIRED = 0x80131381,

        ///<Summary>
        ///Profiler must implement ICorProfilerCallback7 interface for this call to be supported. 
        ///</Summary>
        CORPROF_E_CALLBACK7_REQUIRED = 0x80131382,
        ///<Summary>
        ///Loading this assembly would produce a different grant set from other instances 
        ///</Summary>
        SECURITY_E_INCOMPATIBLE_SHARE = 0x80131401,
        ///<Summary>
        ///Unverifable code failed policy check 
        ///</Summary>
        SECURITY_E_UNVERIFIABLE = 0x80131402,
        ///<Summary>
        ///Assembly already loaded without additional security evidence. 
        ///</Summary>
        SECURITY_E_INCOMPATIBLE_EVIDENCE = 0x80131403,
        ///<Summary>
        ///PolicyException thrown 
        ///</Summary>
        CORSEC_E_POLICY_EXCEPTION = 0x80131416,
        ///<Summary>
        ///Failed to grant minimum permission requests 
        ///</Summary>
        CORSEC_E_MIN_GRANT_FAIL = 0x80131417,
        ///<Summary>
        ///Failed to grant permission to execute 
        ///</Summary>
        CORSEC_E_NO_EXEC_PERM = 0x80131418,
        ///<Summary>
        ///XML Syntax error 
        ///</Summary>
        CORSEC_E_XMLSYNTAX = 0x80131419,
        ///<Summary>
        ///Strong name validation failed 
        ///</Summary>
        CORSEC_E_INVALID_STRONGNAME = 0x8013141a,
        ///<Summary>
        ///Assembly is not strong named 
        ///</Summary>
        CORSEC_E_MISSING_STRONGNAME = 0x8013141b,
        ///<Summary>
        ///Invalid assembly file format 
        ///</Summary>
        CORSEC_E_INVALID_IMAGE_FORMAT = 0x8013141d,
        ///<Summary>
        ///Invalid assembly public key 
        ///</Summary>
        CORSEC_E_INVALID_PUBLICKEY = 0x8013141e,
        ///<Summary>
        ///Signature size mismatch 
        ///</Summary>
        CORSEC_E_SIGNATURE_MISMATCH = 0x80131420,
        ///<Summary>
        ///generic CryptographicException 
        ///</Summary>
        CORSEC_E_CRYPTO = 0x80131430,
        ///<Summary>
        ///generic CryptographicUnexpectedOperationException 
        ///</Summary>
        CORSEC_E_CRYPTO_UNEX_OPER = 0x80131431,
        ///<Summary>
        ///Invalid security action code 
        ///</Summary>
        CORSECATTR_E_BAD_ACTION = 0x80131442,
        ///<Summary>
        ///Base class for all exceptions in the runtime
        ///</Summary>
        COR_E_EXCEPTION = 0x80131500,
        ///<Summary>
        ///The base class for the runtime's "less serious" exceptions
        ///</Summary>
        COR_E_SYSTEM = 0x80131501,
        ///<Summary>
        ///An argument was out of its legal range.
        ///</Summary>
        COR_E_ARGUMENTOUTOFRANGE = 0x80131502,
        ///<Summary>
        ///Attempted to store an object of the wrong type in an array
        ///</Summary>
        COR_E_ARRAYTYPEMISMATCH = 0x80131503,
        ///<Summary>
        ///
        ///</Summary>
        COR_E_CONTEXTMARSHAL = 0x80131504,
        ///<Summary>
        ///
        ///</Summary>
        COR_E_TIMEOUT = 0x80131505,
        ///<Summary>
        ///An internal error happened in the Common Language Runtime's Execution Engine
        ///</Summary>
        COR_E_EXECUTIONENGINE = 0x80131506,
        ///<Summary>
        ///Access to this field is denied.
        ///</Summary>
        COR_E_FIELDACCESS = 0x80131507,
        ///<Summary>
        ///Attempted to access an element within an array by using an index that is
        ///</Summary>
        COR_E_INDEXOUTOFRANGE = 0x80131508,
        ///<Summary>
        ///An operation is not legal in the current state.
        ///</Summary>
        COR_E_INVALIDOPERATION = 0x80131509,
        ///<Summary>
        ///An error relating to security occurred.
        ///</Summary>
        COR_E_SECURITY = 0x8013150a,

        ///<Summary>
        ///An error relating to serialization has occurred.
        ///</Summary>
        COR_E_SERIALIZATION = 0x8013150c,
        ///<Summary>
        ///A verification failure occurred
        ///</Summary>
        COR_E_VERIFICATION = 0x8013150d,
        ///<Summary>
        ///Access to this method is denied.
        ///</Summary>
        COR_E_METHODACCESS = 0x80131510,
        ///<Summary>
        ///An attempt was made to dynamically access a field that does not exist.
        ///</Summary>
        COR_E_MISSINGFIELD = 0x80131511,
        ///<Summary>
        ///An attempt was made to dynamically invoke or access a field or method
        ///</Summary>
        COR_E_MISSINGMEMBER = 0x80131512,
        ///<Summary>
        ///An attempt was made to dynamically invoke a method that does not exist
        ///</Summary>
        COR_E_MISSINGMETHOD = 0x80131513,
        ///<Summary>
        ///Attempted to combine delegates that are not multicast
        ///</Summary>
        COR_E_MULTICASTNOTSUPPORTED = 0x80131514,
        ///<Summary>
        ///The operation is not supported
        ///</Summary>
        COR_E_NOTSUPPORTED = 0x80131515,
        ///<Summary>
        ///An arithmetic, casting, or conversion operation overflowed or underflowed.
        ///</Summary>
        COR_E_OVERFLOW = 0x80131516,
        ///<Summary>
        ///An array has the wrong number of dimensions for a particular operation.
        ///</Summary>
        COR_E_RANK = 0x80131517,
        ///<Summary>
        ///Wait(), Notify() or NotifyAll() was called from an unsynchronized ** block of c
        ///</Summary>
        COR_E_SYNCHRONIZATIONLOCK = 0x80131518,
        ///<Summary>
        ///Indicates that the thread was interrupted from a waiting state
        ///</Summary>
        COR_E_THREADINTERRUPTED = 0x80131519,
        ///<Summary>
        ///Access to this member is denied.
        ///</Summary>
        COR_E_MEMBERACCESS = 0x8013151a,
        ///<Summary>
        ///Indicate that the Thread class is in an invalid state for the method call
        ///</Summary>
        COR_E_THREADSTATE = 0x80131520,
        ///<Summary>
        ///Thrown into a thread to cause it to stop. This exception is typically not caught
        ///</Summary>
        COR_E_THREADSTOP = 0x80131521,
        ///<Summary>
        ///Could not find or load a specific type (class, enum, etc).
        ///</Summary>
        COR_E_TYPELOAD = 0x80131522,
        ///<Summary>
        ///Could not find the specified DllImport entry point
        ///</Summary>
        COR_E_ENTRYPOINTNOTFOUND = 0x80131523,
        ///<Summary>
        ///Could not find the specified DllImport DLL.
        ///</Summary>
        COR_E_DLLNOTFOUND = 0x80131524,
        ///<Summary>
        ///Indicate that a user thread fails to start.
        ///</Summary>
        COR_E_THREADSTART = 0x80131525,
        ///<Summary>
        ///An invalid __ComObject has been used.
        ///</Summary>
        COR_E_INVALIDCOMOBJECT = 0x80131527,
        ///<Summary>
        /// Thrown if value (a floating point number) is either the not a number value (NaN) or +- infinity value
        ///</Summary>
        COR_E_NOTFINITENUMBER = 0x80131528,
        ///<Summary>
        ///An object appears more than once in the wait objects array.
        ///</Summary>
        COR_E_DUPLICATEWAITOBJECT = 0x80131529,
        ///<Summary>
        ///Adding the given count to the semaphore would cause it to exceed its maximum count.
        ///</Summary>
        COR_E_SEMAPHOREFULL = 0x8013152b,
        ///<Summary>
        ///No Semaphore of the given name exists.
        ///</Summary>
        COR_E_WAITHANDLECANNOTBEOPENED = 0x8013152c,
        ///<Summary>
        ///The wait completed due to an abandoned mutex.
        ///</Summary>
        COR_E_ABANDONEDMUTEX = 0x8013152d,
        ///<Summary>
        ///Thrown into a thread to cause it to abort. Not catchable.
        ///</Summary>
        COR_E_THREADABORTED = 0x80131530,
        ///<Summary>
        ///The type of an OLE variant that was passed into the runtime is invalid.
        ///</Summary>
        COR_E_INVALIDOLEVARIANTTYPE = 0x80131531,
        ///<Summary>
        ///An expected resource in the assembly manifest was missing.
        ///</Summary>
        COR_E_MISSINGMANIFESTRESOURCE = 0x80131532,
        ///<Summary>
        ///A mismatch has occurred between the runtime type of the array and the subtype recorded in the metadata
        ///</Summary>
        COR_E_SAFEARRAYTYPEMISMATCH = 0x80131533,
        ///<Summary>
        ///An exception was thrown by a type's initializer (.cctor).
        ///</Summary>
        COR_E_TYPEINITIALIZATION = 0x80131534,
        ///<Summary>
        ///The marshaling directives are invalid.
        ///</Summary>
        COR_E_MARSHALDIRECTIVE = 0x80131535,
        ///<Summary>
        ///An expected satellite assembly containing the ultimate fallback resources
        ///</Summary>
        COR_E_MISSINGSATELLITEASSEMBLY = 0x80131536,
        ///<Summary>
        ///The format of one argument does not meet the contract of the method.
        ///</Summary>
        COR_E_FORMAT = 0x80131537,
        ///<Summary>
        ///A mismatch has occurred between the runtime rank of the array and the rank recorded in the metadata
        ///</Summary>
        COR_E_SAFEARRAYRANKMISMATCH = 0x80131538,
        ///<Summary>
        ///The method is not supported on this platform
        ///</Summary>
        COR_E_PLATFORMNOTSUPPORTED = 0x80131539,
        ///<Summary>
        ///A program contained invalid IL or bad metadata.  Usually this is a compiler bug.
        ///</Summary>
        COR_E_INVALIDPROGRAM = 0x8013153a,
        ///<Summary>
        ///The operation was cancelled.
        ///</Summary>
        COR_E_OPERATIONCANCELED = 0x8013153b,
        ///<Summary>
        ///Not enough memory was available for an operation.
        ///</Summary>
        COR_E_INSUFFICIENTMEMORY = 0x8013153d,
        ///<Summary>
        ///An object that does not derive from System.Exception has been wrapped in a RuntimeWrappedException.
        ///</Summary>
        COR_E_RUNTIMEWRAPPED = 0x8013153e,
        ///<Summary>
        ///A datatype misalignment was detected in a load or store instruction.
        ///</Summary>
        COR_E_DATAMISALIGNED = 0x80131541,
        ///<Summary>
        ///A managed code contract (ie, precondition, postcondition, invariant, or assert) failed.
        ///</Summary>
        COR_E_CODECONTRACTFAILED = 0x80131542,
        ///<Summary>
        ///Access to this type is denied.
        ///</Summary>
        COR_E_TYPEACCESS = 0x80131543,
        ///<Summary>
        ///Fail to access a CCW because the corresponding managed object is already collected.
        ///</Summary>
        COR_E_ACCESSING_CCW = 0x80131544,
        ///<Summary>
        ///
        ///</Summary>
        COR_E_KEYNOTFOUND = 0x80131577,
        ///<Summary>
        ///Insufficient stack to continue executing the program safely. This can happen from having too many functions on the call stack or function on the stack using too much stack space.
        ///</Summary>
        COR_E_INSUFFICIENTEXECUTIONSTACK = 0x80131578,
        ///<Summary>
        ///The base class for all "less serious" exceptions.
        ///</Summary>
        COR_E_APPLICATION = 0x80131600,
        ///<Summary>
        ///The given filter criteria does not match the filter contract.
        ///</Summary>
        COR_E_INVALIDFILTERCRITERIA = 0x80131601,
        ///<Summary>
        ///Could not find or load a specific class that was requested through Reflection
        ///</Summary>
        COR_E_REFLECTIONTYPELOAD = 0x80131602,
        ///<Summary>
        ///- If you attempt to invoke a non-static method with a null Object - If you atte
        ///</Summary>
        COR_E_TARGET = 0x80131603,
        ///<Summary>
        ///If the method called throws an exception
        ///</Summary>
        COR_E_TARGETINVOCATION = 0x80131604,
        ///<Summary>
        ///If the binary format of a custom attribute is invalid.
        ///</Summary>
        COR_E_CUSTOMATTRIBUTEFORMAT = 0x80131605,
        ///<Summary>
        ///Some sort of I/O error.
        ///</Summary>
        COR_E_IO = 0x80131620,
        ///<Summary>
        ///
        ///</Summary>
        COR_E_FILELOAD = 0x80131621,
        ///<Summary>
        ///The object has already been disposed.
        ///</Summary>
        COR_E_OBJECTDISPOSED = 0x80131622,
        ///<Summary>
        ///Runtime operation halted by call to System.Environment.FailFast().
        ///</Summary>
        COR_E_FAILFAST = 0x80131623,
        ///<Summary>
        ///Attempted to perform an operation that was forbidden by the host.
        ///</Summary>
        COR_E_HOSTPROTECTION = 0x80131640,
        ///<Summary>
        ///Attempted to call into managed code when executing inside a low level extensibility point.
        ///</Summary>
        COR_E_ILLEGAL_REENTRANCY = 0x80131641,
        ///<Summary>
        ///Failed to load the runtime 
        ///</Summary>
        CLR_E_SHIM_RUNTIMELOAD = 0x80131700,
        ///<Summary>
        ///"A runtime has already been bound for legacy activation policy use."
        ///</Summary>
        CLR_E_SHIM_LEGACYRUNTIMEALREADYBOUND = 0x80131704,
        ///<Summary>
        ///"[field sig]"
        ///</Summary>
        VER_E_FIELD_SIG = 0x80131815,
        ///<Summary>
        ///"Method parent has circular class type parameter constraints."
        ///</Summary>
        VER_E_CIRCULAR_VAR_CONSTRAINTS = 0x801318ce,
        ///<Summary>
        ///"Method has circular method type parameter constraints."
        ///</Summary>
        VER_E_CIRCULAR_MVAR_CONSTRAINTS = 0x801318cf,
        COR_E_Data = 0x80131920,
        ///<Summary>
        ///Illegal "void" in signature 
        ///</Summary>
        VLDTR_E_SIG_BADVOID = 0x80131b24,
        ///<Summary>
        ///GenericParam is a method type parameter and must be non-variant 
        ///</Summary>
        VLDTR_E_GP_ILLEGAL_VARIANT_MVAR = 0x80131b2d,
        ///<Summary>
        ///Thread is not scheduled. Thus we may not have OSThreadId, handle, or context 
        ///</Summary>
        CORDBG_E_THREAD_NOT_SCHEDULED = 0x80131c00,
        ///<Summary>
        ///Handle has been disposed. 
        ///</Summary>
        CORDBG_E_HANDLE_HAS_BEEN_DISPOSED = 0x80131c01,
        ///<Summary>
        ///Cant intercept this exception. 
        ///</Summary>
        CORDBG_E_NONINTERCEPTABLE_EXCEPTION = 0x80131c02,
        ///<Summary>
        ///The intercept frame for this exception has already been set. 
        ///</Summary>
        CORDBG_E_INTERCEPT_FRAME_ALREADY_SET = 0x80131c04,
        ///<Summary>
        ///there's no native patch at the given address. 
        ///</Summary>
        CORDBG_E_NO_NATIVE_PATCH_AT_ADDR = 0x80131c05,
        ///<Summary>
        ///This API is only allowed when interop debugging. 
        ///</Summary>
        CORDBG_E_MUST_BE_INTEROP_DEBUGGING = 0x80131c06,
        ///<Summary>
        ///There's already a native patch at the address 
        ///</Summary>
        CORDBG_E_NATIVE_PATCH_ALREADY_AT_ADDR = 0x80131c07,
        ///<Summary>
        ///a wait timed out .. likely an indication of deadlock. 
        ///</Summary>
        CORDBG_E_TIMEOUT = 0x80131c08,
        ///<Summary>
        ///Can't use the API on this thread. 
        ///</Summary>
        CORDBG_E_CANT_CALL_ON_THIS_THREAD = 0x80131c09,
        ///<Summary>
        ///Method was not JITed in EnC mode 
        ///</Summary>
        CORDBG_E_ENC_INFOLESS_METHOD = 0x80131c0a,
        ///<Summary>
        ///Method is in a callable handler/filter. Cant grow stack 
        ///</Summary>
        CORDBG_E_ENC_IN_FUNCLET = 0x80131c0c,
        ///<Summary>
        ///Attempt to perform unsupported edit 
        ///</Summary>
        CORDBG_E_ENC_EDIT_NOT_SUPPORTED = 0x80131c0e,
        ///<Summary>
        ///The LS is not in a good spot to perform the requested operation. 
        ///</Summary>
        CORDBG_E_NOTREADY = 0x80131c10,
        ///<Summary>
        ///We failed to resolve assembly given an AssemblyRef token. Assembly may be not loaded yet or not a valid token. 
        ///</Summary>
        CORDBG_E_CANNOT_RESOLVE_ASSEMBLY = 0x80131c11,
        ///<Summary>
        ///Must be in context of LoadModule callback to perform requested operation 
        ///</Summary>
        CORDBG_E_MUST_BE_IN_LOAD_MODULE = 0x80131c12,
        ///<Summary>
        ///Requested operation cannot be performed during an attach operation 
        ///</Summary>
        CORDBG_E_CANNOT_BE_ON_ATTACH = 0x80131c13,
        ///<Summary>
        ///NGEN must be supported to perform the requested operation 
        ///</Summary>
        CORDBG_E_NGEN_NOT_SUPPORTED = 0x80131c14,
        ///<Summary>
        ///Trying to shutdown out of order. 
        ///</Summary>
        CORDBG_E_ILLEGAL_SHUTDOWN_ORDER = 0x80131c15,
        ///<Summary>
        ///For Whidbey, we don't support debugging fiber mode managed process 
        ///</Summary>
        CORDBG_E_CANNOT_DEBUG_FIBER_PROCESS = 0x80131c16,
        ///<Summary>
        ///Must be in context of CreateProcess callback to perform requested operation 
        ///</Summary>
        CORDBG_E_MUST_BE_IN_CREATE_PROCESS = 0x80131c17,
        ///<Summary>
        ///All outstanding func-evals have not completed, detaching is not allowed at this time. 
        ///</Summary>
        CORDBG_E_DETACH_FAILED_OUTSTANDING_EVALS = 0x80131c18,
        ///<Summary>
        ///All outstanding steppers have not been closed, detaching is not allowed at this time. 
        ///</Summary>
        CORDBG_E_DETACH_FAILED_OUTSTANDING_STEPPERS = 0x80131c19,
        ///<Summary>
        ///Can't have an ICorDebugStepper do a native step-out. 
        ///</Summary>
        CORDBG_E_CANT_INTEROP_STEP_OUT = 0x80131c20,
        ///<Summary>
        ///All outstanding breakpoints have not been closed, detaching is not allowed at this time. 
        ///</Summary>
        CORDBG_E_DETACH_FAILED_OUTSTANDING_BREAKPOINTS = 0x80131c21,
        ///<Summary>
        ///the operation is illegal because of a stackoverflow. 
        ///</Summary>
        CORDBG_E_ILLEGAL_IN_STACK_OVERFLOW = 0x80131c22,
        ///<Summary>
        ///The operation failed because it's a GC unsafe point. 
        ///</Summary>
        CORDBG_E_ILLEGAL_AT_GC_UNSAFE_POINT = 0x80131c23,
        ///<Summary>
        ///The operation failed because the thread is in the prolog 
        ///</Summary>
        CORDBG_E_ILLEGAL_IN_PROLOG = 0x80131c24,
        ///<Summary>
        ///The operation failed because the thread is in native code 
        ///</Summary>
        CORDBG_E_ILLEGAL_IN_NATIVE_CODE = 0x80131c25,
        ///<Summary>
        ///The operation failed because the thread is in optimized code. 
        ///</Summary>
        CORDBG_E_ILLEGAL_IN_OPTIMIZED_CODE = 0x80131c26,
        ///<Summary>
        ///A supplied object or type belongs to the wrong AppDomain 
        ///</Summary>
        CORDBG_E_APPDOMAIN_MISMATCH = 0x80131c28,
        ///<Summary>
        ///The thread's context is not available. 
        ///</Summary>
        CORDBG_E_CONTEXT_UNVAILABLE = 0x80131c29,
        ///<Summary>
        ///The operation failed because debuggee and debugger are on incompatible platform 
        ///</Summary>
        CORDBG_E_UNCOMPATIBLE_PLATFORMS = 0x80131c30,
        ///<Summary>
        ///The operation failed because the debugging has been disabled 
        ///</Summary>
        CORDBG_E_DEBUGGING_DISABLED = 0x80131c31,
        ///<Summary>
        ///Detach is illegal after a module has been EnCed. 
        ///</Summary>
        CORDBG_E_DETACH_FAILED_ON_ENC = 0x80131c32,
        ///<Summary>
        ///Interception of the current exception is not legal 
        ///</Summary>
        CORDBG_E_CURRENT_EXCEPTION_IS_OUTSIDE_CURRENT_EXECUTION_SCOPE = 0x80131c33,
        ///<Summary>
        ///Helper thread can not safely run code. The opereration may work at a later time. 
        ///</Summary>
        CORDBG_E_HELPER_MAY_DEADLOCK = 0x80131c34,
        ///<Summary>
        ///The operation failed because the debugger could not get the metadata.
        ///</Summary>
        CORDBG_E_MISSING_METADATA = 0x80131c35,
        ///<Summary>
        ///The debuggee is in a corrupt state.
        ///</Summary>
        CORDBG_E_TARGET_INCONSISTENT = 0x80131c36,
        ///<Summary>
        ///The debugger is holding resource in the target (such as GC handles, function evaluations, etc). 
        //  These resources must be released through the appropriate ICorDebug API before detach can succeed.
        ///</Summary>
        CORDBG_E_DETACH_FAILED_OUTSTANDING_TARGET_RESOURCES = 0x80131c37,
        ///<Summary>
        ///The provided ICorDebugDataTarget does not implement ICorDebugMutableDataTarget.
        ///</Summary>
        CORDBG_E_TARGET_READONLY = 0x80131c38,
        ///<Summary>
        ///A clr/mscordacwks mismatch will cause DAC to fail to initialize in ClrDataAccess::Initialize
        ///</Summary>
        CORDBG_E_MISMATCHED_CORWKS_AND_DACWKS_DLLS = 0x80131c39,
        ///<Summary>
        ///Symbols are not supplied for modules loaded from disk
        ///</Summary>
        CORDBG_E_MODULE_LOADED_FROM_DISK = 0x80131c3a,
        ///<Summary>
        ///The application did not supply symbols when it loaded or created this module, or they are not yet available
        ///</Summary>
        CORDBG_E_SYMBOLS_NOT_AVAILABLE = 0x80131c3b,
        ///<Summary>
        ///A debug component is not installed
        ///</Summary>
        CORDBG_E_DEBUG_COMPONENT_MISSING = 0x80131c3c,
        ///<Summary>
        ///The ICLRDebuggingLibraryProvider callback returned an error or did not provide a valid handle
        ///</Summary>
        CORDBG_E_LIBRARY_PROVIDER_ERROR = 0x80131c43,
        ///<Summary>
        ///The module at the base address indicated was not recognized as a CLR
        ///</Summary>
        CORDBG_E_NOT_CLR = 0x80131c44,
        ///<Summary>
        ///The provided data target does not implement the required interfaces for this version of the runtime
        ///</Summary>
        CORDBG_E_MISSING_DATA_TARGET_INTERFACE = 0x80131c45,
        ///<Summary>
        ///This debugging model is unsupported by the specified runtime
        ///</Summary>
        CORDBG_E_UNSUPPORTED_DEBUGGING_MODEL = 0x80131c46,
        ///<Summary>
        ///The debugger is not designed to support the version of the CLR the debuggee is using.
        ///</Summary>
        CORDBG_E_UNSUPPORTED_FORWARD_COMPAT = 0x80131c47,
        ///<Summary>
        ///The version struct has an unrecognized value for wStructVersion
        ///</Summary>
        CORDBG_E_UNSUPPORTED_VERSION_STRUCT = 0x80131c48,
        ///<Summary>
        ///A call into a ReadVirtual implementation returned failure
        ///</Summary>
        CORDBG_E_READVIRTUAL_FAILURE = 0x80131c49,
        ///<Summary>
        ///The Debugging API doesn't support dereferencing function pointers.
        ///</Summary>
        CORDBG_E_VALUE_POINTS_TO_FUNCTION = 0x80131c4a,
        ///<Summary>
        ///The address provided does not point to a valid managed object.
        ///</Summary>
        CORDBG_E_CORRUPT_OBJECT = 0x80131c4b,
        ///<Summary>
        ///The GC heap structures are not in a valid state for traversal.
        ///</Summary>
        CORDBG_E_GC_STRUCTURES_INVALID = 0x80131c4c,
        ///<Summary>
        ///The specified IL offset or opcode is not supported for this operation.
        ///</Summary>
        CORDBG_E_INVALID_OPCODE = 0x80131c4d,
        ///<Summary>
        ///The specified action is unsupported by this version of the runtime.
        ///</Summary>
        CORDBG_E_UNSUPPORTED = 0x80131c4e,
        ///<Summary>
        ///The debuggee memory space does not have the expected debugging export table.
        ///</Summary>
        CORDBG_E_MISSING_DEBUGGER_EXPORTS = 0x80131c4f,
        ///<Summary>
        ///Failure when calling a data target method.
        ///</Summary>
        CORDBG_E_DATA_TARGET_ERROR = 0x80131c61,
        ///<Summary>
        ///Couldn't find a native image.
        ///</Summary>
        CORDBG_E_NO_IMAGE_AVAILABLE = 0x80131c64,
        ///<Summary>
        ///File is PE32+ 
        ///</Summary>
        PEFMT_E_64BIT = 0x80131d02,
        ///<Summary>
        ///File is PE32 
        ///</Summary>
        PEFMT_E_32BIT = 0x80131d0b,
        ///<Summary>
        ///Compiling any assembly other than mscorlib in the absence of mscorlib.ni.dll is not allowed.
        ///</Summary>
        NGEN_E_SYS_ASM_NI_MISSING = 0x80131f06,
        CLDB_E_INTERNALERROR = 0x80131fff,
        ///<Summary>
        ///For AppX binder, indicates that bound assembly has a version lower than that requested, and we will refuse version rollback.
        ///</Summary>
        CLR_E_BIND_ASSEMBLY_VERSION_TOO_LOW = 0x80132000,
        ///<Summary>
        ///For AppX binder, indicates that bound assembly's public key token doesn't match the key in the assembly name.
        ///</Summary>
        CLR_E_BIND_ASSEMBLY_PUBLIC_KEY_MISMATCH = 0x80132001,
        ///<Summary>
        ///Occurs if a request for a native image is made on an ICLRPrivAssembly interface when one is not available.
        ///</Summary>
        CLR_E_BIND_IMAGE_UNAVAILABLE = 0x80132002,
        ///<Summary>
        ///If a binder is provided an identity format that it cannot parse, it returns this error.
        ///</Summary>
        CLR_E_BIND_UNRECOGNIZED_IDENTITY_FORMAT = 0x80132003,
        ///<Summary>
        ///Returned by binders that bind based on assembly identity.
        ///</Summary>
        CLR_E_BIND_ASSEMBLY_NOT_FOUND = 0x80132004,
        ///<Summary>
        ///Returned by binders that bind based on type identity.
        ///</Summary>
        CLR_E_BIND_TYPE_NOT_FOUND = 0x80132005,
        ///<Summary>
        ///Returned when loading an assembly that only has a native image and no IL and cannot hardbind to mscorlib.ni.dll.
        ///</Summary>
        CLR_E_BIND_SYS_ASM_NI_MISSING = 0x80132006,
        ///<Summary>
        ///Returned when an assembly is NGened in full trust and then used in partial trust or vice versa.
        ///</Summary>
        CLR_E_BIND_NI_SECURITY_FAILURE = 0x80132007,
        ///<Summary>
        ///Returned when an assembly's identities have changed so the native image is no longer valid.
        ///</Summary>
        CLR_E_BIND_NI_DEP_IDENTITY_MISMATCH = 0x80132008,
        ///<Summary>
        ///During a GC when we try to allocate memory for GC datastructures we could not.
        ///</Summary>
        CLR_E_GC_OOM = 0x80132009,
        ///<Summary>
        ///Access is denied.
        ///</Summary>
        COR_E_UNAUTHORIZEDACCESS = 0x80070005,
        ///<Summary>
        ///An argument does not meet the contract of the method.
        ///</Summary>
        COR_E_ARGUMENT = 0x80070057,
        ///<Summary>
        ///Indicates a bad cast condition
        ///</Summary>
        COR_E_INVALIDCAST = 0x80004002,
        ///<Summary>
        ///The EE thows this exception when no more memory is avaible to continue execution
        ///</Summary>
        COR_E_OUTOFMEMORY = 0x8007000E,
        ///<Summary>
        ///Dereferencing a null reference. In general class libraries should not throw this
        ///</Summary>
        COR_E_NULLREFERENCE = 0x80004003,
        ///<Summary>
        ///While late binding to a method via reflection, could not resolve between
        ///</Summary>
        COR_E_AMBIGUOUSMATCH = 0x8000211D,
        ///<Summary>
        ///DISP_E_BADPARAMCOUNT // There was a mismatch between number of arguments provided and the number expected
        ///</Summary>
        COR_E_TARGETPARAMCOUNT = 0x8002000E,
        ///<Summary>
        ///DISP_E_DIVBYZERO // Attempted to divide a number by zero.
        ///</Summary>
        COR_E_DIVIDEBYZERO = 0x80020012,
        ///<Summary>
        ///The format of DLL or executable being loaded is invalid.
        ///</Summary>
        COR_E_BADIMAGEFORMAT = 0x8007000B


    }
}
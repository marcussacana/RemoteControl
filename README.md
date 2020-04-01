
# RemoteControl

A Compact Library to do simple operations in a external process.

**Features:**
- Inject Umanaged DLL's
- Invoke any DLL Export
- Inject a Managed Assembly
- Can Inject before the target process execution begins
- Can read, write and release data in the process memory
- Supports x64 and x86 Applications
- Hook Unmanaged Exports
- And others minor but cool features...
- Everything without extras files! you can merge the RemoteControl into your assembly and don't will see any dll!

### See the example project

#### Invoking a DLL Export
```csharp
RemoteControl Control = new RemoteControl("C:\\Windows\\System32\\Notepad.exe", out Process Notepad);
//Wait the minimal process startup to the RemoteControl works
Control.WaitInitialize();
Control.WaitModuleLoad("user32.dll");//Since the notepad already load this dll, it's better wait it.

//Without this the RemoteControl can't invoke functions in the target process
//Lock the entry point and remove the suspended state of the process 
Control.LockEntryPoint();


//Alloc the string in target process
var Message = Notepad.MAllocString("Wow, I'm called in the target process from the Example!", true);
var Title = Notepad.MAllocString("This is a test", true);


//If the target process don't have loaded the user32.dll, he will be automatically loaded!
//int MessageBoxW(HWND hWnd, LPCWSTR lpText, LPCWSTR lpCaption, UINT uType);
var Rst = Control.Invoke("user32.dll", "MessageBoxW", IntPtr.Zero, Message, Title, new IntPtr(0x20 | 0x04));//0x20 = MB_ICONQUESTION, 0x04 = MB_YESNO
switch (Rst.ToInt32()){
	case 1:
		Console.WriteLine("OK");
		break;
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

//Dispose the allocated string
Notepad.MFree(Message);
Notepad.MFree(Title);

//Allows the process continues his startup
Control.UnlockEntryPoint();

Console.WriteLine("Press any key to exit");
Console.ReadKey();
```
![image](https://user-images.githubusercontent.com/10576957/78092167-620c6900-73a5-11ea-9797-d34804683d53.png)
### Injecting a Managed Assembly
```csharp
public static int EntryPoint(string Arg) {
	MessageBox.Show(Arg, "Assembly Injected!", MessageBoxButtons.OK, MessageBoxIcon.Information);
	return int.MaxValue;
}

public static void Inject(){
RemoteControl Control = new RemoteControl("C:\\Windows\\System32\\Notepad.exe", out Process Notepad);
//Wait the minimal process startup to the RemoteControl works
Control.WaitInitialize();

//Without this the RemoteControl can't invoke functions in the target process
//Lock the entry point and remove the suspended state of the process 
Control.LockEntryPoint();

string CurrentAssembly = Assembly.GetExecutingAssembly().Location;
string Message = "LOL, I'm a managed dll inside of the target process!";


//If you have only one method like 'public static int XXXX(string Arg)' you don't need give the Injection EntryPoint
int Ret = Control.CLRInvoke(CurrentAssembly, Message);


//If you have more than one method like that, you need specify it
//Since "EntryPoint" is inside the "Program" class, we use typeof(Program).FullName
//int Ret = Control.CLRInvoke(CurrentAssembly, typeof(Program).FullName, "EntryPoint", Message);


//Allows the process continues his startup
Control.UnlockEntryPoint();
}
```
![image](https://user-images.githubusercontent.com/10576957/78092187-7badb080-73a5-11ea-8e26-9b4e2e9c44ed.png)

### Extra

[PInvoke Tools](https://github.com/dahall/Vanara)


### By Marcussacana

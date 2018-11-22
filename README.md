
# RemoteControl

A Compact Library to do simple operations in a external process.

**Features:**
- Inject Umanaged DLL's
- Invoke any DLL Export
- Inject a Managed Assembly
- Can Inject before the target process execution begins
- Can read, write and release data in the process memory
- Supports x64 and x86 Applications
- And others minor but cool features...
- Everything without extras files! you can merge the RemoteControl into your assembly and don't will see any dll!

### See the example project

#### Invoking a DLL Export
```csharp
RemoteControl Control = new RemoteControl("C:\\Windows\\System32\\Notepad.exe", out Process Notepad);
//Wait the minimal process startup to the RemoteControl works
Control.WaitInitialize();
Control.WaitModuleLoad("user32.dll");//Since the notepad already load this dll, it's better wait it.

//Alloc the string in target process
var Message = Notepad.AllocString("Wow, I'm called in the target process from the Example!", true);
var Title = Notepad.AllocString("This is a test", true);

//Allows the process continues his startup
Control.ResumeProcess();

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
Console.WriteLine("Press any key to exit");
Console.ReadKey();
```
![https://a.doko.moe/dqhajo.png](https://a.doko.moe/dqhajo.png)
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
//Allows the process continues his startup
Control.ResumeProcess();

string CurrentAssembly = Assembly.GetExecutingAssembly().Location;
string Message = "LOL, I'm a managed dll inside of the target process!";

//If you have only one method like 'public static int XXXX(string Arg)' you don't need give the Injection EntryPoint
int Ret = Control.CLRInvoke(CurrentAssembly, Message);
//If you have more than one method like that, you need specify it
//Since "EntryPoint" is inside the "Program" class, we use typeof(Program).FullName
Ret = Control.CLRInvoke(CurrentAssembly, typeof(Program).FullName, "EntryPoint", Message);
}
```
![https://a.doko.moe/ikwzem.png](https://a.doko.moe/ikwzem.png)

### Extra

[Unmanaged hooking from Managed Injected Assembly](https://github.com/marcussacana/StringReloads/blob/master/SRL/Hook/HookFX.cs)  
[PInvoke Tools](https://github.com/dahall/Vanara)


### By Marcussacana

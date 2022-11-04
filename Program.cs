using System;
using System.IO;
using System.Runtime.InteropServices;

string argv0 = null;

#if NETFRAMEWORK
argv0 = GetWindowsArgv0();
#else
if (OperatingSystem.IsWindows())
{
	argv0 = GetWindowsArgv0();
}
else if (OperatingSystem.IsMacOS())
{
	argv0 = GetMacOSArgv0();
}
else if (OperatingSystem.IsLinux())
{
	argv0 = GetLinuxArgv0();
}
#endif

Console.WriteLine(argv0);

static string GetLinuxArgv0()
{
	string selfExe = File.ReadAllText("/proc/self/exe");
	return selfExe;
}

static string GetMacOSArgv0()
{
	int size;
	_NSGetExecutablePath(IntPtr.Zero, out size);

	IntPtr bufPtr = Marshal.AllocHGlobal(size);
	int result = _NSGetExecutablePath(bufPtr, out size);

	string name = result == 0 ? Marshal.PtrToStringAuto(bufPtr, size) : null;

	Marshal.FreeHGlobal(bufPtr);
	
	return name;
}

static string GetWindowsArgv0()
{
	IntPtr argvPtr = CommandLineToArgvW(GetCommandLine(), out _);
	IntPtr argv0Ptr = Marshal.ReadIntPtr(argvPtr);
	return Marshal.PtrToStringAuto(argv0Ptr);
}

[DllImport("libc", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
static extern int _NSGetExecutablePath(IntPtr buf, out int bufsize);

[DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
static extern IntPtr GetCommandLine();

[DllImport("Shell32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
static extern IntPtr CommandLineToArgvW(IntPtr lpCmdLine, out int pNumArgs);

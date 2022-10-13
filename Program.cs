using System.Diagnostics;
using System.Runtime.InteropServices;

string? argv0 = null;

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

argv0 ??= Process.GetCurrentProcess().MainModule?.FileName
	?? Environment.GetCommandLineArgs()[0];

Console.WriteLine(argv0);

static string? GetLinuxArgv0()
{
	string cmdline = File.ReadAllText("/proc/self/cmdline");
	return cmdline.Split('\0')[0];
}

static string? GetMacOSArgv0()
{
	IntPtr ptr = _NSGetArgv();
	IntPtr argvPtr = Marshal.ReadIntPtr(ptr);
	IntPtr argv0Ptr = Marshal.ReadIntPtr(argvPtr);
	return Marshal.PtrToStringAnsi(argv0Ptr);
}

static string? GetWindowsArgv0()
{
	IntPtr argvPtr = CommandLineToArgvW(GetCommandLine(), out _);
	IntPtr argv0Ptr = Marshal.ReadIntPtr(argvPtr);
	return Marshal.PtrToStringAuto(argv0Ptr);
}

[DllImport("libc", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
static extern IntPtr _NSGetArgv();

[DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
static extern IntPtr GetCommandLine();

[DllImport("Shell32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
static extern IntPtr CommandLineToArgvW(IntPtr lpCmdLine, out int pNumArgs);

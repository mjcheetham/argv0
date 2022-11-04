using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

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
	string cmdline = File.ReadAllText("/proc/self/cmdline");
	return cmdline.Split('\0')[0];
}

const int MAXPATHLEN = 1024;
const int PROC_PIDPATHINFO_MAXSIZE = 4 * MAXPATHLEN;

static unsafe string GetMacOSArgv0()
{
	var pid = Process.GetCurrentProcess().Id;
	int result = 0;
	byte* pBuffer = stackalloc byte[PROC_PIDPATHINFO_MAXSIZE];
	result = proc_pidpath(pid, pBuffer, (uint)(PROC_PIDPATHINFO_MAXSIZE * sizeof(byte)));
	if (result <= 0)
	{
		return null;
	}

	return Encoding.UTF8.GetString(pBuffer, result);
}

static string GetWindowsArgv0()
{
	IntPtr argvPtr = CommandLineToArgvW(GetCommandLine(), out _);
	IntPtr argv0Ptr = Marshal.ReadIntPtr(argvPtr);
	return Marshal.PtrToStringAuto(argv0Ptr);
}

[DllImport("/usr/lib/libproc.dylib", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
static extern unsafe int proc_pidpath(int pid, byte* buffer, uint bufferSize);

[DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
static extern IntPtr GetCommandLine();

[DllImport("Shell32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
static extern IntPtr CommandLineToArgvW(IntPtr lpCmdLine, out int pNumArgs);

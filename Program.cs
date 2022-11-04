using System;
using System.IO;
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

static unsafe string GetLinuxArgv0()
{
	const int PATH_MAX = 4096;
	byte[] pathBytes = Encoding.UTF8.GetBytes("/proc/self/exe");
	IntPtr buf = Marshal.AllocHGlobal(PATH_MAX);
	int len = readlink(pathBytes, buf, PATH_MAX);
	
	string path = null;
	if (len > 0)
	{
		path = Marshal.PtrToStringAuto(buf, len);
	}

	Marshal.FreeHGlobal(buf);
	return path;
}

static string GetMacOSArgv0()
{
	int size;
	_NSGetExecutablePath(IntPtr.Zero, out size);

	IntPtr bufPtr = Marshal.AllocHGlobal(size);
	int result = _NSGetExecutablePath(bufPtr, out size);

	string path = result == 0 ? Marshal.PtrToStringAuto(bufPtr, size) : null;

	Marshal.FreeHGlobal(bufPtr);
	
	return path;
}

static string GetWindowsArgv0()
{
	IntPtr argvPtr = CommandLineToArgvW(GetCommandLine(), out _);
	IntPtr argv0Ptr = Marshal.ReadIntPtr(argvPtr);
	return Marshal.PtrToStringAuto(argv0Ptr);
}

[DllImport("libc", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
static extern int readlink(byte[] pathname, IntPtr buf, int bufsiz);

[DllImport("libc", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
static extern int _NSGetExecutablePath(IntPtr buf, out int bufsize);

[DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
static extern IntPtr GetCommandLine();

[DllImport("Shell32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
static extern IntPtr CommandLineToArgvW(IntPtr lpCmdLine, out int pNumArgs);

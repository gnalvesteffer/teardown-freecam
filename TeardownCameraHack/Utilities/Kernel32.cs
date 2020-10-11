using System;
using System.Runtime.InteropServices;

namespace TeardownCameraHack.Utilities
{
    public class Kernel32
    {
        [DllImport("kernel32.dll")]
        internal static extern IntPtr CreateRemoteThread(
            IntPtr hProcess,
            IntPtr lpThreadAttributes,
            uint dwStackSize,
            IntPtr lpStartAddress,
            IntPtr lpParameter,
            uint dwCreationFlags,
            IntPtr lpThreadId
        );
    }
}

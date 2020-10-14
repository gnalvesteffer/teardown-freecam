using Squalr.Engine.Memory;
using Squalr.Engine.OS;
using TeardownCameraHack.Utilities;

namespace TeardownCameraHack.Teardown
{
    public static class TeardownApi
    {
        public static void ExitLevel() // wip, proof of concept
        {
            var allocatedAddress = Allocator.Default.AllocateMemory(1);
            Writer.Default.Write(allocatedAddress, 7);
            AssemblyUtility.CallFunction((ulong)Processes.Default.OpenedProcess.MainModule.BaseAddress + 0x30890, Reader.Default.Read<ulong>((ulong)Processes.Default.OpenedProcess.MainModule.BaseAddress + 0x003E2528, out _), allocatedAddress);
            // maybe this can get the function's return value https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-getexitcodethread
        }
    }
}

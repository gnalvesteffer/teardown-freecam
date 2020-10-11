using System;
using System.Collections.Generic;
using Squalr.Engine.Memory;
using Squalr.Engine.OS;
using Squalr.Engine.Utils.Extensions;

namespace TeardownCameraHack.Utilities
{
    public static class AssemblyUtility
    {
        internal static void CallFunction(ulong functionAddress, params ulong[] parameters)
        {
            var shellCode = AssembleFunctionCallX64(functionAddress, parameters);
            var codeInjectionAddress = Allocator.Default.AllocateMemory(shellCode.Length);
            Writer.Default.WriteBytes(codeInjectionAddress, shellCode);
            Kernel32.CreateRemoteThread(
                Processes.Default.OpenedProcess.Handle,
                IntPtr.Zero,
                0,
                codeInjectionAddress.ToIntPtr(),
                IntPtr.Zero,
                0,
                IntPtr.Zero
            );
        }

        /// <summary>
        /// credits: https://guidedhacking.com/threads/function-call-assembler.12584/
        /// Calls a subroutine with the fastcall convention.
        /// </summary>
        /// <param name="functionAddress"></param>
        /// <param name="parameters">addresses for rcx, rdx, etc</param>
        /// <returns></returns>
        internal static byte[] AssembleFunctionCallX64(ulong functionAddress, params ulong[] parameters)
        {
            var shellcode = new List<byte>();

            // sub rsp, 0x28

            shellcode.AddRange(new byte[] { 0x48, 0x83, 0xEc, 0x28 });

            for (var index = 0; index < parameters.Length; index += 1)
            {
                switch (index)
                {
                    case 0:
                    {
                        // mov rcx, parameter

                        shellcode.AddRange(new byte[] { 0x48, 0xB9 });

                        shellcode.AddRange(BitConverter.GetBytes(parameters[index]));

                        break;
                    }

                    case 1:
                    {
                        // mov rdx, parameter

                        shellcode.AddRange(new byte[] { 0x48, 0xBA });

                        shellcode.AddRange(BitConverter.GetBytes(parameters[index]));

                        break;
                    }

                    case 2:
                    {
                        // mov r8, parameter

                        shellcode.AddRange(new byte[] { 0x49, 0xB8 });

                        shellcode.AddRange(BitConverter.GetBytes(parameters[index]));

                        break;
                    }

                    case 3:
                    {
                        // mov r9, parameter

                        shellcode.AddRange(new byte[] { 0x49, 0xB9 });

                        shellcode.AddRange(BitConverter.GetBytes(parameters[index]));

                        break;
                    }
                }
            }

            // mov rax, functionAddress

            shellcode.AddRange(new byte[] { 0x48, 0xB8 });

            shellcode.AddRange(BitConverter.GetBytes(functionAddress));

            // call rax

            shellcode.AddRange(new byte[] { 0xFF, 0xD0 });

            // xor eax, eax

            shellcode.AddRange(new byte[] { 0x31, 0xC0 });

            // add rsp, 0x28

            shellcode.AddRange(new byte[] { 0x48, 0x83, 0xC4, 0x28 });

            // ret

            shellcode.Add(0xC3);

            return shellcode.ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static ProcessMemory.PInvoke;

namespace MemCore
{
    public unsafe class MemoryHandler : IDisposable
    {
        private const int STILL_ACTIVE = 259;
        public readonly IntPtr ProcessHandle = IntPtr.Zero;
        
        public MemoryHandler(int pid, bool readOnly = true)
        {
            var accessFlags = readOnly 
                ? ProcessAccessFlags.QueryInformation | ProcessAccessFlags.VirtualMemoryRead 
                : ProcessAccessFlags.QueryInformation | ProcessAccessFlags.VirtualMemoryRead | ProcessAccessFlags.VirtualMemoryWrite;
                
            ProcessHandle = OpenProcess(accessFlags, false, pid);
        }

        public bool ProcessRunning
        {
            get 
            { 
                int exitCode = 0;
                bool success = GetExitCodeProcess(ProcessHandle, ref exitCode);
                return success && exitCode == STILL_ACTIVE;
            }
        }

        public bool ProcessExitCode
        {
            get
            {
                int exitCode = 0;
                GetExitCodeProcess(ProcessHandle, ref exitCode);
                return exitCode;
            }
        }

        private static readonly int memBasicInfoSize = sizeof(MEMORY_BASIC_INFORMATION32);

        private string GetMemProtectFlags(IntPtr offset)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                MEMORY_BASIC_INFORMATION32 memBasicInfo = new MEMORY_BASIC_INFORMATION32();
                VirtualQueryEx(ProcessHandle, offset, out memBasicInfo, memBasicInfoSize);

                sb.AppendLine("[MEMORY_BASIC_INFORMATION32]")
                    .AppendLine($"BaseAddress: {memBasicInfo.BaseAddress}\r\n")
                    .AppendLine($"AllocationBase: {memBasicInfo.AllocationBase}\r\n")
                    .AppendLine($"AllocationProtect: {memBasicInfo.AllocationProtect}\r\n")
                    .AppendLine($"RegionSize: {memBasicInfo.RegionSize}\r\n")
                    .AppendLine($"State: {memBasicInfo.State}\r\n")
                    .AppendLine($"Protect: {memBasicInfo.Protect}\r\n")
                    .AppendLine($"Type: {memBasicInfo.Type}\r\n");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"[GetMemoryProtectFlags EXCEPTION: {ex.ToString()}]";
            }
        }

        // Generic
        public bool TryGetAt<T>(IntPtr offset, ref T value) where T : unmanaged
        {
            fixed (T* pointer = &value)
                return ReadProcessMemory(ProcessHandle, offset, pointer, sizeof(T), out IntPtr bytesRead);
        }
        public bool TryGetAt<T>(int* offset, ref T value) where T : unmanaged
        {
            fixed (T* pointer = &value)
                return ReadProcessMemory(ProcessHandle, offset, pointer, sizeof(T), out IntPtr bytesRead);
        }

        public bool TrySetAt<T>(IntPtr offset, ref T value) where T : unmanaged
        {
            fixed (T* pointer = &value)
                return WriteProcessMemory(ProcessHandle, offset, pointer, sizeof(T), out IntPtr bytesRead);
        }
        public bool TrySetAt<T>(int* offset, ref T value) where T : unmanaged
        {
            fixed (T* pointer = &value)
                return WriteProcessMemory(ProcessHandle, offset, pointer, sizeof(T), out IntPtr bytesRead);
        }

        // Byte Array
        public bool TryGetByteArrayAt(IntPtr offset, int size, IntPtr result)
            => ReadProcessMemory(ProcessHandle, offset, result, size, out IntPtr _);
        public bool TryGetByteArrayAt(int* offset, int size, IntPtr result) 
            => TryGetByteArrayAt((IntPtr)offset, size, result);
        public bool TryGetByteArrayAt(IntPtr offset, int size, void* result)
            => ReadProcessMemory(ProcessHandle, offset, result, size, out IntPtr _);
        public bool TryGetByteArrayAt(int* offset, int size, void* result)
            => TryGetByteArrayAt((IntPtr)offset, size, result);

        public bool TrySetByteArrayAt(IntPtr offset, int size, IntPtr result)
            => WriteProcessMemory(ProcessHandle, offset, result, size, out IntPtr _);
        public bool TrySetByteArrayAt(int* offset, int size, IntPtr result)
            => TrySetByteArrayAt((IntPtr)offset, size, result);
        public bool TrySetByteArrayAt(IntPtr offset, int size, void* result)
            => WriteProcessMemory(ProcessHandle, offset, result, size, out IntPtr _);
        public bool TrySetByteArrayAt(int* offset, int size, void* result)
            => TrySetByteArrayAt((IntPtr)offset, size, result);
    }
}
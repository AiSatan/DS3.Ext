using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace AutoDeathCounter
{
    /// <summary>
    /// https://stackoverflow.com/a/50672487/3569212
    /// with "write" disabled
    /// </summary>
    internal static class MemoryManager
    {
        private static Process m_iProcess;
        private static IntPtr m_iProcessHandle;

        private static int m_iBytesWritten;
        private static int m_iBytesRead;

        public static bool Attatch(string ProcName)
        {
            if (Process.GetProcessesByName(ProcName).Length > 0)
            {
                m_iProcess = Process.GetProcessesByName(ProcName)[0];
                m_iProcessHandle = OpenProcess(Flags.PROCESS_VM_OPERATION | Flags.PROCESS_VM_READ, false, m_iProcess.Id);
                return true;
            }

            return false;
        }

        public static void Detach()
        {
            CloseHandle(m_iProcessHandle);
        }

        public static void WriteMemory<T>(int Address, object Value)
        {
            throw new AccessViolationException();
            var buffer = StructureToByteArray(Value);

            NtWriteVirtualMemory((int)m_iProcessHandle, Address, buffer, buffer.Length, out m_iBytesWritten);
        }

        public static void WriteMemory<T>(int Adress, char[] Value)
        {
            throw new AccessViolationException();
            var buffer = Encoding.UTF8.GetBytes(Value);

            NtWriteVirtualMemory((int)m_iProcessHandle, Adress, buffer, buffer.Length, out m_iBytesWritten);
        }

        public static T ReadMemory<T>(int address) where T : struct
        {
            return ReadMemory<T>(new IntPtr(address));
        }

        public static T ReadMemory<T>(long address) where T : struct
        {
            return ReadMemory<T>(new IntPtr(address));
        }

        public static T ReadMemory<T>(IntPtr address) where T : struct
        {
            var ByteSize = Marshal.SizeOf(typeof(T));

            var buffer = new byte[ByteSize];

            NtReadVirtualMemory((int)m_iProcessHandle, address, buffer, buffer.Length, ref m_iBytesRead);

            return ByteArrayToStructure<T>(buffer);
        }

        public static byte[] ReadMemory(int offset, int size)
        {
            var buffer = new byte[size];

            NtReadVirtualMemory((int)m_iProcessHandle, (IntPtr)offset, buffer, size, ref m_iBytesRead);

            return buffer;
        }

        public static float[] ReadMatrix<T>(int Adress, int MatrixSize) where T : struct
        {
            var ByteSize = Marshal.SizeOf(typeof(T));
            var buffer = new byte[ByteSize * MatrixSize];
            NtReadVirtualMemory((int)m_iProcessHandle, (IntPtr)Adress, buffer, buffer.Length, ref m_iBytesRead);

            return ConvertToFloatArray(buffer);
        }

        public static IntPtr GetModuleAddress(string Name)
        {
            try
            {
                foreach (ProcessModule ProcMod in m_iProcess.Modules)
                    if (Name.ToUpper() == ProcMod.ModuleName.ToUpper())
                        return ProcMod.BaseAddress;
            }
            catch
            {
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: Cannot find - " + Name + " | Check file extension.");
            Console.ResetColor();

            return IntPtr.Zero;
        }

        #region Other

        internal struct Flags
        {
            public const int PROCESS_VM_OPERATION = 0x0008;
            public const int PROCESS_VM_READ = 0x0010;
            public const int PROCESS_VM_WRITE = 0x0020;
        }

        #endregion Other

        #region Conversion

        public static float[] ConvertToFloatArray(byte[] bytes)
        {
            if (bytes.Length % 4 != 0)
                throw new ArgumentException();

            var floats = new float[bytes.Length / 4];

            for (var i = 0; i < floats.Length; i++)
                floats[i] = BitConverter.ToSingle(bytes, i * 4);

            return floats;
        }

        private static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }

        private static byte[] StructureToByteArray(object obj)
        {
            var length = Marshal.SizeOf(obj);

            var array = new byte[length];

            var pointer = Marshal.AllocHGlobal(length);

            Marshal.StructureToPtr(obj, pointer, true);
            Marshal.Copy(pointer, array, 0, length);
            Marshal.FreeHGlobal(pointer);

            return array;
        }

        #endregion Conversion

        #region DllImports

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("ntdll.dll")]
        private static extern bool NtReadVirtualMemory(int hProcess, IntPtr lpBaseAddress, byte[] buffer, int size, ref int lpNumberOfBytesRead);

        [DllImport("ntdll.dll")]
        private static extern bool NtWriteVirtualMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, out int lpNumberOfBytesWritten);

        #endregion DllImports
    }
}
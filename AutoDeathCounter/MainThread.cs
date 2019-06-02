using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AutoDeathCounter
{
    internal class MainThread
    {
        /// <summary>  
        /// 7FF49E871778
        /// 7FF49E7CAB44
        /// 7FF48EE9A284
        /// </summary>
        public static void Start()
        {
            Console.WriteLine("Attaching..");
            var found = false;

            while (!found && Program.IsRunnings)
            {
                Thread.Sleep(250);
                found = MemoryManager.Attatch("DarkSoulsIII");
            }
            var baseAddress = MemoryManager.GetModuleAddress("DarkSoulsIII.exe");
            Console.WriteLine(MemoryManager.ReadMemory<int>(new IntPtr(140688607718212)));
        }
    }
}

using System;
using System.Threading;

namespace AutoDeathCounter
{
    internal static class MainThread
    {
        /// <summary>
        /// 7FF49E871778
        /// 0x7FF49E7CAB44 = 140688607718212
        /// 7FF48EE9A284
        /// DarkSoulsIII.exe+63BB310 - call DarkSoulsIII.exe+63BB315
        /// 0x0000000140000000 - 5368709120
        /// 0x7FF48EE9A284 - 140688346423940
        /// 
        /// DarkSoulsIII.exe+1CD2E8
        /// DarkSoulsIII.exe+FFB414
        /// DarkSoulsIII.exe+1517FE4
        /// DarkSoulsIII.exe+3B5603C
        /// DarkSoulsIII.exe+3B5627C
        /// </summary>
        private readonly static IntPtr HPPointer = new IntPtr(0x1447819e8);
        private const int HPOffset = 0x454;
        
        private static bool _isRunning;

        internal static event Action OnDeath = delegate { };

        internal static event Action OnRespawn = delegate { };

        internal static event OnHPChangeHandle OnHPChange = delegate { };

        internal delegate void OnHPChangeHandle(int currentHP, int lastHP);

        public static void Run()
        {
            _isRunning = true;

            Console.WriteLine("Attaching..");
            var found = false;

            while (!found && _isRunning)
            {
                Thread.Sleep(250);
                found = MemoryManager.Attatch("DarkSoulsIII");
            }

            Console.WriteLine("Running..");

            Execute();

            // close process handler
            MemoryManager.Detach();
            Console.WriteLine("Closed.");
        }

        private static void Execute()
        {
            var lastHP = -1;

            while (_isRunning)
            {
                var addr = MemoryManager.GetModuleAddress("DarkSoulsIII.exe");
                var pointerVal = MemoryManager.ReadMemory<IntPtr>(HPPointer);
                var currentHP = MemoryManager.ReadMemory<int>(IntPtr.Add(pointerVal, HPOffset));

                if (currentHP != lastHP)
                {
                    OnHPChange(currentHP, lastHP);

                    if (currentHP == 0)
                    {
                        OnDeath();
                    }

                    if (lastHP == 0)
                    {
                        OnRespawn();
                    }
                }

                lastHP = currentHP;

                Thread.Sleep(500);
            }
        }

        public static void Stop()
        {
            _isRunning = false;
        }
    }
}
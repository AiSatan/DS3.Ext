using System;
using System.Threading;

namespace AutoDeathCounter
{
    internal static class MainThread
    {
        /// <summary>
        /// 7FF49E871778
        /// 7FF49E7CAB44 = 140688607718212
        /// 7FF48EE9A284
        /// </summary>
        private const long HPAddress = 140688607718212;

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
                var currentHP = MemoryManager.ReadMemory<int>(HPAddress);

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
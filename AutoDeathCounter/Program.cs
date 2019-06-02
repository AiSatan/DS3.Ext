using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace AutoDeathCounter
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Starting..");

            if (!IsAdmin())
            {
                Console.WriteLine("Run as admin is required");
                throw new AccessViolationException("Run as admin is required");
            }

            InitEvents();

            Console.WriteLine("Running main..");
            var task = Task.Run(MainThread.Run);

            Console.WriteLine("Press 'any' to quit");
            Console.ReadKey();
            MainThread.Stop();
            task.Wait(1000);
        }

        private static void InitEvents()
        {
            MainThread.OnDeath += EventHandles.MainThread_OnDeath;
            MainThread.OnRespawn += EventHandles.MainThread_OnRespawn;
            MainThread.OnHPChange += EventHandles.MainThread_OnHPChange;
        }

        private static bool IsAdmin()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
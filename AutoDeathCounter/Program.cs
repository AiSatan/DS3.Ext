using System;
using System.IO;
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
            Task.Run(MainThread.Run);

            Console.WriteLine("Press 'any' to quit");
            Console.ReadKey();
            MainThread.Stop();
        }

        private static void InitEvents()
        {
            MainThread.OnDeath += MainThread_OnDeath;
            MainThread.OnRespawn += MainThread_OnRespawn;
            MainThread.OnHPChange += MainThread_OnHPChange;
        }

        private static void MainThread_OnHPChange(int currentHP, int lastHP)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"currentHP: {currentHP}");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void MainThread_OnDeath()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"OnDeath");
            Console.ForegroundColor = ConsoleColor.Gray;
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "output.txt"), "dead");
        }

        private static void MainThread_OnRespawn()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"OnRespawn");
            Console.ForegroundColor = ConsoleColor.Gray;
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "output.txt"), "alive");
        }

        private static bool IsAdmin()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}

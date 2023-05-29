using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using winp;

namespace example
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (WinProcess.IsAdministrator())
            {
                while (true)
                {
                    Console.Write("Process name: ");
                    string input = Console.ReadLine();

                    Process[] processes = Process.GetProcessesByName(input);

                    foreach (Process process in processes)
                    {
                        WinProcess.SetCritical(process);
                    }
                }
            }
            else
            {
                Console.WriteLine("The Application needs Administrator privileges.");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
    }
}

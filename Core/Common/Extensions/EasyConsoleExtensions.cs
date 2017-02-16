using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseMonitoringSystem.Core.Common.Extensions
{
    using EasyConsole;

    public static class EasyConsoleExtensions
    {
        public static Guid ReadGuid(string prompt)
        {
            Output.DisplayPrompt(prompt);

            return ReadGuid();
        }

        public static Guid ReadGuid()
        {
            string consoleInput = Console.ReadLine();
            Guid value;

            while (!Guid.TryParse(consoleInput, out value))
            {
                Output.DisplayPrompt("Please enter a guid");
                consoleInput = Console.ReadLine();
            }

            return value;
        }
    }
}

using ManyConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXLocalizationNugetGenerator.Abstractions
{
    public abstract class BaseCommand : ConsoleCommand
    {
        public virtual string CommandName { get => GetType().Name; }

        public abstract string CommandDescription { get; }

        public BaseCommand() : base()
        {
            // Register the actual command with a simple (optional) description.
            IsCommand(CommandName, CommandDescription);
        }

        public override int Run(string[] remainingArguments)
        {
            try
            {
                var result = Execute(remainingArguments);

                ConsoleWrite($"\nCommand {CommandName} succeeded.", false);

                return result;
            }
            catch (Exception)
            {
                ConsoleWrite($"\nCommand {CommandName} failed.", true);

                return -1;
            }
        }

        public virtual int Execute(string[] remainingArguments)
        {
            return 0;
        }

        protected void ConsoleWriteLine(string mesg, bool isError = false)
        {
            ConsoleWrite($"{mesg}\n", isError);
        }

        protected void ConsoleWrite(string mesg, bool isError = false)
        {
            Console.ForegroundColor = isError ? ConsoleColor.Red : ConsoleColor.Green;
            Console.Write(mesg ?? string.Empty);
            Console.ResetColor();
        }
    }
}

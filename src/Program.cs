using DXLocalizationNugetGenerator.Command;
using ManyConsole;
using System;
using System.Collections.Generic;

namespace DXLocalizationNugetGenerator
{
    class Program
    {
        static int Main(string[] args)
        {
#if DEBUG
            //args = new[] { nameof(CreateNuget),
            //    @"-i=D:\Playground\devexpress-nuget-localization\target\nuspec",
            //    @"-o=D:\Playground\devexpress-nuget-localization\target\nuget"};
            args = new[] { nameof(CreateNuspec),
                @"-inputDXNuGetPath=C:\Program Files (x86)\DevExpress 21.1\Components\System\Components\packages",
                @"-inputLocalizationPath=D:\Playground\devexpress-nuget-localization\localization",
                @"-outputLanguageCode=cs",
                @"-outputNuspecPath=D:\Playground\devexpress-nuget-localization\nuspec"};
#endif

            // locate any commands in the assembly (or use an IoC container, or whatever source)
            var commands = GetCommands();

            // then run them.
            return ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
        }

        public static IEnumerable<ConsoleCommand> GetCommands()
        {
            return ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
        }
    }
}

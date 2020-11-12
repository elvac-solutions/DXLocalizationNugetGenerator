using DXLocalizationNugetGenerator.Abstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using static DXLocalizationNugetGenerator.Model.Data;

namespace DXLocalizationNugetGenerator.Command
{
    public class CreateNuget : BaseCommand
    {
        const string NugetDownloadSite = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe";

        public override string CommandDescription => "Creates NuGet localization packages from Nuspec files.";

        public CreateNuget() : base()
        {
            HasRequiredOption("i|inputNuspecPath=", "The full path of the reference nuget .", t => InputNuspecPath = t);
            HasRequiredOption("o|outputDirectory=", "The output directory path.", t => OutputDirectory = t);
        }

        #region PARAMETERS

        /// <summary>
        /// Gets or sets the localization DLL path.
        /// </summary>
        /// <value>
        /// The localization DLL path.
        /// </value>
        public string OutputDirectory { get; set; }

        /// <summary>
        /// Gets or sets the nuget packages path.
        /// </summary>
        /// <value>
        /// The nuget packages path.
        /// </value>
        public string InputNuspecPath { get; set; }

        #endregion PARAMETERS

        public override int Execute(string[] remainingArguments)
        {
            ValidateParameters();

            string nugetExecutablePath = GetNugetExecutablePath();

            CreateNugetPackages(nugetExecutablePath);

            return 0;
        }

        void ValidateParameters()
        {
            if (!Directory.Exists(OutputDirectory))
            {
                Directory.CreateDirectory(OutputDirectory);
            }

            if (!Directory.Exists(InputNuspecPath))
            {
                throw new DirectoryNotFoundException("Input Nuspec path does not exist.");
            }
        }

        void CreateNugetPackages(string nugetExecutablePath)
        {
            var nuspecFilePaths = Directory.GetFiles(InputNuspecPath, "*.nuspec");
            
            foreach (string nuspecFilePath in nuspecFilePaths)
            {
                using (var process = new Process())
                {
                    process.StartInfo.FileName = nugetExecutablePath;
                    process.StartInfo.Arguments = "pack " + nuspecFilePath + " -OutputDirectory " + OutputDirectory;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                    process.Start();
                    process.WaitForExit();
                }
            }
        }

        string GetNugetExecutablePath()
        {
            string alternatePath;

            var nugetExecutableExistsOnPath = ExistsOnPath("nuget.exe", out alternatePath);

            if (nugetExecutableExistsOnPath)
            {
                alternatePath = Path.Combine(OutputDirectory, "nuget.exe");

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(NugetDownloadSite, alternatePath);
                }
            }

            return alternatePath;
        }

        bool ExistsOnPath(string fileName, out string targetPath)
        {
            targetPath = GetFullPath(fileName);

            return targetPath != null;
        }

        string GetFullPath(string fileName)
        {
            if (File.Exists(fileName))
            {
                return Path.GetFullPath(fileName);
            }

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(Path.PathSeparator))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            return null;
        }
    }
}

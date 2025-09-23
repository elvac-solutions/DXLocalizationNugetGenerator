using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using DXLocalizationNugetGenerator.Abstractions;

using static DXLocalizationNugetGenerator.Model.Data;

namespace DXLocalizationNugetGenerator.Command
{
    public class CreateNuspec : BaseCommand
    {
        public override string CommandDescription => "Creates DX nuspec localization files from DX nuget packages.";

        public CreateNuspec() : base()
        {
            HasRequiredOption("inputDXNuGetPath=", "The full path of the directory that contains DevExpress nuget packages.", t => NugetPackagesPath = t);
            HasRequiredOption("inputLocalizationPath=", "The full path of DevExpress localization libraries.", t => LocalizationDllPath = t);
            HasRequiredOption("outputLanguageCode=", "The two-letter language code.", t => LanguageCode = t);
            HasRequiredOption("outputNuspecPath=", "The output nuspec path.", t => OutputNuspecPath = t);
        }

        #region CONSTANTS

        public const string DEFAULT_INPUT_LANGUAGE = "de";

        #endregion

        #region PARAMETERS

        /// <summary>
        /// Gets or sets the output nuspec path.
        /// </summary>
        /// <value>
        /// The output nuspec path.
        /// </value>
        public string OutputNuspecPath { get; set; }

        /// <summary>
        /// Gets or sets the localization DLL path.
        /// </summary>
        /// <value>
        /// The localization DLL path.
        /// </value>
        public string LocalizationDllPath { get; set; }

        /// <summary>
        /// Gets or sets the nuget packages path.
        /// </summary>
        /// <value>
        /// The nuget packages path.
        /// </value>
        public string NugetPackagesPath { get; set; }

        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        public string LanguageCode { get; set; }

        #endregion PARAMETERS

        public override int Execute(string[] remainingArguments)
        {
            ValidateParameters();

            var packages = FindNugetPackages(NugetPackagesPath);

            if (packages.Count() == 0)
            {
                ConsoleWrite("There are no nuget packages.");
                return 0;
            }

            CreateNuspecFiles(packages, LocalizationDllPath, OutputNuspecPath, LanguageCode);

            return 0;
        }

        void ValidateParameters()
        {
            if (!Directory.Exists(OutputNuspecPath))
            {
                Directory.CreateDirectory(OutputNuspecPath);
            }

            if (!Directory.Exists(LocalizationDllPath))
            {
                throw new DirectoryNotFoundException("Directory with localization libraries does not exist.");
            }

            if (!Directory.Exists(NugetPackagesPath))
            {
                throw new DirectoryNotFoundException("Directory with DevExpress Nuget packages does not exist.");
            }
        }

        string[] FindNugetPackages(string nugetPackagesPath)
        {
            string pattern = "*." + DEFAULT_INPUT_LANGUAGE + ".*" + ".nupkg";
            string[] files = Directory.GetFiles(nugetPackagesPath, pattern);

            return files;
        }

        void CreateNuspecFiles(string[] nugetPackages, string localizationLibrariesPath, string outputDirectory, string languageCode)
        {
            foreach (string nugetPackage in nugetPackages)
            {
                using ZipArchive zip = ZipFile.OpenRead(nugetPackage);

                var dlllibEntryList = zip.Entries.Where(w => w.FullName.StartsWith("lib/") && w.Name.EndsWith(".dll"));

                var nuspecFile = zip.Entries.FirstOrDefault(f => f.Name.EndsWith(".nuspec"));

                if (nuspecFile == null)
                {
                    /*
                     * nuspec not found in nupkg; skip
                     */

                    continue;
                }

                /*
                 * Prepare new nuspec filename and path.
                 */
                string nuspecFileLocalizedName = ReplaceLanguage(nuspecFile.Name, languageCode);
                string nuspecFileLocalizedPath = Path.Combine(OutputNuspecPath, nuspecFileLocalizedName);

                /*
                 * Extract nuspec.
                 */
                nuspecFile.ExtractToFile(nuspecFileLocalizedPath, true);

                FilesRoot root = new FilesRoot();

                /*
                 * Create files node.
                 */
                foreach (var dlllibEntry in dlllibEntryList)
                {
                    string subFolder;
                    if (dlllibEntry.FullName.StartsWith("lib/net4"))
                        subFolder = "Framework";
                    else
                        subFolder = "NetCore";

                    XmlFile xmlFile = new XmlFile()
                    {
                        Src = Path.Combine(Path.GetRelativePath(OutputNuspecPath, localizationLibrariesPath), subFolder, dlllibEntry.Name),
                        Target = dlllibEntry.FullName,
                    };
                    root.XmlFiles.Add(xmlFile);
                }

                /*
                 * Convert data to xml.
                 */
                string filesElementToXml = string.Empty;

                using (var stringwriter = new System.IO.StringWriter())
                {
                    StringBuilder sb = new StringBuilder();
                    using (XmlWriter writer = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true }))
                    {
                        new XmlSerializer(root.GetType()).Serialize(writer, root);
                    }
                    filesElementToXml = sb.ToString();
                }

                XmlDocument doc = new XmlDocument();
                doc.Load(nuspecFileLocalizedPath);

                if (dlllibEntryList.Any())
                {
                    doc["package"].InnerXml = doc["package"].InnerXml + filesElementToXml;
                }

                doc.InnerXml = ReplaceLanguage(doc.InnerXml, LanguageCode);
                doc.InnerXml = FixNet50(doc.InnerXml);
                doc.Save(nuspecFileLocalizedPath);
            }
        }

        private string FixNet50(string text)
        {
            text = text.Replace("/net5.0-windows/", "/net5.0-windows7.0/");
            text = text.Replace("\"net5.0-windows\"", "\"net5.0-windows7.0\"");

            return text;
        }

        string ReplaceLanguage(string text, string language)
        {
            // .de.
            text = text.Replace("." + DEFAULT_INPUT_LANGUAGE + ".", "." + language + ".");
            // .de<
            text = text.Replace("." + DEFAULT_INPUT_LANGUAGE + "<", "." + language + "<");
            // .de"
            text = text.Replace("." + DEFAULT_INPUT_LANGUAGE + "\"", "." + language + "\"");
            // /de/
            text = text.Replace("/" + DEFAULT_INPUT_LANGUAGE + "/", "/" + language + "/");
            // >de<
            text = text.Replace(">" + DEFAULT_INPUT_LANGUAGE + "<", ">" + language + "<");
            return text;
        }

    }
}

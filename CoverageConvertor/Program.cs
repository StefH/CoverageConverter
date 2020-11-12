using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CoverageConverter.Settings;
using Microsoft.Extensions.Logging;
using NuGet.Common;

namespace CoverageConverter
{
    /// <summary>
    /// Based on https://github.com/danielpalme/ReportGenerator/wiki/Visual-Studio-Coverage-Tools
    /// </summary>
    public class Program
    {
        private const string VERSION = "16.8.0";
        private const string DOT_COVERAGE_DOT_XML = ".coveragexml";

        /// <summary>
        /// The Logger.
        /// </summary>
        private readonly ILogger<Program> _logger = LoggerFactory.Create(o => o.AddConsole()).CreateLogger<Program>();

        static void Main(string[] args)
        {
            

            var settings = CoverageConvertorSettingsParser.ParseArguments(args);

            List<string> coverageFiles;
            try
            {
                coverageFiles = Directory
                    .EnumerateFiles(settings.CoverageFilesFolder, $"*{settings.DotCoverageExtension}", SearchOption.AllDirectories)
                    .Where(path => Guid.TryParse(new DirectoryInfo(Path.GetDirectoryName(path)).Name, out _)) // Only take .coverage file if the folder is a guid (that's the one VSTest creates)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing folder '{settings.CoverageFilesFolder}' (Exception: {ex})");
                return;
            }

            if (coverageFiles.Count == 0)
            {
                Console.WriteLine($"No '{settings.DotCoverageExtension}' files found in folder '{settings.CoverageFilesFolder}'");
                return;
            }

            var nugetHomePath = NuGetEnvironment.GetFolderPath(NuGetFolderPath.NuGetHome);
            // C:\Users\azurestef\.nuget\packages\microsoft.codecoverage\16.8.0\build\netstandard1.0\CodeCoverage
            var codeCoverageExePath = $"{Path.Combine(nugetHomePath, "packages", "microsoft.codecoverage", VERSION, "build", "netstandard1.0", "CodeCoverage", "CodeCoverage.exe")}";

            foreach (var sourceFilePath in coverageFiles)
            {
                var destinationFilePath = sourceFilePath.Replace(settings.DotCoverageExtension, DOT_COVERAGE_DOT_XML);

                Console.WriteLine($"Generating file '{destinationFilePath}' based on '{sourceFilePath}'");

                try
                {
                    var process = Process.Start(codeCoverageExePath, $"analyze /output:\"{destinationFilePath}\" \"{sourceFilePath}\"");
                    process?.WaitForExit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing file '{sourceFilePath}' (Exception: {ex})");
                }

                Console.WriteLine();
            }
        }
    }
}
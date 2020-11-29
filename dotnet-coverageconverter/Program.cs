using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using Microsoft.Extensions.Logging;

namespace CoverageConverter
{
    /// <summary>
    /// Idea is based on https://github.com/danielpalme/ReportGenerator/wiki/Visual-Studio-Coverage-Tools
    /// </summary>
    public class Program
    {
        private const string DOT_COVERAGE_DOT_XML = ".coveragexml";

        public class Options
        {
            [Option('f', "CoverageFilesFolder", Required = true, HelpText = "The folder where the .coverage files are defined.")]
            public string CoverageFilesFolder { get; set; }

            [Option('d', "DotCoverageExtension", HelpText = "The extension from the coverage files.", Default = ".coverage")]
            public string DotCoverageExtension { get; set; }

            [Option('a', "AllDirectories", HelpText = "Includes also sub-folders in the search operation.", Default = true)]
            public bool AllDirectories { get; set; }

            [Option('p', "ProcessAllFiles", HelpText = "Process all .coverage files, if not set, then only folders which are a guid (that's the one VSTest creates) will be processed.", Default = false)]
            public bool ProcessAllFiles { get; set; }

            [Option('o', "Overwrite", HelpText = "Overwrite the existing .coveragexml files.", Default = true)]
            public bool Overwrite { get; set; }

            [Option('r', "RemoveOriginalCoverageFiles", HelpText = "Remove the original .coverage files.")]
            public bool RemoveOriginalCoverageFiles { get; set; }
        }

        /// <summary>
        /// The Logger.
        /// </summary>
        private static readonly ILogger<Program> Logger = LoggerFactory.Create(o =>
        {
            o.SetMinimumLevel(LogLevel.Information);
            o.AddConsole();
        }).CreateLogger<Program>();

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(Run);
        }

        private static void Run(Options options)
        {
            List<string> coverageFiles;
            try
            {
                coverageFiles = FindCoverageFiles(options);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Error reading folder '{CoverageFilesFolder}'.", options.CoverageFilesFolder);
                return;
            }

            if (coverageFiles.Count == 0)
            {
                Logger.LogWarning("No '{DotCoverageExtension}' files found in folder '{CoverageFilesFolder}'.", options.DotCoverageExtension, options.CoverageFilesFolder);
                return;
            }

            var codeCoverageExePath = $"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Microsoft.CodeCoverage", "CodeCoverage.exe")}";
            foreach (var sourceFilePath in coverageFiles)
            {
                var destinationFilePath = sourceFilePath.Replace(options.DotCoverageExtension, DOT_COVERAGE_DOT_XML);

                Logger.LogInformation("Generating file '{DestinationFilePath}' based on '{SourceFilePath}'.", destinationFilePath, sourceFilePath);

                DeleteExistingDestinationFileIfNeeded(options, destinationFilePath);

                RunCodeCoverageExe(codeCoverageExePath, sourceFilePath, destinationFilePath);

                DeleteOriginalCoverageFileIfNeeded(options, sourceFilePath);
            }
        }

        private static List<string> FindCoverageFiles(Options options)
        {
            var searchOption = options.AllDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            return Directory
                .EnumerateFiles(options.CoverageFilesFolder, $"*{options.DotCoverageExtension}", searchOption)
                .Where(path => options.ProcessAllFiles || Guid.TryParse(new DirectoryInfo(Path.GetDirectoryName(path)).Name, out _))
                .ToList();
        }

        private static void DeleteExistingDestinationFileIfNeeded(Options options, string destinationFilePath)
        {
            try
            {
                if (options.Overwrite && File.Exists(destinationFilePath))
                {
                    File.Delete(destinationFilePath);
                }
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Unable to delete existing file '{DestinationFilePath}'.", destinationFilePath);
            }
        }

        private static void RunCodeCoverageExe(string codeCoverageExePath, string sourceFilePath, string destinationFilePath)
        {
            try
            {
                var process = Process.Start(codeCoverageExePath, $"analyze /output:\"{destinationFilePath}\" \"{sourceFilePath}\"");
                process?.WaitForExit();
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Error processing file '{SourceFilePath}'.", sourceFilePath);
            }
        }

        private static void DeleteOriginalCoverageFileIfNeeded(Options options, string sourceFilePath)
        {
            try
            {
                if (options.RemoveOriginalCoverageFiles && File.Exists(sourceFilePath))
                {
                    File.Delete(sourceFilePath);
                }
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Unable to delete original file '{sourceFilePath}'.", sourceFilePath);
            }
        }
    }
}
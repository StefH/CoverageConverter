using CoverageConvertor.Settings;

namespace CoverageConverter.Settings
{
    /// <summary>
    /// A static helper class to parse commandline arguments into CoverageConvertorSettings.
    /// </summary>
    internal static class CoverageConvertorSettingsParser
    {
        public static CoverageConvertorSettings ParseArguments(string[] args)
        {
            var parser = new SimpleCommandLineParser();
            parser.Parse(args);

            return new CoverageConvertorSettings
            {
                CoverageFilesFolder = parser.GetStringValue("CoverageFilesFolder", "E:\\TFSOnline\\_work"),
                DotCoverageExtension = parser.GetStringValue("DotCoverageExtension", ".coverage"),
            };
        }
    }
}
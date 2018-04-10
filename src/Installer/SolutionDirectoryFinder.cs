namespace Installer
{
    using System;
    using System.IO;
    using System.Linq;

    public static class SolutionDirectoryFinder
    {
        public static string CalculateContentRootFolder(string sourcePath)
        {
            var directoryInfo = new DirectoryInfo(sourcePath);
            while (!DirectoryContains(directoryInfo.FullName, "LicenseMonitoringSystem.sln"))
            {
                directoryInfo = directoryInfo.Parent ?? throw new Exception("Could not find content root folder!");
            }

            return directoryInfo.FullName;
        }

        private static bool DirectoryContains(string directory, string fileName)
        {
            return Directory.GetFiles(directory).Any(filePath => string.Equals(Path.GetFileName(filePath), fileName));
        }
    }
}
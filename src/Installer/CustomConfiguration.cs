using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Installer
{
    using Newtonsoft.Json;

    public class CustomConfiguration
    {
        public string ProjectDir { get; set; }

        [JsonIgnore]
        public string SolutionDir => SolutionDirectoryFinder.CalculateContentRootFolder(ProjectDir);
        public string Configuration { get; set; }
        public string OutDir { get; set; }
    }
}

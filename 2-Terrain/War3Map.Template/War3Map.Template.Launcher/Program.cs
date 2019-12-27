using System.Diagnostics;
using System.IO;

using War3Net.Build;

namespace War3Map.Template.Launcher
{
    internal static class Program
    {
        // Input
        private const string SourceCodeProjectFolderPath = @"..\..\..\..\War3Map.Template.Source";
        private const string AssetsFolderPath = @".\Assets\";

        // Output
        private const string OutputFolderPath = @"..\..\..\..\artifacts";
        private const string OutputMapName = @"War3CSharpTutorial.w3x";

        // Warcraft III
        private const string Warcraft3ExecutableFilePath = "C:\\Program Files (x86)\\Warcraft III Beta\\x86_64\\Warcraft III.exe";
        private const string Warcraft3CommandLineArgs = @"-nowfpause -launch ";

        private static void Main()
        {
            const string BaseMapPath = @"..\..\..\..\MyBaseMap.w3x";

            var co = CompilerOptions.GetCompilerOptions(SourceCodeProjectFolderPath, OutputFolderPath);
            co.MapEnvironment = null;

            // Build and launch
            var mapBuilder = new MapBuilder(OutputMapName);
            if (mapBuilder.Build(co, BaseMapPath, AssetsFolderPath))
            {
                var mapPath = Path.Combine(OutputFolderPath, OutputMapName);
                var absoluteMapPath = new FileInfo(mapPath).FullName;

#if DEBUG
                if (Warcraft3ExecutableFilePath != null)
                {
                    Process.Start(Warcraft3ExecutableFilePath, $"{Warcraft3CommandLineArgs} -loadfile \"{absoluteMapPath}\"");
                }
                else
#endif
                {
                    Process.Start("explorer.exe", $"/select, \"{absoluteMapPath}\"");
                }
            }
        }
    }
}
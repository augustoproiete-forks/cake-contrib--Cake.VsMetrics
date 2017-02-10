﻿using System.Collections.Generic;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.VsMetrics
{
    public sealed class VsMetricsRunner : Tool<VsMetricsSettings>
    {
        private ICakeEnvironment _environment;

        public VsMetricsRunner(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IToolLocator tools)
            : base(fileSystem, environment, processRunner, tools)
        {
            _environment = environment;
        }

        // TODO fw
        // - Add VsMetrics.Tests
        // - Add StyleCopAnalyzer and Ruleset for all projects
        // - Test all parameters to metrics.exe
        // - Create a better sample .cake script
        // - Add a build pipeline with NuGet etc.
        // - Should I move the project into a Source folder?
        // - Clean up code
        // - Add documentation
        public void Run(IEnumerable<FilePath> inputFilePaths, FilePath outputFilePath, VsMetricsSettings settings)
        {
            Contract.RequireNonNull(inputFilePaths, nameof(inputFilePaths));
            Contract.RequireNonNull(outputFilePath, nameof(outputFilePath));
            settings = settings ?? new VsMetricsSettings();

            Run(settings, GetArguments(inputFilePaths, outputFilePath, settings));
        }

        protected override string GetToolName()
        {
            return "Metrics";
        }

        protected override IEnumerable<string> GetToolExecutableNames()
        {
            return new[] { "metrics.exe" };
        }

        private ProcessArgumentBuilder GetArguments(IEnumerable<FilePath> inputFilePaths, FilePath outputFilePath, VsMetricsSettings settings)
        {
            var builder = new ProcessArgumentBuilder();

            Append(builder, "/f:", inputFilePaths);
            Append(builder, "/o:", outputFilePath);
            Append(builder, "/dir:", settings.AssemblyDirectories);
            Append(builder, "/gac:", settings.SearchGac);
            Append(builder, "/plat:", settings.AssemblyPlatforms);
            Append(builder, "/ref:", settings.AssemblyReferences);
            Append(builder, "/iit:", settings.IgnoreInvalidTargets);
            Append(builder, "/igc:", settings.IgnoreGeneratedCode);
            Append(builder, "/sf:", settings.SuccessFile);
            Append(builder, "/q:", settings.Quiet);

            return builder;
        }

        private void Append(ProcessArgumentBuilder builder, string arg, IEnumerable<DirectoryPath> directoryPaths)
        {
            foreach (var directoryPath in directoryPaths)
            {
                Append(builder, arg, directoryPath);
            }
        }

        private void Append(ProcessArgumentBuilder builder, string arg, DirectoryPath directoryPath)
        {
            builder.Append($"{arg}\"{directoryPath.MakeAbsolute(_environment).FullPath}\"");
        }

        private void Append(ProcessArgumentBuilder builder, string arg, IEnumerable<FilePath> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                Append(builder, arg, filePath);
            }
        }

        private void Append(ProcessArgumentBuilder builder, string arg, FilePath filePath)
        {
            builder.Append($"{arg}\"{filePath.MakeAbsolute(_environment).FullPath}\"");
        }

        private void Append(ProcessArgumentBuilder builder, string arg, bool expression)
        {
            if (expression)
            {
                builder.Append(arg);
            }
        }
    }
}
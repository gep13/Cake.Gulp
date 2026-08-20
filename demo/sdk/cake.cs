#!/usr/bin/env dotnet
#:sdk Cake.Sdk@6.2.0
#:project ../../src/Cake.Gulp/Cake.Gulp.csproj

// Cake SDK consumer demo for Cake.Gulp. Runs as a file-based .NET
// program (introduced in .NET 10) using the Cake.Sdk directives.
// The #:project directive above lets the SDK build the addin from
// source rather than referencing a published nupkg.
//
// To run locally:
//   cd demo/sdk
//   dotnet cake.cs
//
// Runs the same three checks the script and frosting demos run.

using Cake.Gulp;

Task("Default")
    .IsDependentOn("Settings-Global")
    .IsDependentOn("Settings-Local")
    .IsDependentOn("Alias-Surface");

Task("Settings-Global")
    .Does(() =>
{
    var settings = new GulpRunnerSettings();
    settings.WithGulpFile("./gulpfile.js");
    settings.WithArguments("default --silent");

    AssertThat(settings.GulpFile != null, "GulpFile should be set");
    AssertThat(settings.GulpFile.FullPath.EndsWith("gulpfile.js"),
        "GulpFile mismatch: " + settings.GulpFile.FullPath);
    AssertThat(settings.Arguments == "default --silent",
        "Arguments mismatch: " + settings.Arguments);

    Information("GulpRunnerSettings OK (GulpFile={0}, Arguments={1})",
        settings.GulpFile.FullPath, settings.Arguments);
});

Task("Settings-Local")
    .Does(() =>
{
    var settings = new GulpLocalRunnerSettings();
    settings.WithGulpFile("./build.gulpfile.js");
    settings.WithArguments("ci");
    settings.SetPathToGulpJs("custom/path/gulp.js");

    AssertThat(settings.GulpFile != null
        && settings.GulpFile.FullPath.EndsWith("build.gulpfile.js"),
        "GulpFile mismatch");
    AssertThat(settings.Arguments == "ci",
        "Arguments mismatch: " + settings.Arguments);
    AssertThat(settings.PathToGulpJs.FullPath == "custom/path/gulp.js",
        "PathToGulpJs mismatch: " + settings.PathToGulpJs.FullPath);

    Information("GulpLocalRunnerSettings OK (PathToGulpJs={0})",
        settings.PathToGulpJs.FullPath);
});

Task("Alias-Surface")
    .Does(() =>
{
    var factory = Gulp;
    AssertThat(factory != null, "Gulp factory should be non-null");

    var local = factory.Local;
    AssertThat(local != null, "Gulp.Local should be non-null");

    var global = factory.Global;
    AssertThat(global != null, "Gulp.Global should be non-null");

    Information("Alias surface OK (factory + Local + Global resolved)");
});

RunTarget("Default");

// ----- Helpers (must come AFTER top-level statements per CS8803) -----

static void AssertThat(bool condition, string message)
{
    if (!condition)
    {
        throw new Exception("Assertion failed: " + message);
    }
}

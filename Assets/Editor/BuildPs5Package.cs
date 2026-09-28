using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildPs5Package
{
    private static string GetProjectDir()
    {
        return Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
    }

    private static string GetBuildOutputDir()
    {
        return Path.Combine(GetProjectDir(), "build", "Build");
    }

    private static void PrepareCleanBuildOutput(string outputPath)
    {
        string expectedPath = Path.GetFullPath(Path.Combine(GetProjectDir(), "build", "Build"));
        string resolvedPath = Path.GetFullPath(outputPath);
        if (!string.Equals(resolvedPath, expectedPath, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Refusing to clean an unexpected build path: {resolvedPath}");
        }

        if (Directory.Exists(resolvedPath))
        {
            Directory.Delete(resolvedPath, true);
        }
        Directory.CreateDirectory(resolvedPath);
    }

    [MenuItem("PS5/1. Build Player (Unity)", false, 1)]
    public static void Build()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.PS5, BuildTarget.PS5);

        string outputPath = GetBuildOutputDir();
        PrepareCleanBuildOutput(outputPath);

        var scenes = System.Array.ConvertAll(
            System.Array.FindAll(EditorBuildSettings.scenes, s => s.enabled),
            s => s.path
        );

        if (scenes.Length == 0)
        {
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (!string.IsNullOrEmpty(activeScene.path))
            {
                scenes = new[] { activeScene.path };
            }
            else
            {
                throw new InvalidOperationException("No enabled build scenes or saved active scene were found.");
            }
        }

        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = BuildTarget.PS5,
            targetGroup = BuildTargetGroup.PS5,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new Exception($"PS5 build failed: {report.summary.result}");
        }
        UnityEngine.Debug.Log($"[PS5] Player built successfully at: {outputPath}");
    }

    [MenuItem("PS5/2. Create PKG (LibProsperoPkg)", false, 2)]
    public static void CreatePkg()
    {
        string projectDir = GetProjectDir();
        string batPath = Path.Combine(projectDir, "build_Pkg.bat");

        string builderPath = Path.Combine(projectDir, "Tools", "BuildPs5Pkg", "BuildPs5Pkg.exe");
        if (!File.Exists(batPath) || !File.Exists(builderPath))
        {
            EditorUtility.DisplayDialog("Error", $"Precompiled package builder not found at:\n{builderPath}", "OK");
            return;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c \"{batPath}\"",
            WorkingDirectory = projectDir,
            UseShellExecute = true
        };

        var proc = Process.Start(startInfo);
        proc?.WaitForExit();

        string pkgDir = Path.Combine(projectDir, "build", "Build-pkg");
        if (proc != null && proc.ExitCode == 0 && Directory.Exists(pkgDir))
        {
            EditorUtility.DisplayDialog("Success", $"PKG created successfully in:\n{pkgDir}", "OK");
            EditorUtility.RevealInFinder(pkgDir);
        }
        else
        {
            EditorUtility.DisplayDialog("Warning", "The process finished with an error. Check the console window for details.", "OK");
        }
    }

    [MenuItem("PS5/3. Full Build (Player + PKG)", false, 3)]
    public static void BuildAndCreatePkg()
    {
        Build();
        CreatePkg();
    }
}

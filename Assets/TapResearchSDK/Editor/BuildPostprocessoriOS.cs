using UnityEngine;
using System.IO;
using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
#endif
#endif

namespace TapResearch.Editor
{
    public class BuildPostprocessoriOS
    {
#if UNITY_EDITOR && UNITY_IOS


        private static string AppAssetsPath => Path.Combine(Application.dataPath, "TapResearchSDK", "Editor", "Templates", "iOS");

        [PostProcessBuild]
        private static void PostProcessBuildiOS(BuildTarget target, string buildPath)
        {
            if (target == BuildTarget.iOS)
            {
                HandleiOSPostBuild(buildPath);
            }
        }

        private static void HandleiOSPostBuild(string buildPath)
        {
            PBXProject project = new PBXProject();
            string projPath = PBXProject.GetPBXProjectPath(buildPath);
            project.ReadFromFile(projPath);

            var mainProjectGuid = project.GetUnityMainTargetGuid();
            string unityFrameworkGuid = project.GetUnityFrameworkTargetGuid();

            project.SetBuildProperty(mainProjectGuid, "ENABLE_BITCODE", "NO");
            project.SetBuildProperty(unityFrameworkGuid, "ENABLE_BITCODE", "NO");
            project.SetBuildProperty(mainProjectGuid, "TARGETED_DEVICE_FAMILY", "1,2");
            project.SetBuildProperty(mainProjectGuid, "SWIFT_VERSION", "5.0");
            var archs = PlayerSettings.iOS.sdkVersion == iOSSdkVersion.DeviceSDK ? "arm64" : "x86_64";
            project.SetBuildProperty(mainProjectGuid, "ARCHS", archs);

            AddFrameworks(project, mainProjectGuid, unityFrameworkGuid, buildPath);

            project.WriteToFile(projPath);
        }


        private static void AddFrameworks(PBXProject project, string mainProjectGuid, string unityFrameworkGuid, string buildPath)
        {            
            string[] requiredFrameworks = {
              "TapResearchSDK.xcframework"
            };

            var unityLinkPhaseGuid = project.GetFrameworksBuildPhaseByTarget(mainProjectGuid);
            var unityFrameworkLinkPhaseGuid = project.GetFrameworksBuildPhaseByTarget(unityFrameworkGuid);

            Directory.CreateDirectory(Path.Combine(buildPath, "Frameworks"));

            foreach (var framework in requiredFrameworks)
            {
                string sourcePath = Path.Combine(AppAssetsPath, "Frameworks", framework + ".zip");
                string destPath = Path.Combine(buildPath, "Frameworks", framework);
                string zipToPath = Path.Combine(buildPath, "Frameworks");

                //Debug.Log("TapResearchSDK sourcePath = " + sourcePath);
                //Debug.Log("TapResearchSDK destPath = " + destPath);
                //Debug.Log("TapResearchSDK zipToPath = " + zipToPath);
                FileUtil.DeleteFileOrDirectory(destPath);
                System.IO.Compression.ZipFile.ExtractToDirectory(sourcePath, zipToPath);

                var frameworkGuid = project.AddFile(destPath, Path.Combine("Frameworks", framework));
                //Debug.Log("TapResearchSDK addFile path combine = " + Path.Combine("Frameworks", framework));

                project.AddFileToBuildSection(mainProjectGuid, unityLinkPhaseGuid, frameworkGuid);
                project.AddFileToBuildSection(unityFrameworkGuid, unityFrameworkLinkPhaseGuid, frameworkGuid);

                project.AddFileToEmbedFrameworks(mainProjectGuid, frameworkGuid);
            }
        }

#endif
    }
}
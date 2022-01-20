#if (UNITY_5_3_OR_NEWER)
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEditor.iOS.Xcode;
using System.IO;
using System;

namespace TapResearch {
	
	public class PostProcessIOS : MonoBehaviour {

		//---------------------------------------------------------------------------------------------
		[PostProcessBuildAttribute(45)]
		private static void OnPostProcessBuildiOS_45(BuildTarget target, string buildPath) {

			if (target == BuildTarget.iOS) {
				//Debug.Log("(45) runtime version: " + Application.unityVersion);
													
				#if !UNITY_CLOUD_BUILD
					string podFileName = buildPath + "/Podfile";
					PostprocessBuild(target, buildPath);
				#endif 
			}
		}

		//---------------------------------------------------------------------------------------------
		// This is called after the Cocoapod has been "installed"
		[PostProcessBuildAttribute(9999)]
		private static void OnPostProcessBuildiOS_9999(BuildTarget target, string buildPath) {

			if (target == BuildTarget.iOS) {
				//Debug.Log("(9999) runtime version: " + Application.unityVersion);

				#if !UNITY_CLOUD_BUILD
					string podFileName = buildPath + "/Podfile";
					AddFrameworkToProjectEmbeddedList(target, buildPath);
				#endif 
			}
		}

		private const string XCFRAMEWORK_ORIGIN_PATH = "Pods/TapResearch";
   	    private const string XCFRAMEWORK_NAME = "TapResearchSDK.xcframework";

		private const string FRAMEWORK_TARGET_PATH = "Frameworks";
 
		private const string FRAMEWORK_ORIGIN_PATH = "Pods/TapResearch";
   	    private const string FRAMEWORK_NAME = "TapResearchSDK.framework";

		//---------------------------------------------------------------------------------------------
		private static void AddFrameworkToProjectEmbeddedList(BuildTarget buildTarget, string path) {

	        //Debug.Log("AddFrameworkToProjectEmbeddedList -----------------------------------------------------------");
			string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
			PBXProject project = new PBXProject();
			project.ReadFromString(File.ReadAllText(projectPath));

			#if (UNITY_2019_3_OR_NEWER)
			string target = project.GetUnityMainTargetGuid();
			#else
			string target = project.TargetGuidByName("Unity-iPhone");
			#endif

			// add the framework to the project and enable 'Embed & Sign' for it
			string sourcePath;
			string destPath;
			if (Application.unityVersion.Contains("2018")) {
				sourcePath = path + "/" + FRAMEWORK_ORIGIN_PATH + "/" + XCFRAMEWORK_NAME + "/ios-arm64_armv7/" + FRAMEWORK_NAME;
				destPath = path + "/" + FRAMEWORK_TARGET_PATH + "/" + FRAMEWORK_NAME;
			}
			else {
				sourcePath = path + "/" + XCFRAMEWORK_ORIGIN_PATH + "/" + XCFRAMEWORK_NAME;
				destPath = path + "/" + FRAMEWORK_TARGET_PATH + "/" + XCFRAMEWORK_NAME;
			}

			Debug.Log(" starting copy from " + sourcePath + " to " + destPath);
			CopyDirectory(sourcePath, destPath);
			string fileGuid = project.AddFile(destPath, destPath);
			Debug.Log("FILE GUID = " + fileGuid);
			UnityEditor.iOS.Xcode.Extensions.PBXProjectExtensions.AddFileToEmbedFrameworks(project, target, fileGuid);

			File.WriteAllText(projectPath, project.WriteToString());
		}

		//---------------------------------------------------------------------------------------------
	    private static void PostprocessBuild(BuildTarget buildTarget, string path) {

	        //Debug.Log("PostprocessBuild -----------------------------------------------------------");
			//Debug.Log("buildPath = " + path);

	        string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
	        PBXProject project = new PBXProject();
	        project.ReadFromString(File.ReadAllText(projectPath));

			#if (UNITY_2019_3_OR_NEWER)
			    string target = project.GetUnityMainTargetGuid();
			#else
	            string target = project.TargetGuidByName("Unity-iPhone");
			#endif

	        // Required Frameworks
	        project.AddFrameworkToProject(target, "SystemConfiguration.framework", false);
	        project.AddFrameworkToProject(target, "Security.framework", false);
	        project.AddFrameworkToProject(target, "AdSupport.framework", false);
	        project.AddFrameworkToProject(target, "CoreServices.framework", false);

			// Required Linker Flags
			project.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");
			project.AddBuildProperty(target, "SWIFT_COMPILATION_MODE", "wholemodule");

			//Debug.Log("Getting SWIFT_VERSION");
			string swiftVersion = project.GetBuildPropertyForAnyConfig(target, "SWIFT_VERSION");
			//Debug.Log("SWIFT_VERSION = " + swiftVersion);
			if (swiftVersion == null || swiftVersion.Length == 0) {
				//Debug.Log("Adding SWIFT_VERSION = 5");
				project.AddBuildProperty(target, "SWIFT_VERSION", "5");
			}
			else {
				//Debug.Log("Setting SWIFT_VERSION = 5");
				project.SetBuildProperty(target, "SWIFT_VERSION", "5");
			}
		
			File.WriteAllText(projectPath, project.WriteToString());
	    }

		//---------------------------------------------------------------------------------------------
		private static void CopyDirectory(string sourcePath, string destPath) {
			
			//Debug.Log("copy from " + sourcePath + " to " + destPath);
			if (!Directory.Exists(destPath)) {
				Directory.CreateDirectory(destPath);
			}
	
			foreach (string file in Directory.GetFiles(sourcePath)) {
				if (!file.Contains(".meta")) {
					File.Copy(file, Path.Combine(destPath, Path.GetFileName(file)));
				}
			}
			foreach (string dir in Directory.GetDirectories(sourcePath)) {
				CopyDirectory(dir, Path.Combine(destPath, Path.GetFileName(dir)));
			}
		}

    }

}
#endif

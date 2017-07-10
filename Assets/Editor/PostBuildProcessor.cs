#if UNITY_5
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using UnityEditor.iOS.Xcode;
using System.IO;

public class PostBuildProcessor : MonoBehaviour {
	
#if UNITY_CLOUD_BUILD
	public static void OnPostprocessBuildiOS (string exportPath) {
		PostprocessBuild (BuildTarget.iPhone, exportPath);
	}
#endif

	[PostProcessBuild]
	public static void OnPostprocessBuild (BuildTarget buildTarget, string path) {
		if (buildTarget != BuildTarget.iOS) {
			Debug.LogWarning ("Build target is not iOS. Postprocess build will not run.");
			return;
		}

#if !UNITY_CLOUD_BUILD
		PostprocessBuild (buildTarget, path);
#endif
		
	}

	private static void PostprocessBuild (BuildTarget buildTarget, string path) {
		Debug.Log ("PostprocessBuild");

		string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
		PBXProject project = new PBXProject ();
		project.ReadFromString (File.ReadAllText (projectPath));

		string target = project.TargetGuidByName ("Unity-iPhone");

		// Required Frameworks
		project.AddFrameworkToProject (target, "SystemConfiguration.framework", false);
		project.AddFrameworkToProject (target, "Security.framework", false);
		project.AddFrameworkToProject (target, "AdSupport.framework", false);
		project.AddFrameworkToProject (target, "MobileCoreServices.framework", false);

		// Required Linker Flags
		project.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC"); 


		File.WriteAllText (projectPath, project.WriteToString ());
	}
}
#endif

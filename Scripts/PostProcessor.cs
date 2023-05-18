using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hibzz.PackageCreator
{
    public class PostProcessor : AssetPostprocessor
    {
		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			// Get the path where to initialize the git repository...
			// If there's no value found, we skip this process
			var git_init_path = EditorPrefs.GetString(PackageCreator.GIT_INIT_KEY, null);
			if(string.IsNullOrWhiteSpace(git_init_path)) { return; }

			// initialize the git repository
			PackageCreator.InitializeGitRepo(git_init_path);

			// now that the git repository has been initialized, we don't want
			// to initialize a repo every time a domain reload happens...
			// so clear the key
			EditorPrefs.DeleteKey(PackageCreator.GIT_INIT_KEY); 
		}
	}
}

using UnityEngine;
using UnityEditor;

namespace Hibzz.PackageCreator
{
	internal class PackageCreatorWindow : EditorWindow
	{
		public static void OpenPackageCreator()
		{
			// open package creator editor window
			var window = GetWindow<PackageCreatorWindow>();
			window.titleContent = new GUIContent("Package Creator");
			window.minSize = new Vector2(400, 180);
		}

		string packagename = "";
		string packagedescription = "";

		const float padding = 10;

		// drawn every frame
		void OnGUI()
		{
			Rect area = new Rect(padding, padding, position.width - padding * 2.0f, position.height - padding * 2.0f);
			GUILayout.BeginArea(area);

			// package name
			GUILayout.Space(5);
			packagename = EditorGUILayout.TextField("Package Name", packagename);

			// package description
			GUILayout.Space(10);
			EditorStyles.textField.wordWrap = true;
			EditorGUILayout.PrefixLabel("Description");
			packagedescription = EditorGUILayout.TextArea(packagedescription, GUILayout.Height(position.height - 110));
			EditorStyles.textField.wordWrap = false;

			// add a button
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("Create Package", GUILayout.Height(25)))
			{
				// close the window
				Close();

				// create package
				PackageCreator.CreatePackage(packagename, packagedescription);
			}

			GUILayout.EndArea();
		}
	}
}

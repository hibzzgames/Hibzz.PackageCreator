using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace Hibzz.PackageCreator
{
	public static class PackageCreator
	{
		const string TEMPLATE_PATH     = "Packages\\com.hibzz.packagecreator\\Templates";
		const string README_PATH       = TEMPLATE_PATH + "\\readme.template";
		const string PACKAGE_JSON_PATH = TEMPLATE_PATH + "\\package.json.template";
		const string LICENSE_PATH      = TEMPLATE_PATH + "\\license.template";
		const string ASMDEF_PATH       = TEMPLATE_PATH + "\\asmdef.template";

		public const string GIT_INIT_KEY = "HIBZZ_PC_GIT_INIT_PREFS_KEY";

		static string name = "";
		static string description = "";

		/// <summary>
		/// Create a loal package with the given name and description
		/// </summary>
		/// <param name="name">The name of the package</param>
		/// <param name="description">The description of the package</param>
		public static void CreatePackage(string name, string description)
		{
			// update global variable
			PackageCreator.name = name;
			PackageCreator.description = description;

			// create the folder
			var folder_name = $"com.hibzz.{name.ToLower()}";
			var package_path = Path.GetFullPath($"Packages\\{folder_name}");
			Directory.CreateDirectory(package_path);

			// create the readme.md based on the template
			var readme_content = ReadTemplate(README_PATH);
			File.WriteAllText($"{package_path}\\README.md", readme_content);

			// create package.json based on the template
			var package_json_content = ReadTemplate(PACKAGE_JSON_PATH);
			File.WriteAllText($"{package_path}\\package.json", package_json_content);

			// create license.md file based on the template
			var license_content = ReadTemplate(LICENSE_PATH);
			File.WriteAllText($"{package_path}\\LICENSE.md", license_content);

			// create the assembly definition based on the template
			var asmdef_content = ReadTemplate(ASMDEF_PATH);
			File.WriteAllText($"{package_path}\\com.hibzz.{name.ToLower()}.asmdef", asmdef_content);

			// Refresh asset database doesn't work and I can't figure out the
			// reason why... but anyways doing it so that in case in a later
			// patch unity fixes this issue
			AssetDatabase.Refresh();

			// set the editor prefs string letting the next refresh know to do
			// a git initialization
			EditorPrefs.SetString(GIT_INIT_KEY, package_path);
		}

		/// <summary>
		/// Reads the template file at the given and replaces the contents based on a specified schema
		/// </summary>
		/// <param name="path">The path to the template file</param>
		/// <returns>The contents of the template file with the fields appropriately replaced</returns>
		static string ReadTemplate(string path)
		{
			string content = File.ReadAllText(path);

			content = content.Replace("$Name", name);
			content = content.Replace("$LName", name.ToLower());
			content = content.Replace("$Description", description);
			content = content.Replace("$Year", $"{DateTime.Now.Year}");

			return content;
		}

		/// <summary>
		/// Initialize a git repo for the package
		/// </summary>
		/// <param name="path">The path to the newly created package</param>
		public static void InitializeGitRepo(string path)
		{
			// > git init .
			var init_process = new Process()
			{
				StartInfo = new ProcessStartInfo()
				{
					FileName = "git",
					Arguments = "init .",
					WorkingDirectory = path
				}
			};

			// > git add .
			var add_process = new Process()
			{
				StartInfo = new ProcessStartInfo()
				{
					FileName = "git",
					Arguments = "add .",
					WorkingDirectory = path
				}
			};

			// > git commit -m "Initial commit"
			var commit_process = new Process()
			{
				StartInfo = new ProcessStartInfo()
				{
					FileName = "git",
					Arguments = "commit -m \"Initial commit\"",
					WorkingDirectory = path
				}
			};

			// run the processes in a sequence
			init_process.Start();
			init_process.WaitForExit();

			add_process.Start();
			add_process.WaitForExit();
			
			commit_process.Start();
			commit_process.WaitForExit();
		}
	}
}

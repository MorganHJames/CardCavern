////////////////////////////////////////////////////////////
// File: KeywordReplace.cs
// Author: Morgan Henry James
// Date Created: 30-09-2019
// Brief: This script allows the use of Creation date to be 
// auto filled in using my custom new script template.
//////////////////////////////////////////////////////////// 

using UnityEngine;
using UnityEditor;

/// <summary>
/// Replaces the keyword #CREATIONDATE# on script creation
/// </summary>
public class KeywordReplace : UnityEditor.AssetModificationProcessor
{
	#region Methods
	#region Public
	/// <summary>
	/// When a new script is created goes through the script created 
	/// with the new script template and replace the keyword #CREATIONDATE#
	/// with the date that the script was created.
	/// </summary>
	/// <param name="path">The path to the asset just created.</param>
	public static void OnWillCreateAsset(string path)
	{
		path = path.Replace(".meta", "");
		int index = path.LastIndexOf(".");
		string file = path.Substring(index);
		if (file != ".cs" && file != ".js" && file != ".boo") return;
		index = Application.dataPath.LastIndexOf("Assets");
		path = Application.dataPath.Substring(0, index) + path;
		file = System.IO.File.ReadAllText(path);
		file = file.Replace("#CREATIONDATE#", System.DateTime.Now.ToString("dd-MM-yyy"));
		System.IO.File.WriteAllText(path, file);
		AssetDatabase.Refresh();
	}
	#endregion
	#endregion
}
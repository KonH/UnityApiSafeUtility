using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class ApiSafeUtility {

	static string LogsPath { 
		get {
			return Path.Combine("Assets", "Logs");
	 	}
	}

	static StringBuilder _sb = null;

	[MenuItem("Api Safe/Test")]
	static void Test() {
		var assemblies = new string[] {
			"mscorlib",
			"UnityEditor",
			"UnityEngine",
			"Assembly-CSharp",
			"Assembly-CSharp-Editor"
		};
		WriteAssembliesInfo(assemblies);
		AssetDatabase.Refresh();
	}

	static void WriteAssembliesInfo(string[] assemblyNames) {
		foreach ( var assemblyName in assemblyNames ) {
			WriteAssemblyInfo(assemblyName);
		}
	}

	static void WriteAssemblyInfo(string assemblyName) {
		var assembly = Assembly.Load(assemblyName);
		if ( assembly == null ) {
			Debug.LogErrorFormat("Can't load assembly: {0}", assemblyName);
			return;
		}
		_sb = new StringBuilder();
		_sb.AppendFormat("ASSEMBLY: {0}\n", assembly.FullName);
		
		var allTypes = assembly.GetTypes();
		var publicTypes = assembly.GetExportedTypes();
		var privateTypes = FindPrivateTypes(allTypes, publicTypes);

		_sb.Append("PUBLIC_TYPES:\n");
		WriteTypesInfo(publicTypes);
		_sb.Append("\n");

		_sb.Append("PRIVATE_TYPES:\n");
		WriteTypesInfo(privateTypes);
		_sb.Append("\n");

		var text = _sb.ToString();
		Debug.Log(text);
		WriteToFile(assembly.GetName().Name + ".txt", text);
	}

	static Type[] FindPrivateTypes(Type[] allTypes, Type[] publicTypes) {
		var privateTypes = new List<Type>();
		foreach ( var type in allTypes ) {
			var isPublic = false;
			foreach ( var publicType in publicTypes ) {
				if( type == publicType ) {
					isPublic = true;
					break;
				}
			}
			if( !isPublic ) {
				privateTypes.Add(type);
			}
		}
		return privateTypes.ToArray();
	}

	static void WriteToFile(string filename, string text) {
		if ( !Directory.Exists(LogsPath) ) {
			Directory.CreateDirectory(LogsPath);
		}
		var path = Path.Combine(LogsPath, filename);
		File.WriteAllText(path, text);
	}

	static void WriteTypesInfo(Type[] types) {
 		foreach ( var type in types ) {
			WriteTypeInfo(type);
			_sb.Append("\n");
		}
	}
	static void WriteTypeInfo(Type type) {
		_sb.AppendFormat("TYPE: {0}", type.FullName);
	}
}

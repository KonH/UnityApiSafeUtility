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
		
		var publicTypes = assembly.GetExportedTypes();

		_sb.Append("TYPES:\n");
		WriteTypesInfo(publicTypes);
		_sb.Append("\n");
		
		var text = _sb.ToString();
		Debug.Log(text);
		WriteToFile(assembly.GetName().Name + ".txt", text);
	}

	public static Type[] SelectPrivateTypes(Type[] allTypes, Type[] publicTypes) {
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
		_sb.Append("\n");
		WriteFieldsInfo(type.GetFields());
		WritePropertiesInfo(type.GetProperties());
		WriteConstructorsInfo(type.GetConstructors());
		WriteMethodsInfo(type.GetMethods());
	}

	static void WriteFieldsInfo(FieldInfo[] fields) {
		foreach ( var field in fields ) {
			WriteFieldInfo(field);
			_sb.Append("\n");
		}
	}

	static void WriteFieldInfo(FieldInfo field) {
		var fieldUsage = field.IsStatic ? "STATIC" : "INSTANCE";
		_sb.AppendFormat("{0} FIELD: {1} {2}", fieldUsage, field.FieldType, field.Name);
	}

	static void WritePropertiesInfo(PropertyInfo[] properties) {
		foreach ( var prop in properties ) {
			WritePropertyInfo(prop);
			_sb.Append("\n");
		}
	}

	static void WritePropertyInfo(PropertyInfo property) {
		var getMethod = property.GetGetMethod();
		var setMethod = property.GetSetMethod();
		var access = string.Format(
			"{0}{1}", 
			((getMethod != null) && getMethod.IsPublic) ? "get;" : "", 
			((setMethod != null) && setMethod.IsPublic) ? "set;" : "");
		_sb.AppendFormat("PROPERTY: {0} {1}({2})", property.PropertyType, property.Name, access);
	}

	static void WriteConstructorsInfo(ConstructorInfo[] constructors) {
		foreach ( var constr in constructors ) {
			WriteConstructorInfo(constr);
			_sb.Append("\n");
		}
	}

	static void WriteConstructorInfo(ConstructorInfo constructor) {
		// TODO: Args
		var constrUsage = constructor.IsStatic ? "STATIC" : "INSTANCE";
		_sb.AppendFormat("{0} CONSTRUCTOR", constrUsage);
		ExtractParametersInfo(constructor.GetParameters());
	}

	static void WriteMethodsInfo(MethodInfo[] methods) {
		foreach ( var method in methods ) {
			WriteMethodInfo(method);
			_sb.Append("\n");
		}
	}

	static void WriteMethodInfo(MethodInfo method) {
		var methodUsage = method.IsStatic ? "STATIC" : "INSTANCE";
		_sb.AppendFormat("{0} METHOD: {1} {2}", methodUsage, method.ReturnType, method.Name);
		ExtractParametersInfo(method.GetParameters());
	}

	static void ExtractParametersInfo(ParameterInfo[] parameters) {
		_sb.Append("(");
		for ( int i = 0; i < parameters.Length; i++ ) {
			var param = parameters[i];
			_sb.AppendFormat("{0} {1}", param.ParameterType, param.Name);
			if ( i < parameters.Length - 1 ) {
				_sb.Append(", ");
			}
		}
		_sb.Append(")");
	}
}

﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ThunderKit.Common.Configuration;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace ThunderKit.Common.Package
{
    public static class PackageHelper
    {
        public static string nl => Environment.NewLine;

        public static void DownloadPackage(string url, string filePath)
        {
            var webRequest = UnityWebRequest.Get(url);
            var asyncOpRequest = webRequest.SendWebRequest();
            void Request_completed(AsyncOperation obj)
            {
                if (webRequest.isNetworkError || webRequest.isHttpError)
                    Debug.Log(webRequest.error);
                else
                    File.WriteAllBytes(Path.ChangeExtension(filePath, "dl"), webRequest.downloadHandler.data);

                if (File.Exists(filePath)) File.Delete(filePath);
                File.Move(Path.ChangeExtension(filePath, "dl"), filePath);
            }
            asyncOpRequest.completed += Request_completed;
        }

        public static async Task DownloadPackageAsync(string url, string filePath)
        {
            await Task.Run(() => {
                var webRequest = UnityWebRequest.Get(url);
                var asyncOpRequest = webRequest.SendWebRequest();
                asyncOpRequest.completed += Request_completed;
                void Request_completed(AsyncOperation obj)
                {
                    if (webRequest.isNetworkError || webRequest.isHttpError)
                        Debug.Log(webRequest.error);
                    else
                        File.WriteAllBytes(Path.ChangeExtension(filePath, "dl"), webRequest.downloadHandler.data);

                    if (File.Exists(filePath)) File.Delete(filePath);
                    File.Move(Path.ChangeExtension(filePath, "dl"), filePath);
                }
            });
        }

        public static void GeneratePackageManifest(string packageName, string outputDir, string modName, string authorAlias, string modVersion, string description = null)
        {
            string unityVersion = Application.unityVersion.Substring(0, Application.unityVersion.LastIndexOf("."));
            var author = new Author
            {
                name = authorAlias,
            };
            var packageManifest = new PackageManagerManifest(author, packageName, ObjectNames.NicifyVariableName(modName), modVersion, unityVersion, description);
            var packageManifestJson = JsonUtility.ToJson(packageManifest);
            ScriptingSymbolManager.AddScriptingDefine(packageName);
            File.WriteAllText(Path.Combine(outputDir, "package.json"), packageManifestJson);
        }

        public static PackageManagerManifest GetPackageManagerManifest(string directory)
        {
            var packageJsonPath = Path.Combine(directory, "package.json");
            var json = File.ReadAllText(packageJsonPath);
            var pmm = JsonUtility.FromJson<PackageManagerManifest>(json);
            return pmm;
        }

        public static void WriteAssemblyMetaData(string assemblyPath, string metadataPath)
        {
            string guid = PackageHelper.GetAssemblyHash(assemblyPath);
            string metaData = PackageHelper.DefaultAssemblyMetaData(guid);
            if (File.Exists(metadataPath)) File.Delete(metadataPath);
            File.WriteAllText(metadataPath, metaData);
        }

        public static string GetAssemblyHash(string assemblyPath)
        {
            using (var md5 = MD5.Create())
            {
                string shortName = Path.GetFileNameWithoutExtension(assemblyPath);
                byte[] shortNameBytes = Encoding.Default.GetBytes(shortName);
                var shortNameHash = md5.ComputeHash(shortNameBytes);
                var guid = new Guid(shortNameHash);
                var cleanedGuid = guid.ToString().ToLower().Replace("-", "");
                return cleanedGuid;
            }
        }

        public static string DefaultAssemblyMetaData(string guid) =>
"fileFormatVersion: 2"
+ nl + $"guid: {guid}"
+ nl + "PluginImporter:"
+ nl + "  externalObjects: {}"
+ nl + "  serializedVersion: 2"
+ nl + "  iconMap: {}"
+ nl + "  executionOrder: {}"
+ nl + "  defineConstraints: []"
+ nl + "  isPreloaded: 0"
+ nl + "  isOverridable: 0"
+ nl + "  isExplicitlyReferenced: 1"
+ nl + "  validateReferences: 1"
+ nl + "  platformData:"
+ nl + "  - first:"
+ nl + "      '': Any"
+ nl + "    second:"
+ nl + "      enabled: 0"
+ nl + "      settings:"
+ nl + "        Exclude Editor: 0"
+ nl + "        Exclude Linux: 0"
+ nl + "        Exclude Linux64: 0"
+ nl + "        Exclude LinuxUniversal: 0"
+ nl + "        Exclude OSXUniversal: 0"
+ nl + "        Exclude Win: 0"
+ nl + "        Exclude Win64: 0"
+ nl + "  - first:"
+ nl + "      Any: "
+ nl + "    second:"
+ nl + "      enabled: 1"
+ nl + "      settings: {}"
+ nl + "  - first:"
+ nl + "      Editor: Editor"
+ nl + "    second:"
+ nl + "      enabled: 1"
+ nl + "      settings:"
+ nl + "        CPU: AnyCPU"
+ nl + "        DefaultValueInitialized: true"
+ nl + "        OS: AnyOS"
+ nl + "  - first:"
+ nl + "      Facebook: Win"
+ nl + "    second:"
+ nl + "      enabled: 0"
+ nl + "      settings:"
+ nl + "        CPU: AnyCPU"
+ nl + "  - first:"
+ nl + "      Facebook: Win64"
+ nl + "    second:"
+ nl + "      enabled: 0"
+ nl + "      settings:"
+ nl + "        CPU: AnyCPU"
+ nl + "  - first:"
+ nl + "      Standalone: Linux"
+ nl + "    second:"
+ nl + "      enabled: 1"
+ nl + "      settings:"
+ nl + "        CPU: x86"
+ nl + "  - first:"
+ nl + "      Standalone: Linux64"
+ nl + "    second:"
+ nl + "      enabled: 1"
+ nl + "      settings:"
+ nl + "        CPU: x86_64"
+ nl + "  - first:"
+ nl + "      Standalone: LinuxUniversal"
+ nl + "    second:"
+ nl + "      enabled: 1"
+ nl + "      settings: {}"
+ nl + "  - first:"
+ nl + "      Standalone: OSXUniversal"
+ nl + "    second:"
+ nl + "      enabled: 1"
+ nl + "      settings:"
+ nl + "        CPU: AnyCPU"
+ nl + "  - first:"
+ nl + "      Standalone: Win"
+ nl + "    second:"
+ nl + "      enabled: 1"
+ nl + "      settings:"
+ nl + "        CPU: AnyCPU"
+ nl + "  - first:"
+ nl + "      Standalone: Win64"
+ nl + "    second:"
+ nl + "      enabled: 1"
+ nl + "      settings:"
+ nl + "        CPU: AnyCPU"
+ nl + "  - first:"
+ nl + "      Windows Store Apps: WindowsStoreApps"
+ nl + "    second:"
+ nl + "      enabled: 0"
+ nl + "      settings:"
+ nl + "        CPU: AnyCPU"
+ nl + "  userData: "
+ nl + "  assetBundleName: "
+ nl + "  assetBundleVariant: ";
    }
}
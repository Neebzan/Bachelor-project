using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace ChecksumHandlerLib
{
    public class GetFilesDictionaryProgressEventArgs : EventArgs
    {
        public int FilesFound { get; set; }
        public int ChecksumsGenerated { get; set; }
    }

    public class ChecksumTool
    {

        public static void HelloWorld()
        {
            Console.WriteLine("Hello World!");
        }

        public static event EventHandler<GetFilesDictionaryProgressEventArgs> GetFilesDictionaryProgress = delegate { };
        private static void OnGetFilesDictionaryProgress(GetFilesDictionaryProgressEventArgs e)
        {
            var handler = GetFilesDictionaryProgress;
            handler(null, e);
        }

        public static InstallationDataModel GetInstalledVersion(string installPath)
        {
            var install = GetInstallationAtPath(installPath);
            var tempVersionInfo = install.ElementAt(0);
            string path = SanitizePath(installPath);

            //Get data from the VersionFile
            InstallationDataModel tempModel = InstallationDataModel.GetModelFromFile(path);

            if (tempModel == null)
            {
                tempModel = new InstallationDataModel();
                Console.WriteLine("NO VersionInfo FOUND!!");
            }
            tempModel.InstallPath = installPath;


            tempModel.InstallPath = path;

            tempModel.Files.Clear();
            tempModel.MissingFiles.Clear();


            foreach (var item in tempVersionInfo.Value)
            {
                tempModel.Files.Add(new FileModel()
                {
                    FileChecksum = item.Value,
                    FilePath = item.Key
                });
            }

            tempModel.InstallationChecksum = GetCombinedChecksum(SanitizePath(installPath));
            tempModel.SaveToFile();

            return tempModel;
        }

        public static List<InstallationDataModel> GetInstalledVersions(string installPath)
        {
            var installs = ChecksumTool.GetInstallationsAtPath(installPath);
            List<InstallationDataModel> installedVersions = new List<InstallationDataModel>();
            for (int i = 0; i < installs.Count; i++)
            {
                var tempVersionInfo = installs.ElementAt(i);
                string path = SanitizePath(installPath + "\\" + tempVersionInfo.Key);

                //Get data from the VersionFile
                InstallationDataModel tempModel = InstallationDataModel.GetModelFromFile(path);

                if (tempModel == null)
                {
                    tempModel = new InstallationDataModel();
                    Console.WriteLine("NO VersionInfo FOUND!!");
                }

                tempModel.InstallPath = path;
                tempModel.Files.Clear();
                tempModel.MissingFiles.Clear();

                //tempModel.VersionName = tempVersionInfo.Key;

                foreach (var item in tempVersionInfo.Value)
                {
                    tempModel.Files.Add(new FileModel()
                    {
                        FileChecksum = item.Value,
                        FilePath = item.Key
                    });
                }

                tempModel.InstallationChecksum = GetCombinedChecksum(tempModel.InstallPath);

                tempModel.SaveToFile();
                installedVersions.Add(tempModel);
            }


            return installedVersions;
        }

        /// <summary>
        /// Returns a list of paths if there are any different or missing files from the master
        /// </summary>
        /// <param name="masterFiles">The dictionary containing the valid checksums</param>
        /// <param name="filesToCompare">The dictionary to be checked against the master</param>
        /// <returns></returns>
        public static List<string> CompareFileDictionaries(Dictionary<string, string> masterFiles, Dictionary<string, string> filesToCompare)
        {
            List<string> result = new List<string>();

            foreach (var item in masterFiles)
            {
                string key;
                if (filesToCompare.TryGetValue(item.Key, out key))
                {
                    if (key != item.Value)
                        result.Add(item.Key);
                }
                else
                    result.Add(item.Key);
            }

            return result;
        }

        public static void GetFilesDictionary()
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string[] files = Directory.GetFiles(currentDirectory, "*", SearchOption.AllDirectories);

            Dictionary<string, string> validFiles = new Dictionary<string, string>();

            for (int i = 0; i < files.Length; i++)
            {
                validFiles.Add(GetRelativePath(files[i], currentDirectory), GetChecksum(files[i]));
            }
        }


        /// <summary>
        /// Generate a dictionary from a collection of file paths and a base directory
        /// Progress can be fetched from the OnGetFilesDictionaryProgress event
        /// </summary>
        /// <param name="files"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static void GetFilesDictionary(out Dictionary<string, string> result, string path = "")
        {
            GetFilesDictionaryProgressEventArgs args = new GetFilesDictionaryProgressEventArgs();
            result = null;
            path = RootedPathCheck(path);

            try
            {
                string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories).Where(name => !(name == "VersionInfo")).ToArray();
                args.FilesFound = files.Length;
                args.ChecksumsGenerated = 0;
                OnGetFilesDictionaryProgress(args);
                Dictionary<string, string> validFilesDictionary = new Dictionary<string, string>();
                for (int i = 0; i < files.Length; i++)
                {
                    string relPath = GetRelativePath(files[i], path);
                    validFilesDictionary.Add(relPath, GetChecksum(files[i], relPath));
                    args.ChecksumsGenerated++;
                    OnGetFilesDictionaryProgress(args);
                }
                result = validFilesDictionary;
            }
            catch (Exception e)
            {
                Console.WriteLine("Not a valid path OR path not found");
                Console.WriteLine("Path was: \n" + path);
                Console.WriteLine(e.Message);

            }
        }

        /// <summary>
        /// Generate a dictionary from a collection of file paths and a base directory
        /// </summary>
        /// <param name="files"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetFilesDictionary(string[] files, string directory)
        {
            Dictionary<string, string> validFiles = new Dictionary<string, string>();

            for (int i = 0; i < files.Length; i++)
            {
                string relPath = GetRelativePath(files[i], directory);
                validFiles.Add(relPath, GetChecksum(files[i]));
            }

            return validFiles;
        }

        public static string[] GetAvailableFolders(string path = @"")
        {
            path = RootedPathCheck(path);
            string[] result = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = GetRelativePath(result[i], path);
            }

            return result;
        }

        /// <summary>
        /// Returns the relative path to a file, based on a directory
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        private static string GetRelativePath(string filePath, string directory)
        {
            System.Uri uri1 = new Uri(filePath);

            System.Uri uri2;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                uri2 = new Uri(directory + "/");
            else
                uri2 = new Uri(directory + "\\");

            //return Path.GetFileName(filePath);
            string t = uri2.MakeRelativeUri(uri1).ToString();
            return Uri.UnescapeDataString(uri2.MakeRelativeUri(uri1).ToString());
        }

        public static string GetCombinedChecksum(string path)
        {
            path = RootedPathCheck(path);
            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories).OrderBy(p => p).Where(name => !(name.Contains("VersionInfo"))).ToList();
            using (MD5 md5 = MD5.Create())
            {
                //If the folder is empty, generate a hash based on a 0 byte
                if (files.Count == 0)
                {
                    md5.ComputeHash(new byte[0]);
                }
                else
                    for (int i = 0; i < files.Count; i++)
                    {
                        string relPath = GetRelativePath(files[i], path);
                        byte[] nameBytes = Encoding.ASCII.GetBytes(relPath);
                        using (FileStream stream = File.OpenRead(files[i]))
                        {
                            byte[] checksum = md5.ComputeHash(stream);

                            md5.TransformBlock(nameBytes, 0, nameBytes.Length, nameBytes, 0);
                            if (i == files.Count - 1)
                                md5.TransformFinalBlock(checksum, 0, checksum.Length);
                            else
                                md5.TransformBlock(checksum, 0, checksum.Length, checksum, 0);
                        }
                    }
                return BitConverter.ToString(md5.Hash).Replace("-", String.Empty);
            }
        }


        /// <summary>
        /// Generate checksum from a single file path
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static string GetChecksum(string filePath, string relPath = "")
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                using(MD5 md5 = MD5.Create())
                {
                    byte[] checksum = md5.ComputeHash(stream);
                    byte[] nameBytes = Encoding.ASCII.GetBytes(relPath);

                    md5.TransformBlock(nameBytes, 0, nameBytes.Length, nameBytes, 0);
                    md5.TransformFinalBlock(checksum, 0, checksum.Length);

                    return BitConverter.ToString(checksum).Replace("-", String.Empty);
                }   
            }
            //using (FileStream stream = File.OpenRead(filePath))
            //{
            //    var sha = new SHA256Managed();
            //    byte[] checksum = sha.ComputeHash(stream);
            //    byte[] nameBytes = Encoding.ASCII.GetBytes(relPath);

            //    sha.TransformBlock(nameBytes, 0, nameBytes.Length, nameBytes, 0);
            //    sha.TransformFinalBlock(checksum, 0, checksum.Length);

            //    return BitConverter.ToString(checksum).Replace("-", String.Empty);
            //}
        }

        public static string RootedPathCheck(string path)
        {
            //Console.WriteLine("Checking if path is rooted: " + path);
            if (!Path.IsPathRooted(path))
            {
                //Console.WriteLine("Path was NOT rooted");
                string currentDirectory;
                currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                //Console.WriteLine("Current dir is: " + currentDirectory);
                if (path != "")
                {
                    if (currentDirectory[currentDirectory.Length - 1] != '\\')
                        currentDirectory += "\\";

                    currentDirectory += path;
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    currentDirectory = currentDirectory.Replace('\\', '/');

                //Console.WriteLine("Final dir is: " + currentDirectory);
                return currentDirectory;
            }
            else
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    path = path.Replace('\\', '/');
                return path;
            }
        }

        public static Dictionary<string, Dictionary<string, string>> GetInstallationsAtPath(string path)
        {
            Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();
            if (Directory.Exists(path))
            {
                string[] foldersFound = GetAvailableFolders(path);

                for (int i = 0; i < foldersFound.Length; i++)
                {
                    Console.WriteLine("Find files for: " + foldersFound[i]);
                    Console.WriteLine("At path: " + path + @"\" + foldersFound[i]);
                    Dictionary<string, string> temp = new Dictionary<string, string>();
                    if (path != "")
                        GetFilesDictionary(out temp, path + @"\" + foldersFound[i]);
                    else
                        GetFilesDictionary(out temp, foldersFound[i]);
                    result.Add(foldersFound[i], temp);
                }
            }
            else
                Console.WriteLine("Path does not exist!: " + path);
            return result;
        }


        public static Dictionary<string, Dictionary<string, string>> GetInstallationAtPath(string path)
        {
            Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();
            if (Directory.Exists(path))
            {

                Dictionary<string, string> temp = new Dictionary<string, string>();
                GetFilesDictionary(out temp, path);

                result.Add(path.Split(Path.DirectorySeparatorChar).Last(), temp);
            }

            return result;
        }

        private static string SanitizePath(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                path = path.Replace('\\', '/');

            return path;
        }
    }
}

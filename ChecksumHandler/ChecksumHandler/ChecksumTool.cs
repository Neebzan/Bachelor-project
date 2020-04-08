using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

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
                string value;
                if (filesToCompare.TryGetValue(item.Key, out value))
                {
                    if (value != item.Value)
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
            string[] files = Directory.GetFiles(currentDirectory, "*.*", SearchOption.AllDirectories);

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
            //string currentDirectory;
            //if (!Path.IsPathRooted(path))
            //{
            //    currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            //    if (path != "")
            //    {
            //        if (currentDirectory[currentDirectory.Length - 1] != '\\')
            //            currentDirectory += "\\";

            //        currentDirectory += path;
            //    }
            //}
            //else
            //    currentDirectory = path;

            try
            {
                string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                args.FilesFound = files.Length;
                args.ChecksumsGenerated = 0;
                OnGetFilesDictionaryProgress(args);
                Dictionary<string, string> validFilesDictionary = new Dictionary<string, string>();
                for (int i = 0; i < files.Length; i++)
                {
                    validFilesDictionary.Add(GetRelativePath(files[i], path), GetChecksum(files[i]));
                    args.ChecksumsGenerated++;
                    OnGetFilesDictionaryProgress(args);
                }
                result = validFilesDictionary;
            }
            catch (Exception e)
            {
                Console.WriteLine("Not a valid path / Path not found");
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
                validFiles.Add(GetRelativePath(files[i], directory), GetChecksum(files[i]));
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

        /// <summary>
        /// Generate checksums for a collection of file paths
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private static string[] GetChecksums(string[] files)
        {
            string[] checkSums = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                using (FileStream stream = File.OpenRead(files[i]))
                {
                    var sha = new SHA256Managed();
                    byte[] checksum = sha.ComputeHash(stream);
                    checkSums[i] = BitConverter.ToString(checksum).Replace("-", String.Empty);
                }
            }
            return checkSums;
        }

        /// <summary>
        /// Generate checksum from a single file path
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static string GetChecksum(string filePath)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                var sha = new SHA256Managed();
                byte[] checksum = sha.ComputeHash(stream);
                return BitConverter.ToString(checksum).Replace("-", String.Empty);
            }
        }

        private static string RootedPathCheck(string path)
        {
            Console.WriteLine("Checking if path is rooted: " + path);
            if (!Path.IsPathRooted(path))
            {
                Console.WriteLine("Path was NOT rooted");
                string currentDirectory;
                currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                Console.WriteLine("Current dir is: " + currentDirectory);
                if (path != "")
                {
                    if (currentDirectory[currentDirectory.Length - 1] != '\\')
                        currentDirectory += "\\";

                    currentDirectory += path;
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    currentDirectory = currentDirectory.Replace('\\', '/');

                Console.WriteLine("Final dir is: " + currentDirectory);
                return currentDirectory;
            }
            else
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    path = path.Replace('\\', '/');
                return path;
            }
        }
    }
}

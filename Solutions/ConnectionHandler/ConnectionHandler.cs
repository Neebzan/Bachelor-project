﻿using HelperTools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ConnectionHandlerLib
{
    public static class ConnectionHandler
    {
        /// <summary>
        /// Takes in a string, converts to byte array, adds a integer header before message, which value is the length of the byte message. Then returns the full package
        /// </summary>
        /// <param name="_message"></param>
        /// <returns></returns>
        public static byte[] ConvertToBytes(string _message)
        {
            byte[] packageData = System.Text.Encoding.ASCII.GetBytes(_message);

            byte[] totalPackage = AddSizeHeaderToPackage(packageData);

            return totalPackage;
        }

        /// <summary>
        /// Serializes to JSON, converts to byte array, and adds a header to the package, consisting of an integer value of the byte length of the orignial message
        /// </summary>
        /// <typeparam name="T">The type of object to JSON serialize</typeparam>
        /// <param name="obj">The object to serialize as a message</param>
        /// <returns></returns>
        public static byte[] ConvertToBytes<T>(T obj, Formatting format = 0)
        {
            string packageJson = JsonConvert.SerializeObject(obj, format, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string msg = packageJson;
            //Convert to JSON
            byte[] packageData = System.Text.Encoding.ASCII.GetBytes(msg);

            byte[] totalPackage = AddSizeHeaderToPackage(packageData);

            return totalPackage;
        }

        private static byte[] AddSizeHeaderToPackage(byte[] _package)
        {
            //Create a uint containing the length of the package, and encode to byte array
            int pckLen = _package.Length;
            byte[] packageHeader = BitConverter.GetBytes(pckLen);
            byte[] totalPackage = new byte[packageHeader.Length + _package.Length];
            //Merge byte arrays
            System.Buffer.BlockCopy(packageHeader, 0, totalPackage, 0, packageHeader.Length);
            System.Buffer.BlockCopy(_package, 0, totalPackage, packageHeader.Length, _package.Length);

            return totalPackage;
        }

        public static void SendObject<T>(T obj, TcpClient sender)
        {
            byte[] data = ConvertToBytes<T>(obj);
            sender.GetStream().Write(data, 0, data.Length);
        }
        public static async Task SendObjectAsync<T>(T obj, TcpClient sender)
        {
            byte[] data = ConvertToBytes<T>(obj);
            await sender.GetStream().WriteAsync(data, 0, data.Length);
        }

        public static async Task<bool> ReadFileAsync(TcpClient client, string fileName, string saveDirectory = "")
        {
            byte[] readBuffer = new byte[4];

            if (saveDirectory != "")
            {
                if (saveDirectory[saveDirectory.Length - 1] != '\\')
                    saveDirectory += "\\";
            }

            int bytesRead = 0;

            while (bytesRead < 4)
            {
                bytesRead += await client.GetStream().ReadAsync(readBuffer, bytesRead, 4 - bytesRead);
            }

            int totalFileSize = BitConverter.ToInt32(readBuffer, 0);

            Console.WriteLine(ConsoleExtension.AddTimestamp("Incoming file size: " + totalFileSize));

            //Create subfolders if needed
            string[] pathSplit = fileName.Split('/');
            string subfolders = fileName.Replace(pathSplit[pathSplit.Length - 1], "");
            Directory.CreateDirectory(saveDirectory + subfolders);

            using (var output = File.Create(saveDirectory + fileName))
            {

                // read the file in chunks of 1KB
                var buffer = new byte[2048];
                bytesRead = 0; bytesRead = 0;
                int totalBytesRead = 0;

                //client.Client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(FileRead_Callback), null);

                while (totalBytesRead < totalFileSize)
                {
                    //bytesRead = client.Client.Receive(buffer, buffer.Length, SocketFlags.None);
                    bytesRead = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);

                    //bytesRead = client.Client.Receive(buffer, buffer.Length, SocketFlags.None);
                    output.Write(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;
                }
                return true;
            }

            return false;
        }

        public static void FileRead_Callback(IAsyncResult ar)
        {

        }

        public static bool ReadFile(TcpClient client, string fileName, string saveDirectory = "")
        {
            byte[] readBuffer = new byte[4];

            if (saveDirectory != "")
            {
                if (saveDirectory[saveDirectory.Length - 1] != '\\')
                    saveDirectory += "\\";
            }

            while (client.GetStream().DataAvailable)
            {
                int bytesRead = 0;

                while (bytesRead < 4)
                {
                    bytesRead += client.GetStream().Read(readBuffer, bytesRead, 4 - bytesRead);
                }

                int totalFileSize = BitConverter.ToInt32(readBuffer, 0);

                //Create subfolders if needed
                string[] pathSplit = fileName.Split('/');
                string subfolders = fileName.Replace(pathSplit[pathSplit.Length - 1], "");
                Directory.CreateDirectory(saveDirectory + subfolders);

                using (var output = File.Create(saveDirectory + fileName))
                {
                    // read the file in chunks of 1KB
                    var buffer = new byte[1024];
                    bytesRead = 0;
                    int totalBytesRead = 0;
                    while (totalBytesRead < totalFileSize)
                    {
                        bytesRead = client.Client.Receive(buffer, buffer.Length, SocketFlags.None);
                        output.Write(buffer, 0, bytesRead);
                        totalBytesRead += bytesRead;
                    }
                    return true;
                }
            }
            return false;
        }

        public static bool ReadFileStream(TcpClient client, string fileName, long fileSize, string saveDirectory = "")
        {
            byte[] readBuffer = new byte[4];

            if (saveDirectory != "")
            {
                if (saveDirectory[saveDirectory.Length - 1] != '\\')
                    saveDirectory += "\\";
            }
            while (Connected(client))
            {

                while (client.GetStream().DataAvailable)
                {
                    int bytesRead = 0;

                    while (bytesRead < 4)
                    {
                        bytesRead += client.GetStream().Read(readBuffer, bytesRead, 4 - bytesRead);
                    }

                    int totalFileSize = BitConverter.ToInt32(readBuffer, 0);

                    //Create subfolders if needed
                    string[] pathSplit = fileName.Split('/');
                    string subfolders = fileName.Replace(pathSplit[pathSplit.Length - 1], "");
                    Directory.CreateDirectory(saveDirectory + subfolders);

                    using (var output = File.Create(saveDirectory + fileName))
                    {

                        // read the file in chunks of 1KB
                        var buffer = new byte[1024];
                        bytesRead = 0;
                        //int totalBytesRead = 0;
                        int remainingBytes = totalFileSize;

                        while (remainingBytes > 0)
                        {
                            if (remainingBytes > buffer.Length)
                                bytesRead = client.Client.Receive(buffer, buffer.Length, SocketFlags.None);
                            else
                                bytesRead = client.Client.Receive(buffer, remainingBytes, SocketFlags.None);
                            output.Write(buffer, 0, bytesRead);
                            remainingBytes -= bytesRead;
                        }
                        return true;

                    }
                }

            }
            return false;
        }

        public static bool Connected(TcpClient tcpClient)
        {
            try
            {
                if (tcpClient.Client != null && tcpClient.Client.Connected)
                {
                    if (tcpClient.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];

                        if (tcpClient.Client.Receive(buff, SocketFlags.Peek) == 0)
                        {
                            return false;
                        }

                        return true;
                    }

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Reads the stream once for a message. If there is a message, reads first 4 bytes of integer length of message, then reads until the length of message has been read
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static IEnumerable<string> ReadForMessages(NetworkStream stream)
        {
            string msg = string.Empty;

            byte[] readBuffer = new byte[4];

            while (stream.DataAvailable)
            {
                int bytesRead = 0;

                while (bytesRead < 4)
                {
                    bytesRead += stream.Read(readBuffer, bytesRead, 4 - bytesRead);
                }

                bytesRead = 0;
                byte[] buffer = new byte[BitConverter.ToInt32(readBuffer, 0)];

                while (bytesRead < buffer.Length)
                {
                    bytesRead += stream.Read(buffer, bytesRead, buffer.Length - bytesRead);
                }
                msg = System.Text.Encoding.UTF8.GetString(buffer);
                yield return msg;
            }
            yield return null;

        }

        /// <summary>
        /// Reads the stream once for a message. If there is a message, reads first 4 bytes of integer length of message, then reads until the length of message has been read
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<string> ReadMessageAsync(NetworkStream stream)
        {
            string msg = string.Empty;

            byte[] readBuffer = new byte[4];


            int bytesRead = 0;

            while (bytesRead < 4)
            {
                bytesRead += await stream.ReadAsync(readBuffer, bytesRead, 4 - bytesRead);
                //bytesRead += stream.Read(readBuffer, bytesRead, 4 - bytesRead);
            }

            bytesRead = 0;
            byte[] buffer = new byte[BitConverter.ToInt32(readBuffer, 0)];

            while (bytesRead < buffer.Length)
            {
                bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
            }
            msg = System.Text.Encoding.UTF8.GetString(buffer);

            return msg;
        }

        /// <summary>
        /// Reads the stream once for a message. If there is a message, reads first 4 bytes of integer length of message, then reads until the length of message has been read
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ReadMessage(NetworkStream stream)
        {
            string msg = string.Empty;

            byte[] readBuffer = new byte[4];

            if (stream.DataAvailable)
            {
                int bytesRead = 0;

                while (bytesRead < 4)
                {
                    bytesRead += stream.Read(readBuffer, bytesRead, 4 - bytesRead);
                }

                bytesRead = 0;
                byte[] buffer = new byte[BitConverter.ToInt32(readBuffer, 0)];

                while (bytesRead < buffer.Length)
                {
                    bytesRead += stream.Read(buffer, bytesRead, buffer.Length - bytesRead);
                }
                msg = System.Text.Encoding.UTF8.GetString(buffer);
            }
            return msg;
        }
    }
}

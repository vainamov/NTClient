using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NTClient
{
    internal class Program
    {
        private static string rootDirectory = @"C:/";

        private static void Main(string[] args) {

            var cn = new XmlDocument().CreateElement("client");
            cn.SetAttribute("name", Environment.MachineName);

            foreach (var directory in GetDirectories(rootDirectory)) {
                var xdoc = new XmlDocument();

                var imported = xdoc.ImportNode(cn, true);
                var cn2 = xdoc.AppendChild(imported);

                var dn = xdoc.CreateElement("verzeichnis");
                dn.SetAttribute("name", new DirectoryInfo(directory).Name);

                cn2.AppendChild(dn);

                foreach (var subDirectory in GetDirectories(directory)) {
                    var sdn = xdoc.CreateElement("verzeichnis");
                    sdn.SetAttribute("name", new DirectoryInfo(subDirectory).Name);

                    dn.AppendChild(sdn);
                }

                foreach (var file in GetFiles(directory)) {
                    var fn = xdoc.CreateElement("datei");
                    var fi = new FileInfo(file);

                    fn.SetAttribute("name", fi.Name);
                    fn.SetAttribute("groesse", GetFileSize(fi.Length));

                    dn.AppendChild(fn);
                }

                var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

                socket.Connect("localhost", 42420);

                socket.Send(Encoding.UTF8.GetBytes(xdoc.OuterXml));
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }


            var xdoc2 = new XmlDocument();

            var imported2 = xdoc2.ImportNode(cn, true);
            var cn3 = xdoc2.AppendChild(imported2);

            foreach (var file in GetFiles(rootDirectory)) {
                var fn = xdoc2.CreateElement("datei");
                var fi = new FileInfo(file);

                fn.SetAttribute("name", fi.Name);
                fn.SetAttribute("groesse", GetFileSize(fi.Length));

                cn3.AppendChild(fn);
            }

            var socket2 = new Socket(SocketType.Stream, ProtocolType.Tcp);

            socket2.Connect("localhost", 42420);

            socket2.Send(Encoding.UTF8.GetBytes(xdoc2.OuterXml));
            socket2.Shutdown(SocketShutdown.Both);
            socket2.Close();
        }

        private static IEnumerable<string> GetDirectories(string path) {
            var dl = new List<string>();

            try {
                if ((File.GetAttributes(path) & FileAttributes.ReparsePoint)
                    != FileAttributes.ReparsePoint) {
                    foreach (string folder in Directory.GetDirectories(path)) {
                        dl.Add(folder);
                    }
                }
            } catch (UnauthorizedAccessException) { }

            return dl.AsEnumerable();
        }

        private static IEnumerable<string> GetFiles(string path) {
            var fl = new List<string>();

            try {
                if ((File.GetAttributes(path) & FileAttributes.ReparsePoint)
                    != FileAttributes.ReparsePoint) {
                    foreach (string file in Directory.GetFiles(path)) {
                        fl.Add(file);
                    }
                }
            } catch (UnauthorizedAccessException) { }

            return fl.AsEnumerable();
        }

        private static string GetFileSize(long bytes) {
            var kb = bytes / 1000d;

            if (kb < 1000) {
                return $"{kb} kB";
            }

            var mb = kb / 1000;

            if (mb < 1000) {
                return $"{mb} MB";
            }

            var gb = mb / 1000;

            return $"{gb} GB";
        }
    }
}

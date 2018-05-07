using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NTClient
{
    internal class Program
    {
        private static void Main(string[] args) {
            var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

            socket.Connect("localhost", 42420);

            socket.Send(Encoding.UTF8.GetBytes(Enumerable.Repeat("spam", 1024).Aggregate("", (c, n) => c + " " + n) + "<EOF>"));

            Console.ReadLine();
        }
    }
}

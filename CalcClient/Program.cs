using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;

namespace CalcClient
{
    class Program
    {
        static Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static readonly string _logFile = "D:\\AOS\\ClientLog.txt";
        static int OurPort = 1034;


        static void Log(string data)
        {
            using (StreamWriter writer = File.AppendText(_logFile))
            {
                writer.WriteLine(DateTime.Now.ToShortTimeString() + " " + data);
            }
        }
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Client Started");
                sock.Connect("127.0.0.1", OurPort);
                Console.WriteLine("Client Connected\nWrite your request");
                string request = "";
                while (true)
                {
                    request = Console.ReadLine();
                    Log(request);
                    byte[] bytes = Encoding.ASCII.GetBytes(request);
                    sock.Send(bytes);
                    byte[] ans = new byte[256];
                    int bytesRec = sock.Receive(ans);
                    Console.WriteLine(DateTime.Now.ToShortTimeString() + " " +  Encoding.UTF8.GetString(ans, 0, bytesRec));
                    Log(Encoding.UTF8.GetString(ans, 0, bytesRec));
                    if (request == "stop") { break; }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Log(e.Message);
            }
            Console.ReadKey(true);
            sock.Close();
        }
    }
}

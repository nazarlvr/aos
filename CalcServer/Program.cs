using System;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Xml;


namespace CalcServer
{
    class Program   
    {
        static readonly string _logFile = "D:\\AOS\\ServerLog.txt";
        static int OurPort = 1034;
        static void Send(string message, Socket temp)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            temp.Send(data);
        }

        static void Log(string data)
        {
            using (StreamWriter writer = File.AppendText(_logFile))
            {
                writer.WriteLine(DateTime.Now.ToShortTimeString() + " " + data);
            }
        }

        public static string WhiteSpace(string s)
        {
            string temp = s.Replace(" ", "");
            return temp;
        }


        static void Main(string[] args)
        {
            try
            {
                Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Console.WriteLine("Server started");
                listenSocket.Bind(new IPEndPoint(IPAddress.Any, OurPort));
                listenSocket.Listen(1);
                Socket temperary = listenSocket.Accept();
                string s = "";
                int max = -50000;
                int min = 50000;
                int sum = 0;
                int kz = 0;
                int zag = 0;
                while (s != "stop")
                {
                    Console.WriteLine("New Connection");
                    byte[] bytes = new byte[256];
                    int len = temperary.Receive(bytes);
                    s = Encoding.ASCII.GetString(bytes, 0, len);
                    Console.WriteLine(DateTime.Now.ToShortTimeString() + " " + s);
                    Log(s);
                    zag++;
                    string answer = "";
                    if (s == "Who")
                    {
                        answer =  "Project creator: Lavrentyuk Nazar K-25, Variant - 09";
                        byte[] ans = new byte[256];
                        ans = Encoding.ASCII.GetBytes(answer);
                        temperary.Send(ans);
                        Console.WriteLine(DateTime.Now.ToShortTimeString() + " " + answer);
                    }
                    else
                    {
                        string s1 = WhiteSpace(s);
                        char zn = s1[0];
                        int z = s1.IndexOf(",");
                        if (z < 0)
                        {
                            answer =  "##ERROR## (wrong expression)";
                        }
                        else
                        {
                            int znak = 0, u = 1, a = 0, b = 0;
                            if (s1[1] == '-') { znak = 1; u++; }

                            for (int i = u; i < z; ++i)
                            {
                                a *= 10;
                                a += ((int)(s1[i] - '0'));
                            }
                            if (znak == 1) a = (-1) * a;
                            znak = 0; u = z + 1;
                            if (s1[u] == '-') { znak = 1; u++; }
                            for (int i = u; i < s1.Length; ++i)
                            {
                                b *= 10;
                                b += ((int)(s1[i] - '0'));
                            }
                            if (znak == 1) b = (-1) * b;
                            max = Math.Max(max, b); 
                            max = Math.Max(max, a);
                            min = Math.Min(min, a);
                            min = Math.Min(min, b);
                            sum += a + b;
                            kz++;
                            if (zn == '+')
                            {
                                int c = a + b;
                                answer = c.ToString();
                                max = Math.Max(max, c);
                                min = Math.Min(min, c);
                            }
                            else if (zn == '-')
                            {
                                int c = a - b;
                                answer = c.ToString();
                                max = Math.Max(max, c);
                                min = Math.Min(min, c);
                            }
                            else if (zn == '*')
                            {
                                int c = a * b;
                                answer = c.ToString();
                                max = Math.Max(max, c);
                                min = Math.Min(min, c);
                            }
                            else if (zn == '/')
                            {
                                int c = a / b;
                                answer = c.ToString();
                                max = Math.Max(max, c);
                                min = Math.Min(min, c);
                            }
                            else answer = "syntax error";
                            byte[] ans = new byte[256];
                            ans = Encoding.ASCII.GetBytes(answer);
                            temperary.Send(ans);
                            Console.WriteLine(DateTime.Now.ToShortTimeString() + " " + answer);
                            Log(answer);
                        }
                    }
                }
                int ser = sum / (2*kz);
                string answer1 = "Max: " + max.ToString() + "   " + "Min: " + min.ToString() + "   " + "Ser: " + ser.ToString() +"   " +
                    "Count Of Requests: " + zag.ToString() + "   " + "Count of Calculations: " + kz.ToString();
                byte[] ans1 = new byte[256];
                ans1 = Encoding.ASCII.GetBytes(answer1);
                Console.WriteLine(DateTime.Now.ToShortTimeString() + " " + answer1);
                Log(answer1);
                temperary.Send(ans1);
                temperary.Close();
                listenSocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Log(e.Message);
            }
            Console.ReadKey(true);
        }
    }
}

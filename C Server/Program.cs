using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Collections.Generic;
using System.Threading;


class Chat_Server
{
    public static void Main()
    {
        TcpListener server = null;
        Byte[] bytes = new Byte[256];
        String data = null;
        LinkedList<string> Server_list = new LinkedList<string>();
        bool Server_con = false;
        bool trigger = false;
        try
        {
            Int32 port = 9000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");



            server = new TcpListener(localAddr, port);
            Server_list.AddLast("Waiting for a connection... ");
            server.Start();

            Server_list.AddLast("'수' 님이 127.0.0.1에서 접속하셨습니다.");
            Server_con = true;

            foreach (var chat in Server_list)
            {
                Console.WriteLine(chat);
            }
            TcpClient client = server.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
            while (true)
            {
                if (Server_con == true)
                {
                    Thread sv_write = new Thread(() => Server_write(Server_list, data, stream, client, trigger, bytes));
                    sv_write.Start();


                    Thread sv_read = new Thread(() => Server_read(Server_list, data, stream, bytes, trigger));
                    sv_read.Start();
                }
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            server.Stop();
        }

        Console.WriteLine("\nHit enter to continue...");
        Console.Read();
    }
    public static void Server_write(LinkedList<string> sv_list, string data, NetworkStream stream, TcpClient client, bool trigger, Byte[] byts)
    {
        

            if (Console.ReadKey().Key == ConsoleKey.T)
            {

                Console.SetCursorPosition(0, 15);
                Console.WriteLine("메세지를 입력해주세요.");

                String input = Console.ReadLine();
                if (input == "/q")
                {
                    client.Close();
                    stream.Close();
                    Environment.Exit(0);
                }


                byts = new byte[256];
                data = input;
                byts = System.Text.Encoding.Default.GetBytes(data);
                stream.Write(byts, 0, byts.Length);
                if (sv_list.Count < 10)
                {
                    sv_list.AddLast($"[주]: {data}");
                    Console.Clear();
                    foreach (var chat in sv_list)
                    {
                        Console.WriteLine(chat);
                    }
                    trigger = true;
                }
            }
            /*else
            {
                Console.Clear();
            }*/          
    }

    public static void Server_read(LinkedList<string> sv_list, string data, NetworkStream stream, Byte[] byts, bool trigger)
    {
       
            byts = new byte[256];
            data = null;
            Int32 byt = stream.Read(byts, 0, byts.Length);
            data = System.Text.Encoding.Default.GetString(byts, 0, byt);
            if (sv_list.Count < 10)
            {
                sv_list.AddLast($"[수]: {data}");
                Console.Clear();
                foreach (var chat in sv_list)
                {
                    Console.WriteLine(chat);
                }
                trigger = false;
            }
    }

}
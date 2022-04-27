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
        bool Server_Connect = false;
        bool Trigger = false;
        String data = null;
        LinkedList<String> Server_list = new LinkedList<string>();

        try
        {

            Int32 port = 9000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            server = new TcpListener(localAddr, port);

            Server_list.AddLast("Waiting for a connection... ");

            server.Start();
            Server_list.AddLast("'수' 님이 127.0.0.1에서 접속하셨습니다.");
            Server_Connect = true;

            foreach (var chat in Server_list)
            {
                Console.WriteLine(chat);
            }

            TcpClient client = server.AcceptTcpClient();
            NetworkStream stream = client.GetStream();


            while (true)
            {

                if (Server_Connect == true)
                {
                    Thread sv_write = new Thread(() =>
                    Server_Write(Server_list, data, stream, client, Trigger, bytes));
                    sv_write.Start();

                    Thread sv_read = new Thread(() =>
                    Server_Read(Server_list, data, stream, bytes, Trigger));
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

    public static void Server_Write(LinkedList<string> Sv_list, String data,
        NetworkStream stream, TcpClient client, bool Trigger, Byte[] bytes)
    {
        while (true)
        {
            if (Console.ReadKey().Key == ConsoleKey.D1)
            {
                Console.SetCursorPosition(0, 15);
                Console.WriteLine("메세지를 입력해주세요.");

                String Input = Console.ReadLine();
                if (Input == "/q")
                {
                    stream.Close();
                    client.Close();
                    Environment.Exit(0);
                }

                bytes = new Byte[256];
                data = Input;
                bytes = System.Text.Encoding.Default.GetBytes(data);
                stream.Write(bytes, 0, bytes.Length);
                if (Sv_list.Count < 10)
                {
                    Sv_list.AddLast($"[주]: {data}");
                    Console.Clear();
                    foreach (var chat in Sv_list)
                    {
                        Console.WriteLine(chat);
                    }
                    Trigger = true;
                }
            }
            else
            {
                Console.Clear();
            }
        }
    }

    public static void Server_Read(LinkedList<string> Sv_list, String data,
        NetworkStream stream, Byte[] bytes, bool Trigger)
    {
        while (Trigger == true)
        {

            bytes = new Byte[256];
            data = null;
            Int32 byt = stream.Read(bytes, 0, bytes.Length);
            data = System.Text.Encoding.Default.GetString(bytes, 0, byt);
            if (Sv_list.Count < 10)
            {
                Sv_list.AddLast($"[수]: {data}");
                Console.Clear();
                foreach (var chat in Sv_list)
                {
                    Console.WriteLine(chat);
                }
                Trigger = false;
            }

        }
    }
}
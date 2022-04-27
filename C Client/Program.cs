using System.Net.Sockets;
using System.Text;
using System;
using System.Collections.Generic;
using System.Threading;


public class Chat_Client
{
    public static void Main()
    {
        Byte[] data = new Byte[256];
        TcpClient client;
        bool Client_con = false;
        LinkedList<string> Client_list = new LinkedList<string>();
        bool trigger = false;
        try
        {
            string server = "127.0.0.1";        
            Int32 port = 9000;
            while(Client_con == false)
            {
                String inputtcp = Console.ReadLine();            
                Console.Clear();
                
                if(inputtcp == $"/c {server}:{port}")
                {
                    Client_list.AddLast("127.0.0.1:9000에 접속시도중...");
                    Client_con = true;
                }else
                {
                    Console.WriteLine("서버를 다시 입력해주세요.");
                }

            }
            client = new TcpClient(server, port);  //?
            NetworkStream stream = client.GetStream();
            Client_list.AddLast("'주'님께 연결되었습니다.");

            foreach(var chat in Client_list)
            {
                Console.WriteLine(chat);
            }
            while (true)
            {
                if (Client_con == true)
                {
                    Thread cl_write = new Thread(() => Client_write(Client_list, data, stream, client, trigger));
                    cl_write.Start();

                    Thread cl_read = new Thread(() => Client_read(Client_list, data, stream, trigger));
                    cl_read.Start();
                }
            }                        
        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine("ArgumentNullException: {0}", e);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }

        Console.WriteLine("\n Press Enter to continue...");
        Console.Read();
    }
    public static void Client_write(LinkedList<string> cl_list,Byte[] data,NetworkStream stream,TcpClient client, bool trigger)
    {
       
            if (Console.ReadKey().Key == ConsoleKey.T)
            {
                Console.SetCursorPosition(0, 15);
                Console.WriteLine("메세지를 입력해주세요.");

                String message = Console.ReadLine();
                if (message == "/q")
                {
                    client.Close();
                    stream.Close();
                    Environment.Exit(0);
                }
                data = new byte[256];
                data = System.Text.Encoding.Default.GetBytes(message);
                stream.Write(data, 0, data.Length);
                if (cl_list.Count < 10)
                {
                    cl_list.AddLast($"[수] {message}");
                    Console.Clear();
                    foreach (var chat in cl_list)
                    {
                        Console.WriteLine(chat);
                    }
                    trigger = true;
                }
            }
           /* else
            {
                Console.Clear();
            }*/
                  
    }
    public static void Client_read(LinkedList<string> cl_list, Byte[] data, NetworkStream stream, bool trigger)
    {
       
            data = new byte[256];
            String responseData = String.Empty;
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.Default.GetString(data, 0, bytes);
            if (cl_list.Count < 10)
            {
                cl_list.AddLast($"[주]: {responseData}");
                Console.Clear();
                foreach (var chat in cl_list)
                {
                    Console.WriteLine(chat);
                }
                trigger = false;
            }          
    }
}
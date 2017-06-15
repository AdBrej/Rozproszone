using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Concurrent;


namespace gameServer
{
    class Program
    {
        static int port_main;
        // static int port_client = 51230;
        static Dictionary<int, byte> clients = new Dictionary<int, byte>();
        static UdpClient send_client;
        static IPEndPoint send_endpoint;
        static int num_connected_clients = 0;

        static List<IPEndPoint> ipClient = new List<IPEndPoint>();

        static BlockingCollection<byte[]> blocking_queue = new BlockingCollection<byte[]>(new ConcurrentQueue<byte[]>());

        static void Main(string[] args)
        {

            byte clientID = 0;
            //List<int> clients = new List<int>();
            //int port = 1111;



            Console.Write("Port: ");
            port_main = Convert.ToInt32(Console.ReadLine());
            //send_endpoint = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 5000);
            int port_to_send = free_port();
            send_endpoint = new IPEndPoint(IPAddress.Parse("192.168.0.255"), port_to_send);
            send_client = new UdpClient();
            // send_client.Client.Bind(send_endpoint);
            Action sendData = send_data;
            sendData.BeginInvoke(null, null);

            IPEndPoint localIPEndPoint = new IPEndPoint(IPAddress.Any, port_main);
            UdpClient myUdpClient = new UdpClient(localIPEndPoint);
            IPEndPoint remoteIPEndPoint;
            byte[] data;
            string msg;
            while (true)
            {
                remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
                // Console.WriteLine("\nCzekanie na klienta...");
                data = myUdpClient.Receive(ref remoteIPEndPoint);


                msg = Encoding.ASCII.GetString(data, 0, data.Length);

                //msg = clientID.ToString()+","+port;
                if (msg == "connect")
                {

                    int port = free_port();
                    int p2 = free_port();

                    Console.WriteLine("{0}:{1}> connected to server", remoteIPEndPoint.Address, port);

                    int[] msg_to_send = { (int)clientID, port, p2 };
                    // clients.Add(remoteIPEndPoint.Port);
                    clients.Add(port, clientID);
                    //  send_client = new UdpClient(new IPEndPoint(IPAddress.Any, p2));
                    var byteArray = new byte[msg_to_send.Length * 4];
                    Buffer.BlockCopy(msg_to_send, 0, byteArray, 0, byteArray.Length);

                    //data = Encoding.ASCII.GetBytes(msg);
                    myUdpClient.SendAsync(byteArray, byteArray.Length, remoteIPEndPoint);
                    //myUdpClient.Receive(ref remoteIPEndPoint); 
                    ipClient.Add(new IPEndPoint(remoteIPEndPoint.Address, p2));

                    Action<int> rec = receive_data;
                    rec.BeginInvoke(port, null, null);

                    byte[] start = { 1 };

                    //    send_client.SendAsync(start, start.Length, new IPEndPoint(remoteIPEndPoint.Address, p2));

                    num_connected_clients++;

                    if (num_connected_clients == 2)
                    {

                        foreach (IPEndPoint ic in ipClient)
                            send_client.SendAsync(start, start.Length, ic);
                        // send_client.SendAsync(start, start.Length, send_endpoint);
                    }

                    clientID++;
                }
                else
                {
                    byte id = byte.Parse(msg.Split(',')[1]);
                    int p = clients.FirstOrDefault(x => x.Value == id).Key;
                    Console.WriteLine("{0}:{1}> disconnected", remoteIPEndPoint.Address, p);
                    clients.Remove(p);
                }
            }
        }

        static void receive_data(int port)
        {
            UdpClient receive_client = new UdpClient(port);
            //Console.WriteLine("{0}> connected to server", (IPEndPoint)receive_client.Client.LocalEndPoint);
            byte[] data;
            IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {

                data = receive_client.Receive(ref remoteIPEndPoint);
                if (data.Length == 0)
                {
                    Console.WriteLine("{0}> Disconnected", (IPEndPoint)receive_client.Client.LocalEndPoint);
                    receive_client.Close();
                    break;
                    //} else if (data[data.Length - 1] == 2) {
                    //  Console.WriteLine("{0}> has stopped drawing", (IPEndPoint)receive_client.Client.LocalEndPoint);
                }
                else
                {
                    //  Console.WriteLine("{0}> started drawing", (IPEndPoint)receive_client.Client.LocalEndPoint);

                    //byte id = clients[port];
                    //byte[] data_with_id = new byte[data.Length + 1];
                    //Buffer.BlockCopy(data, 0, data_with_id, 0, data.Length);
                    //data_with_id[data_with_id.Length - 1] = id;
                    blocking_queue.Add(data);
                    //string msg = Encoding.ASCII.GetString(data_with_id, 0, data_with_id.Length);
                    // Console.WriteLine("{0}> {1}", ((IPEndPoint)receive_client.Client.LocalEndPoint), msg);
                }
            }
        }

        static void send_data()
        {
            // UdpClient send_client = new UdpClient();
            // send_endpoint = new IPEndPoint(IPAddress.Any, 5000);
            //send_client.Connect(send_endpoint);
            while (true)
            {

                byte[] data = blocking_queue.Take();
                // Console.WriteLine("Sent");
                //send_client.SendAsync(data, data.Length, send_endpoint);
                foreach (IPEndPoint ic in ipClient)
                    send_client.SendAsync(data, data.Length, ic);

            }

            //UdpClient client = new UdpClient();
            //IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, 15000);
            //  byte[] bytes = Encoding.ASCII.GetBytes("Foo");
            // client.Send(bytes, bytes.Length, ip);
            //client.Close();

        }

        static int free_port()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}

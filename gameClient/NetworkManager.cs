using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace gameClient {

    enum CODE {
        INIT,
        SEND_DATA,
        LOST
    }

    class NetworkManager {
        UdpClient client;
        UdpClient client_receive;
        UdpClient client_send;

        IPAddress ip_server;

        IPEndPoint ep_receive;
        IPEndPoint ep_send;
        Game game;
        int port_main;
        int port_client;
        int port_to_receive;
        byte id;

        public byte Id {
            get { return id; }
        }
        bool stop = true;

        public NetworkManager(Game game) {
            this.game = game;
        }

        public void init() {

            Console.WriteLine("IP: ");
            String ip = Console.ReadLine();
            Console.WriteLine("PORT: ");
            String port = Console.ReadLine();
            ip_server = IPAddress.Parse(ip);
            port_main = int.Parse(port);
            client = new UdpClient();
            //client.Connect(ip_server, port_main);

            client.Connect(ip_server, port_main); 
            string msg = "connect";
            byte[] data_connect = Encoding.ASCII.GetBytes(msg);
            client.Send(data_connect, data_connect.Length);

            IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] bytes = null;
            try {
                bytes = client.Receive(ref remoteIPEndPoint); 
                //  msg = Encoding.ASCII.GetString(data, 0, data.Length);

            } catch (SocketException e) {

                return;
            }

            int[] data = new int[3];
            Buffer.BlockCopy(bytes, 0, data, 0, bytes.Length);

            id = (byte)data[0];
            port_client = data[1];
            port_to_receive = data[2];
            stop = false;


            //receive
            client_receive = new UdpClient();
            ep_receive = new IPEndPoint(IPAddress.Any, port_to_receive);
            client_receive.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client_receive.Client.Bind(ep_receive); // przez to blokowanie przez firewall

            //byte[] dd = {0};


            byte[] start;

            start = client_receive.Receive(ref ep_receive); // czekanie na graczy 



            //send
            client_send = new UdpClient();
            ep_send = new IPEndPoint(ip_server, port_client);
            client_send.Connect(ep_send);

            
            Receive();
        }

        private void Receive() {
            client_receive.BeginReceive(new AsyncCallback(MyReceiveCallback), null);
        }

        private void MyReceiveCallback(IAsyncResult result) {

            byte[] bytes = client_receive.EndReceive(result, ref ep_receive);


            if (bytes[bytes.Length - 1] != id) {

                Action<byte[]> del = game.updateNonPlayers;
                del.BeginInvoke(bytes, null, null);
                
                //if (bytes[bytes.Length - 2] == 0) {
                    
                    //double[] data = new double[4];
                   // Buffer.BlockCopy(bytes, 0, data, 0, bytes.Length - 2);
                    

                    //System.Windows.Application.Current.Dispatcher.Invoke(delegate {
                    //    window.draw_to_canvas(data);
                    //});
                //} else if (bytes[bytes.Length - 2] == 0) {

                //    Action<byte[]> del = window.setOtherColor;
                //    del.BeginInvoke(bytes, null, null);

                //}
                //Action<double[]> d = window.draw_to_canvas;             
                //d.BeginInvoke(data, null, null);
            }
            
            if (!stop) {
                Receive(); 
            }
        }

        public void StopReceiving() {

            string msg = "disconnect," + id;
            byte[] data_disconnect = Encoding.ASCII.GetBytes(msg);
            client.Send(data_disconnect, data_disconnect.Length);

            byte[] bytes = new byte[0];

            client_send.SendAsync(bytes, bytes.Length);

            try {
                client_receive.Client.Disconnect(true);
                client_receive.Close();
                client_send.Client.Disconnect(true);
                client_send.Close();
                client.Client.Disconnect(true);
                client.Close();
            } catch (System.Net.Sockets.SocketException) {
            } finally {
                stop = true;
            }
        }

        public void Send() { 
            // UdpClient client = new UdpClient();
            //client.Connect(ip_server, port_client);
            //IPEndPoint ip = new IPEndPoint(ip_server, port_client); //255.255.255.255
            byte[] bytes = new byte[3];
            bytes[2] = id;
            while (!stop) {
                //string msg = "client num " + id;

                Thread.Sleep(200);
                bytes[0] = (byte)(game.Player.Gap ? 1 : 0);
                bytes[1] = (byte)game.Player.Key;
              //  Buffer.BlockCopy(data, 0, bytes, 0, bytes.Length - 1);
              //  bytes[bytes.Length - 1] = code;
                client_send.SendAsync(bytes, bytes.Length);
                //client.Close();
            }
        }
    
    }
}

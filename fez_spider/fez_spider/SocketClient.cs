using System.Net.Sockets;
using Microsoft.SPOT;
using System;
using System.Net;
using System.Text;

namespace fez_spider
{
    public class SocketClient
    {

        private Socket _socket;
        public Socket Socket {
                                get { return _socket; }
                                private set { }
         } 
        

        public SocketClient()
        {
            // Establish the remote endpoint for the socket.  
            // This example uses port 11000 on the local computer.  

            
            // Create a TCP/IP  socket.  
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        }


        public void Connect(String HOSTNAME, int PORT)
        {
            // Connect the socket to the remote endpoint. Catch any errors.  
            try {

                IPHostEntry ipHostInfo = Dns.GetHostEntry(HOSTNAME);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, PORT);

                _socket.Connect(remoteEP);

                Debug.Print("Socket connected to " + _socket.RemoteEndPoint.ToString());

                    
                // Receive the response from the remote device.  
                //int bytesRec = sender.Receive(bytes);
                //Console.WriteLine("Echoed test = {0}",
                //Encoding.ASCII.GetString(bytes, 0, bytesRec));

                // Release the socket.  
                //sender.Shutdown(SocketShutdown.Both);
                //sender.Close();

            }catch (Exception e)
            {
                Debug.Print("Unexpected exception : {0}" + e.ToString());
            }

            

        }
    }
}

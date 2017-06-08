using System.Net.Sockets;
using Microsoft.SPOT;
using System;
using System.Net;
using System.Text;

namespace fez_spider
{

    #region SocketClient definition
    public class SocketClient
    {

        private Socket _socket;
        public Socket Socket {
            get { return _socket; }
            private set { if (value != _socket) _socket = value; }
         } 
        

        /// <summary>
        /// Initialize Socket
        /// </summary>
        public SocketClient()
        {
            // Create a TCP/IP  socket.  
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }



        /// <summary>
        /// Connect to remote socket
        /// </summary>
        /// <param name="HOSTNAME"></param>
        /// <param name="PORT"></param>
        public void Connect(String HOSTNAME, int PORT)
        {
            // Connect the socket to the remote endpoint. Catch any errors.  
            try {

                IPHostEntry ipHostInfo = Dns.GetHostEntry(HOSTNAME);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, PORT);

                _socket.Connect(remoteEP);

                Debug.Print("Socket connected to " + _socket.RemoteEndPoint.ToString());
                
            }catch (Exception e)
            {
                Debug.Print("Unexpected exception : {0}" + e.ToString());
            }

            

        }


    }
    #endregion
}

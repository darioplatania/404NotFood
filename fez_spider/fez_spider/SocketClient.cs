using System.Net.Sockets;
using Microsoft.SPOT;
using System;
using System.Net;
using System.Text;

namespace fez_spider
{
    public class SocketClient
    {

        public static void StartClient()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];

            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.  

                IPHostEntry ipHostInfo = Dns.GetHostEntry("192.168.100.1");
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 4096);

                // Create a TCP/IP  socket.  
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try {
                                       
                    socket.Connect(remoteEP);

                    Debug.Print("Socket connected to " + socket.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.  
                    byte[] msg = Encoding.UTF8.GetBytes("NEW_ORDER");

                    // Send the data through the socket.                      
                    int bytesSent = socket.Send(msg);                   

                    // Receive the response from the remote device.  
                    //int bytesRec = sender.Receive(bytes);
                    //Console.WriteLine("Echoed test = {0}",
                    //Encoding.ASCII.GetString(bytes, 0, bytesRec));

                    // Release the socket.  
                    //sender.Shutdown(SocketShutdown.Both);
                    //sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Debug.Print("ArgumentNullException : {0}" + ane.ToString());
                }
                catch (SocketException se)
                {
                    Debug.Print("SocketException : {0}" + se.ToString());
                }
                catch (Exception e)
                {
                    Debug.Print("Unexpected exception : {0}" + e.ToString());
                }

            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
            }
        }
    }
}

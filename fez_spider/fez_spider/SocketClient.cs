using System.Net.Sockets;
using Microsoft.SPOT;
using System;
using System.Net;

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
                IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                // Create a TCP/IP  socket.  
                Socket sender = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    sender.Connect(remoteEP);

                    Debug.Print("Socket connected to {0}" + sender.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.  
                    // byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

                    // Send the data through the socket.  
                    //int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.  
                    //int bytesRec = sender.Receive(bytes);
                    //Console.WriteLine("Echoed test = {0}",
                        //Encoding.ASCII.GetString(bytes, 0, bytesRec));

                    // Release the socket.  
                    //sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

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

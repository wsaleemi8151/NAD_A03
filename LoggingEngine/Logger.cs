using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoggingEngine
{
    public static class Logger
    {

        /*
        * FUNCTION : Start
        *
        * DESCRIPTION : This function is used to start logger engine
        *
        * PARAMETERS : void
        *
        */
        public static void Start()
        {
            TcpListener loggingEngine = null;
            try
            {
                Int32 serverPort = Convert.ToInt32(ConfigurationManager.AppSettings["serverPort"]);
                IPAddress serverIP_Address = IPAddress.Parse(ConfigurationManager.AppSettings["serverIP_Address"]);

                loggingEngine = new TcpListener(serverIP_Address, serverPort);

                // Start listening for client requests.
                loggingEngine.Start();


                // Enter the listening loop.
                while (true)
                {
                    // Perform a blocking call to accept requests.
                    TcpClient client = loggingEngine.AcceptTcpClient();
                    ParameterizedThreadStart ts = new ParameterizedThreadStart(Worker);
                    Thread clientThread = new Thread(ts);
                    clientThread.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                loggingEngine.Stop();
            }


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }


        /*
        * FUNCTION : Worker
        *
        * DESCRIPTION : This function is used to process the client request
        *
        * PARAMETERS : Object o     -       TcpClient
        *
        */
        internal static void Worker(Object o)
        {
            TcpClient client = (TcpClient)o;
            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            string data = null;

            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            int i;

            // Loop to receive all the data sent by the client.
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                string response = "Success";

                FileOperations.WriteToFile(data);

                byte[] responseMessage = System.Text.Encoding.ASCII.GetBytes(response);
                stream.Write(responseMessage, 0, responseMessage.Length);
            }

            // Shutdown and end connection
            client.Close();
        }

    }
}

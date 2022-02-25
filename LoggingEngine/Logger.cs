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
            string clientMessage = null;

            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            int i;

            // Loop to receive all the data sent by the client.
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                clientMessage = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                string response = "Success";

                // parsing client message to get formatted log message
                string logMessage = ParseMessage(clientMessage);

                if (logMessage == "Invalid log format.")
                {
                    byte[] responseMessage = System.Text.Encoding.ASCII.GetBytes(logMessage);
                    stream.Write(responseMessage, 0, responseMessage.Length);
                }
                else
                {
                    FileOperations.WriteToFile(logMessage);

                    byte[] responseMessage = System.Text.Encoding.ASCII.GetBytes(response);
                    stream.Write(responseMessage, 0, responseMessage.Length);
                }
            }

            // Shutdown and end connection
            client.Close();
        }

        static string ParseMessage(string message)
        {
            int facilityCode = 0;
            int priorityCode = 0;

            string facilityCodeStr = "";
            string securityCodeStr = "";

            var strMessageArr = message.Split('|');

            if (strMessageArr.Length != 2 || int.TryParse(strMessageArr[0], out priorityCode) == false)
            {
                return "Invalid log format.";
            }

            string errorMessage = strMessageArr[1];

            // parsing Priority code to get facility code and facility string
            ParsePriorityCode(priorityCode, ref facilityCode, ref facilityCodeStr);

            //priorityCode = (facilityCode * 8) + securityCode;
            int securityCode = priorityCode - (facilityCode * 8);

            // parsing Security code to get Security level string
            ParseSecurityCode(securityCode, ref securityCodeStr);


            // {facilityCodestr} [priorityCode]: Scodestr msg
            // facilityCodestr[priorityCode]: Scodestr: msg
            string timeStamp = DateTime.Now.ToString("MMM dd HH:mm:ss");
            return $"{timeStamp} {facilityCodeStr}[{priorityCode}]: {securityCodeStr} {errorMessage}";

        }

        static void ParseSecurityCode(int securityCode, ref string securityCodeStr)
        {
            switch (securityCode)
            {
                case 0:
                    securityCodeStr = "emerg:";
                    break;
                case 1:
                    securityCodeStr = "alert:";
                    break;
                case 2:
                    securityCodeStr = "crit:";
                    break;
                case 3:
                    securityCodeStr = "err:";
                    break;
                case 4:
                    securityCodeStr = "warning:";
                    break;
                case 5:
                    securityCodeStr = "notice:";
                    break;
                case 6:
                    securityCodeStr = "<info> ";
                    break;
                case 7:
                    securityCodeStr = "debug:";
                    break;
                default:
                    securityCodeStr = "";
                    break;
            }
        }

        static void ParsePriorityCode(int priorityCode, ref int facilityCode, ref string facilityCodeStr)
        {
            if ((priorityCode >= 0) && (priorityCode <= 7))
            {
                facilityCode = 0;
                facilityCodeStr = "kern";
            }
            else if ((priorityCode >= 8) && (priorityCode <= 15))
            {
                facilityCode = 1;
                facilityCodeStr = "user";
            }
            else if ((priorityCode >= 16) && (priorityCode <= 23))
            {
                facilityCode = 2;
                facilityCodeStr = "mail";
            }
            else if ((priorityCode >= 24) && (priorityCode <= 31))
            {
                facilityCode = 3;
                facilityCodeStr = "daemon";
            }
            else if ((priorityCode >= 32) && (priorityCode <= 39))
            {
                facilityCode = 4;
                facilityCodeStr = "auth";
            }
            else if ((priorityCode >= 40) && (priorityCode <= 47))
            {
                facilityCode = 5;
                facilityCodeStr = "syslog";
            }
            else if ((priorityCode >= 48) && (priorityCode <= 55))
            {
                facilityCode = 6;
                facilityCodeStr = "Ipr";
            }
            else if ((priorityCode >= 56) && (priorityCode <= 63))
            {
                facilityCode = 7;
                facilityCodeStr = "news";
            }
            else if ((priorityCode >= 64) && (priorityCode <= 71))
            {
                facilityCode = 8;
                facilityCodeStr = "uucp";
            }
            else if ((priorityCode >= 72) && (priorityCode <= 79))
            {
                facilityCode = 9;
                facilityCodeStr = "cron";
            }
            else if ((priorityCode >= 80) && (priorityCode <= 87))
            {
                facilityCode = 10;
                facilityCodeStr = "authpriv";
            }
            else if ((priorityCode >= 88) && (priorityCode <= 95))
            {
                facilityCode = 11;
                facilityCodeStr = "ftp";
            }
            else if ((priorityCode >= 96) && (priorityCode <= 103))
            {
                facilityCode = 12;
                facilityCodeStr = "ntp";
            }
            else if ((priorityCode >= 104) && (priorityCode <= 111))
            {
                facilityCode = 13;
                facilityCodeStr = "security";
            }
            else if ((priorityCode >= 112) && (priorityCode <= 119))
            {
                facilityCode = 14;
                facilityCodeStr = "console";
            }
            else if ((priorityCode >= 120) && (priorityCode <= 127))
            {
                facilityCode = 15;
                facilityCodeStr = "solaris-cron";
            }
            else if ((priorityCode >= 128) && (priorityCode <= 135))
            {
                facilityCode = 16;
                facilityCodeStr = "local0";
            }
            else if ((priorityCode >= 136) && (priorityCode <= 143))
            {
                facilityCode = 17;
                facilityCodeStr = "local1";
            }
            else if ((priorityCode >= 144) && (priorityCode <= 151))
            {
                facilityCode = 18;
                facilityCodeStr = "local2";
            }
            else if ((priorityCode >= 152) && (priorityCode <= 159))
            {
                facilityCode = 19;
                facilityCodeStr = "local3";
            }
            else if ((priorityCode >= 160) && (priorityCode <= 167))
            {
                facilityCode = 20;
                facilityCodeStr = "local4";
            }
            else if ((priorityCode >= 168) && (priorityCode <= 175))
            {
                facilityCode = 21;
                facilityCodeStr = "local5";
            }
            else if ((priorityCode >= 176) && (priorityCode <= 183))
            {
                facilityCode = 22;
                facilityCodeStr = "local6";
            }
            else if ((priorityCode >= 184) && (priorityCode <= 191))
            {
                facilityCode = 23;
                facilityCodeStr = "local7";
            }
            else
            {
                facilityCode = 101;
                facilityCodeStr = "unknown";
            }
        }

    }
}

/*
* FILE : Program.cs
* PROJECT : PROG2121-Windows and Mobile Programming - Assignment #3
* PROGRAMMER : Waqar Ali Saleemi
* FIRST VERSION : 2012-10-22
* DESCRIPTION :
* The functions in this file are used to demostrate the usage of Threading
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoggingEngine
{
    public static class FileOperations
    {

        /*
        * FUNCTION : WriteToFile
        *
        * DESCRIPTION : This function is used to write string blocks to the file.
        *
        * PARAMETERS : 
        * object fileParams : an object of type struct LoggingParams which holds file path, file size and etc
        */
        public static void WriteToFile(string stringToWrite)
        {

            /*
            * TITLE : How to lock a text file for reading and writing using C#
            * AUTHOR : Dimitrios Kalemis
            * DATE : June 23, 2013 
            * AVAILABIILTY : https://dkalemis.wordpress.com/2013/06/23/how-to-lock-a-text-file-for-reading-and-writing-using-csharp/
            */

            FileStream myFileStream = null;

            try
            {
                // continue loop until file is not in use anymore
                while (true)
                {
                    try
                    {
                        // FileShare.Read will lock the file for writing while in use by this process
                        myFileStream = new FileStream(Environment.CurrentDirectory + "\\logFile.txt", FileMode.Append, FileAccess.Write, FileShare.None);
                        break;
                    }
                    catch (FileNotFoundException ex)
                    {
                        throw ex;
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(20);
                    }
                }

                StreamWriter myStreamWriter = new StreamWriter(myFileStream);

                myStreamWriter.WriteLine(stringToWrite);

                myStreamWriter.Close();
                myFileStream.Close();
                myFileStream.Dispose();

                // sleep the current thread for delay provided in params
                //Thread.Sleep(logParams.delay);
            }
            catch (ThreadAbortException)
            {
                // if thread is aborted
                if (myFileStream != null)
                {
                    myFileStream.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (myFileStream != null)
                {
                    myFileStream.Dispose();
                }
            }
        }
    }
}

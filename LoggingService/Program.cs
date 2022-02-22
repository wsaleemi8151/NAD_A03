/*
* FILE : Program.cs
*
* PROJECT : SENG2040-22W-Sec1-Network Application Development - Assignment # 3
* PROGRAMMER : Gursharan Singh - Waqar Ali Saleemi
* FIRST VERSION : 2022-02-21
* DESCRIPTION :
* Function in this file are used to start Logging service 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace LoggingService
{
    static class Program
    {
        /// <summary>
        /// 
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new LoggingService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}

/*
* FILE : Program.cs
* PROJECT : PROG2121-Windows and Mobile Programming - Assignment #3
* PROGRAMMER : Waqar Ali Saleemi
* FIRST VERSION : 2012-10-22
* DESCRIPTION :
* The functions in this file are used to demostrate the usage logging server via console app 
*/

using LoggingEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Start();
        }
    }
}

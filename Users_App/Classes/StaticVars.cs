using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using Microsoft.VisualBasic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace Users_App.Classes
{
    public static class StaticVars
    {
        //public static int ;
        public static string _mainSource;
        //public static bool ;
        public static List<Process> _processesList = null;
        public static List<CompletedProcessesClass> _completedProcessesList = null;
        public static List<OpenProcessesClass> _openProcessesList = null;

        public class CompletedProcessesClass
        {
            public string _processName = "";
            public int _processTime = 0;
            //public bool _processBaned = false;
        }

        public class OpenProcessesClass
        {
            public string _processName = "";
            public DateTime _processStartedTime;
            //public bool _processBaned = false;
        }
    }
}

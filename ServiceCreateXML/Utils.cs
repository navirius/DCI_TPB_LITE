using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Library.Utils;

namespace ServiceCreateXML
{
    internal class Utils
    {
        const string LogName = "ServiceCreateXML_TPB";
        public static NbLogger _logger = new NbLogger(AppDomain.CurrentDomain.BaseDirectory + @"\Log\", LogName);

        public static void Logger(string msg)
        {
            _logger.Log(msg);

            Console.WriteLine(msg);
        }
    }
}

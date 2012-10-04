using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public static class Log
    {
        public static void WriteLine(string text)
        {
            Console.WriteLine("{0} ; {1}", DateTime.Now.ToString("hh:mm:ss"), text);
        }
    }
}

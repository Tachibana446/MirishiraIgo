using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MiriShiraIgo1
{
    class MiriShiraIgo1
    {
        static void Main(String[] args)
        {
            // DEBUG
            Debug.Listeners.Add(new TextWriterTraceListener("debug-file.txt"));
            Debug.WriteLine(DateTime.Now.ToString());

            new Game().Main();
        }
    }
}

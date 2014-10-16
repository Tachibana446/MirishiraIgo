using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxLibDLL;

namespace MiriShiraIgo1
{
    interface Playable
    {
        Tuple<int, int> Select();
        String GetName();
    }
}

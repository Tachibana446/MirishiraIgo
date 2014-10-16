using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiriShiraIgo1
{
    class AI_Rand : Playable
    {
        public String GetName(){
            return "Random AI";
        }

        public Tuple<int, int> Select()
        {
            // こいつの頭は完全にランダムです
            var blain = new Random();
            return new Tuple<int, int>(blain.Next(16), blain.Next(16));
        }
    }
}

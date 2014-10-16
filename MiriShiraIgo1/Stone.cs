using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiriShiraIgo1
{
    /// <summary>
    /// 石のクラス
    /// 座標と手番がある
    /// </summary>
    class Stone
    {
        public int x;
        public int y;
        public int turn;

        public Stone(int x, int y, int turn)
        {
            this.x = x;
            this.y = y;
            this.turn = turn;
        }

        public override String ToString()
        {
            return "(" + x.ToString() + "," + y.ToString() + ":" + turn.ToString() + ")";
        }

    }
}

﻿using System;
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

        public bool Equals(Stone other)
        {
            return (x== other.x && y == other.y && turn == other.turn);
        }

        // 石が端っこかどうか調べる
        public bool IsTopEnd()
        {
            return y <= 0;
        }

        public bool IsBottomEnd()
        {
            return y >= GameConstants.BoardAxis;
        }

        public bool IsLeftEnd()
        {
            return x <= 0;
        }

        public bool IsRightEnd()
        {
            return x >= GameConstants.BoardAxis;
        }


    }
}

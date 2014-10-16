using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxLibDLL;

namespace MiriShiraIgo1
{
    class Player : Playable
    {
        private String name;

        public Player(String name)
        {
            this.name = name;
        }

        public Tuple<int,int> Select(){
            int x, y;
            DX.GetMousePoint(out x, out y);
            // クリックされた位置から本来置きたかった点を推測
            int tx = (x + Game.cellSize / 2) / Game.cellSize;
            int ty = (y + Game.cellSize / 2) / Game.cellSize;

            return new Tuple<int, int>(tx, ty);
        }

        public String GetName()
        {
            return name;
        }
    }
}

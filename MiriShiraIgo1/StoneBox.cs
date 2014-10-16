using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiriShiraIgo1
{
    class StoneBox
    {
        private List<Stone> stones;

        public StoneBox()
        {
            stones = new List<Stone>();
        }

        public StoneBox(List<Stone> list)
        {
            stones = list;
        }

        public List<Stone> getStones()
        {
            return stones;
        }

        public void Add(Stone stone)
        {
            stones.Add(stone);
        }

        /// <summary>
        /// 引数に含まれる石と自分が持つ石の差集合を返す
        /// </summary>
        /// <param name="stonebox"></param>
        /// <returns></returns>
        public List<Stone> Except(StoneBox stonebox)
        {
            var result = new List<Stone>();
            result = stones.Except<Stone>(stonebox.getStones()).ToList<Stone>();
            return result;

        }

        /// <summary>
        /// 引数の石が含まれているかどうか
        /// </summary>
        /// <param name="stone"></param>
        /// <returns></returns>
        public bool hasStone(Stone stone)
        {
            if (stones.IndexOf(stone) == -1)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 座標にある石のインデックスを返す
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int getIndexFromCoordinate(int x, int y)
        {
            foreach (var stone in stones)
            {
                if (stone.x == x && stone.y == y)
                {
                    return stones.IndexOf(stone);
                }
            }
            return -1;
        }
        /// <summary>
        /// 座標位置にある石を返す。なければnullを返す
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Stone stone またはnull</returns>
        public Stone getStoneFromCoordinate(int x, int y)
        {
            foreach (var stone in stones)
            {
                if (stone.x == x && stone.y == y)
                {
                    return stone;
                }
            }
            return null;
        }

        public String ToString()
        {
            String str = "";
            foreach (var stone in stones)
            {
                str += stone.ToString() + "\n";
            }
            return str;
        }
    }
}

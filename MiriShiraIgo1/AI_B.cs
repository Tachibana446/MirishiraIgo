using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiriShiraIgo1
{
    class AI_B : Playable
    {
        public String GetName()
        {
            return "BBB";
        }

        public bool IsGiveUp()
        {
            return false;
        }

        public Tuple<int, int> Select(StoneBox placedStones, StoneBox deadStones)
        {
            return null;
        }

        /// <summary>
        /// 評価関数
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int GetValue(int x, int y, StoneBox myStones, StoneBox oppStones, StoneBox alives)
        {
            int oppDeath = 0;

            foreach (var stone in oppStones.getStones())
            {
                if (Game.StoneAlive(stone, alives))
                {
                    oppDeath += 1;
                }
            }

            return oppDeath;
        }
    }
}

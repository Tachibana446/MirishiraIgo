using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiriShiraIgo1
{
    class AI_A : Playable
    {
        private List<DataSet> datasets;

        public AI_A()
        {
            datasets = new List<DataSet>();
        }

        public String GetName()
        {
            return "Alice";
        }

        public Tuple<int, int> Select()
        {
            Random rand = new Random();
            return new Tuple<int, int>(rand.Next(GameConstants.BoardAxis), rand.Next(GameConstants.BoardAxis));
        }

        /// <summary>
        /// 学習したデータのセット
        /// </summary>
        private class DataSet
        {
            /// <summary>
            /// 現在の相手の石
            /// </summary>
            private List<Data> opponentData;
            /// <summary>
            /// 現在の自分の石
            /// </summary>
            private List<Data> myData;
            /// <summary>
            /// それに対して置いた石とその手の評価値の組のリスト
            /// </summary>
            private List<Evaluation> evaluations;

            public DataSet(List<Data> opponentData, List<Data> myData, List<Evaluation> evaluations)
            {
                this.opponentData = opponentData;
                this.myData = myData;
                this.evaluations = evaluations;
            }

            public DataSet(List<Data> opponentData, List<Data> myData)
            {
                this.opponentData = opponentData;
                this.myData = myData;
                evaluations = new List<Evaluation>();
            }




        }

        /// <summary>
        /// 手とその手に対する評価値のセット
        /// </summary>
        private class Evaluation
        {
            public Data data { get; set; }
            public int evaluation { get; set; }

            public Evaluation(Data data, int evaluation)
            {
                this.data = data;
                this.evaluation = evaluation;
            }

        }

        /// <summary>
        /// 石の座標
        /// </summary>
        private class Data
        {
            public int x { get; set; }
            public int y { get; set; }

            public Data(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
    }
}

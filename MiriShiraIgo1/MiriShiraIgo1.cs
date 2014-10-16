using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxLibDLL;

namespace MiriShiraIgo1
{
    class MiriShiraIgo1
    {
        static int boardSize = 480;
        static int cellSize = boardSize / 16;
        // クリックの履歴
        static bool click = false;
        // 石をおいた場所の記録
        static List<Tuple<int, int>> stones = new List<Tuple<int, int>>();
        // 取られた石たち
        static List<Tuple<int, int>> death = new List<Tuple<int, int>>();

        static void Main(String[] args)
        {
            // ウィンドウモードで起動
            DX.ChangeWindowMode(DX.TRUE);
            // 裏画面
            DX.SetDrawScreen(DX.DX_SCREEN_BACK);

            if (DX.DxLib_Init() == -1)
            {
                throw new Exception("DxLibの初期化に失敗");
            }

            // 盤面の表示
            DrawBoard();

            // メインループ
            while (DX.CheckHitKey(DX.KEY_INPUT_ESCAPE) != 1)
            {
                // メッセージループ
                if (DX.ProcessMessage() == -1)
                {
                    break;
                }

                // 主処理

                // 石を置く
                bool putOn = PutStones();

                // 石が取れるかどうか適当に考える
                if (putOn)
                {
                    Judge(1);
                }

                // 石の描画
                DrawStones();


                // 裏画面の反転
                DX.ScreenFlip();
            }

        }

        // 盤面の表示
        static void DrawBoard()
        {
            DX.DrawBox(0, 0, boardSize + 1, boardSize + 1, DX.GetColor(255, 204, 51), DX.TRUE);
            for (int i = 0; i < 16; i++)
            {
                // 縦線の描画
                DX.DrawLine(cellSize * i, 0, cellSize * i, boardSize, DX.GetColor(0, 0, 0));
                // 横線の描画
                DX.DrawLine(0, cellSize * i, boardSize, cellSize * i, DX.GetColor(0, 0, 0));
            }
            // 交差するとこの点の描画
            for (int i = 0; i <= 16; i++)
            {
                for (int j = 0; j <= 16; j++)
                {
                    DX.DrawCircle(cellSize * i, cellSize * j, 2, DX.GetColor(0, 0, 0), DX.TRUE);
                }
            }
        }

        // 石を置く処理
        static bool PutStones()
        {
            if ((DX.GetMouseInput() & DX.MOUSE_INPUT_LEFT) != 0 && click == false)
            {
                click = true;
                int x, y;
                DX.GetMousePoint(out x, out y);
                // そいつがクリックしたかった本来の場所を推測
                int tx = (x + cellSize / 2) / cellSize;
                int ty = (y + cellSize / 2) / cellSize;
                // そこに石を置いたことにする
                stones.Add(new Tuple<int, int>(tx, ty));

                // DEBUG
                foreach (var item in stones)
                {
                    System.Diagnostics.Debug.WriteLine(item.Item1.ToString() + "," + item.Item2.ToString());
                }
                return true;

            }
            if ((DX.GetMouseInput() & DX.MOUSE_INPUT_LEFT) == 0 && click)
            {
                System.Diagnostics.Debug.WriteLine("------");
                click = false;
                return false;
            }
            return false;

        }

        // 置かれた石の描画
        static void DrawStones()
        {
            bool turn = true;
            foreach (var coordinate in stones)
            {

                // 死んでいたら描画しない
                if (death.IndexOf(coordinate) == -1)
                {
                    // あとは先攻後攻で色を変えて描画
                    if (turn)
                    {
                        DX.DrawCircle(coordinate.Item1 * cellSize, coordinate.Item2 * cellSize, 5, DX.GetColor(0, 0, 0), DX.TRUE);
                    }
                    else
                    {
                        DX.DrawCircle(coordinate.Item1 * cellSize, coordinate.Item2 * cellSize, 5, DX.GetColor(255, 255, 255), DX.TRUE);
                    }
                }
                turn = !turn;
            }
        }

        /// <summary>
        /// 石が生きてる判定する。端の石については内部で判定するので引数のフラグは変えなくて良い
        /// </summary>
        /// <param name="stone">判定する石</param>
        /// <param name="up">上に石があるか</param>
        /// <param name="down">下に石があるか</param>
        /// <param name="left">左に石があるか</param>
        /// <param name="right">右に石があるか</param>
        /// <returns></returns>
        static bool StoneAlive(Tuple<int, int> stone, bool up = true, bool down = true, bool left = true, bool right = true)
        {
            int x = stone.Item1;
            int y = stone.Item2;

            up &= !(x < 0);
            down &= !(x > 16);
            left &= !(y < 0);
            right &= !(y > 16);
            // 周囲の調査
            if (up)
            {
                Tuple<int, int> upStone = new Tuple<int, int>(x, y - 1);
                int index = stones.IndexOf(upStone);
                // 上に石がなければ生きてる
                if (index == -1)
                {
                    return true;
                }
                else
                {
                    // 隣の石が同じ色ならそいつも調査
                    if (index % 2 == stones.IndexOf(stone) % 2)
                    {
                        if (StoneAlive(upStone, true, false, true, true))
                        {
                            return true;
                        }
                    }
                }
            }
            if (down)
            {
                Tuple<int, int> downStone = new Tuple<int, int>(x, y + 1);
                int index = stones.IndexOf(downStone);
                if (index != -1)
                {
                    if (index % 2 == stones.IndexOf(stone) % 2)
                    {
                        if (StoneAlive(downStone, false, true, true, true))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
            if (left)
            {
                Tuple<int, int> leftStone = new Tuple<int, int>(x - 1, y);
                int index = stones.IndexOf(leftStone);
                if (index != -1)
                {
                    if (index % 2 == stones.IndexOf(stone) % 2)
                    {
                        if (StoneAlive(leftStone, true, true, true, false))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
            if (right)
            {
                Tuple<int, int> rightStone = new Tuple<int, int>(x + 1, y);
                int index = stones.IndexOf(rightStone);
                if (index != -1)
                {
                    if (index % 2 == stones.IndexOf(stone) % 2)
                    {
                        if (StoneAlive(rightStone, true, true, false, true))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 囲まれた石を取る
        /// </summary>
        /// <param name="turn">偶数か奇数か</param>
        static void Judge(int turn)
        {
            foreach (var stone in stones.Except(death))
            {
                if (!StoneAlive(stone))
                {
                    // 死亡者リストに登録
                    death.Add(stone);
                    // 盤面をクリーンにする
                    DrawBoard();
                    System.Diagnostics.Debug.WriteLine("DEAD!" + stone.Item1.ToString() + "," + stone.Item2.ToString());
                }
            }
        }
    }
}

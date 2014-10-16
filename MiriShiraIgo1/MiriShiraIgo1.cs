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
        static List<Stone> stones = new List<Stone>();
        // 取られた石たち
        static List<Stone> death = new List<Stone>();
        // ターン数のカウント
        static int turnCount = 0;

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
                // ターン数を進める
                turnCount += 1;

                click = true;
                int x, y;
                DX.GetMousePoint(out x, out y);
                // そいつがクリックしたかった本来の場所を推測
                int tx = (x + cellSize / 2) / cellSize;
                int ty = (y + cellSize / 2) / cellSize;
                // そこに石を置いたことにする
                stones.Add(new Stone(tx, ty, turnCount % 2));

                //DEBUG
                System.Diagnostics.Debug.WriteLine(turnCount.ToString()+"%2="+(turnCount % 2).ToString());

                // DEBUG
                foreach (var stone in stones)
                {
                    System.Diagnostics.Debug.WriteLine(stone.x.ToString() + "," + stone.y.ToString() + ":" + stone.turn.ToString());
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
            foreach (var stone in stones)
            {

                // 死んでいたら描画しない
                if (death.IndexOf(stone) == -1)
                {
                    // あとは先攻後攻で色を変えて描画
                    if (stone.turn % 2 == 0)
                    {
                        DX.DrawCircle(stone.x * cellSize, stone.y * cellSize, 5, DX.GetColor(0, 0, 0), DX.TRUE);
                    }
                    else
                    {
                        DX.DrawCircle(stone.x * cellSize, stone.y * cellSize, 5, DX.GetColor(255, 255, 255), DX.TRUE);
                    }
                }
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
        static bool StoneAlive(Stone stone, bool up = true, bool down = true, bool left = true, bool right = true)
        {
            int x = stone.x;
            int y = stone.y;

            up &= !(x < 0);
            down &= !(x > 16);
            left &= !(y < 0);
            right &= !(y > 16);
            // 周囲の調査
            if (up)
            {
                Stone upStone = GetStoneFromCoordinate(x, y - 1);
                // 上に石がなければ生きてる
                if (upStone == null)
                {
                    return true;
                }
                else
                {
                    // 隣の石が同じ色ならそいつも調査
                    if (upStone.turn == stone.turn)
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
                Stone downStone = GetStoneFromCoordinate(x, y + 1);
                if (downStone != null)
                {
                    if (downStone.turn == stone.turn)
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
                Stone leftStone = GetStoneFromCoordinate(x - 1, y);
                if (leftStone != null)
                {
                    if (leftStone.turn == stone.turn)
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
                Stone rightStone = GetStoneFromCoordinate(x + 1, y);
                if (rightStone != null)
                {
                    if (rightStone.turn == stone.turn)
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
        /// どちらのターンによるかで競合した時の優先度が決まる
        /// ただしその部分はまだない
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
                    System.Diagnostics.Debug.WriteLine("DEAD!" + stone.x.ToString() + "," + stone.y.ToString());
                }
            }
        }

        /// <summary>
        /// 座標にある石のインデックスを返す
        /// 後に別なクラスに分けること
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        static int GetIndexFromCoordinate(int x, int y)
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
        static Stone GetStoneFromCoordinate(int x, int y)
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
    }
}

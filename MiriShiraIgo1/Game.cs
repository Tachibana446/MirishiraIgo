using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxLibDLL;
using System.Diagnostics;

namespace MiriShiraIgo1
{
    class Game
    {
        static int boardSize = 480;
        static int cellSize = boardSize / 16;
        // クリックの履歴
        static bool click = false;
        // 置かれた石たち
        StoneBox placedStones = new StoneBox();
        // 取られた石たち
        StoneBox deadStones = new StoneBox();
        // ターン数のカウント
        private int turnCount;

        public Game()
        {
            turnCount = 0;
        }

        public void Main()
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

                // クリックされたら
                if (IsLeftClick())
                {
                    // ターンを進め
                    turnCount += 1;
                    // 石を置く
                    int x = GetClickCoordinate().Item1;
                    int y = GetClickCoordinate().Item2;
                    PutStone(x, y);
                    // DEBUG
                    Debug.WriteLine("---placed---");
                    Debug.WriteLine(placedStones.ToString());
                    // 石が取れるかの判定
                    Judge(0);
                    // DEBUG
                    Debug.WriteLine("---dead---");
                    Debug.WriteLine(deadStones.ToString());
                }

                // 石の描画
                DrawStones();

                // 裏画面の反転
                DX.ScreenFlip();
            }

        }

        // 盤面の表示
        private void DrawBoard()
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
        /// <summary>
        /// 今クリックしたかどうかを判定する
        /// 押しっぱなしの時はfalse
        /// </summary>
        /// <returns></returns>
        private bool IsLeftClick()
        {
            // 左クリックの入力があり、前回の入力がなければクリックと判断
            if ((DX.GetMouseInput() & DX.MOUSE_INPUT_LEFT) != 0 && click == false)
            {
                click = true;
                return true;
            }
            // 左クリックの入力がなく、前回の入力があればクリックを終えたと判断
            if ((DX.GetMouseInput() & DX.MOUSE_INPUT_LEFT) == 0 && click == true)
            {
                click = false;
            }
            return false;
        }
        /// <summary>
        /// クリックした地点の座標を返す
        /// </summary>
        /// <returns>Tuple(x,y)</returns>
        private Tuple<int, int> GetClickCoordinate()
        {
            int x, y;
            DX.GetMousePoint(out x, out y);
            // クリックされた位置から本来置きたかった点を推測
            int tx = (x + cellSize / 2) / cellSize;
            int ty = (y + cellSize / 2) / cellSize;

            return new Tuple<int, int>(tx, ty);
        }
        /// <summary>
        /// 指定の座標に石を置く
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void PutStone(int x, int y)
        {
            placedStones.Add(new Stone(x, y, turnCount % 2));
        }

        // 置かれた石の描画
        private void DrawStones()
        {
            foreach (var stone in placedStones.getStones())
            {

                // 死んでいなければ描画
                if (!deadStones.hasStone(stone))
                {
                    // あとは先攻後攻で色を変えて描画
                    if (stone.turn == 1)
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
        private bool StoneAlive(Stone stone, bool up = true, bool down = true, bool left = true, bool right = true)
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
                Stone upStone = placedStones.getStoneFromCoordinate(x, y - 1);
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
                Stone downStone = placedStones.getStoneFromCoordinate(x, y + 1);
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
                Stone leftStone = placedStones.getStoneFromCoordinate(x - 1, y);
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
                Stone rightStone = placedStones.getStoneFromCoordinate(x + 1, y);
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
        private void Judge(int turn)
        {
            foreach (var stone in placedStones.getStones().Except(deadStones.getStones()))
            {
                if (!StoneAlive(stone))
                {
                    // 死亡者リストに登録
                    deadStones.Add(stone);
                    // 盤面をクリーンにする
                    DrawBoard();
                    System.Diagnostics.Debug.WriteLine("DEAD!" + stone.x.ToString() + "," + stone.y.ToString());
                }
            }
        }


    }
}

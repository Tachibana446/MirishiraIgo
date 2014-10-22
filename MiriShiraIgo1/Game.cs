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
        public const int boardSize = GameConstants.BoardSize;
        public const int cellSize = boardSize / GameConstants.BoardAxis;

        private List<Playable> players = new List<Playable>();
        // クリックの履歴
        static bool click = false;
        // 置かれた石たち
        StoneBox placedStones = new StoneBox();
        // 取られた石たち
        StoneBox deadStones = new StoneBox();
        // 各ターンに死んだ石
        List<List<Stone>> deadLog = new List<List<Stone>>();
        // ターン数のカウント
        private int turnCount;

        // コンストラクタ中でプレイヤーを登録する
        public Game()
        {
            turnCount = 0;
            players.Add(new Player("ore"));
            players.Add(new Player("omae"));
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
                    // 石が置ければ
                    var xy = players[turnCount % 2].Select();
                    if (CanPutStone(xy))
                    {
                        // ターンを進め石を置く
                        turnCount += 1;
                        PutStone(xy);
                        //DEBUG
                        Debug.WriteLine("===== " + turnCount.ToString() + " =====");
                        // DEBUG
                        Debug.WriteLine("---placed---");
                        Debug.WriteLine(placedStones.ToString());
                        // 石が取れるかの判定
                        Judge(turnCount % 2);
                        // DEBUG
                        Debug.WriteLine("---dead---");
                        Debug.WriteLine(deadStones.ToString());
                    }
                    else
                    {
                        Debug.WriteLine("<置けないよ>");
                    }
                }

                // 石の描画
                DrawStones();
                // カーソル位置の描画
                DrawCursorCoordinate();

                // 裏画面の反転
                DX.ScreenFlip();
            }

        }

        // 盤面の表示
        private void DrawBoard()
        {
            DX.DrawBox(0, 0, boardSize + 1, boardSize + 1, DX.GetColor(255, 204, 51), DX.TRUE);
            for (int i = 0; i < GameConstants.BoardAxis; i++)
            {
                // 縦線の描画
                DX.DrawLine(cellSize * i, 0, cellSize * i, boardSize, DX.GetColor(0, 0, 0));
                // 横線の描画
                DX.DrawLine(0, cellSize * i, boardSize, cellSize * i, DX.GetColor(0, 0, 0));
            }
            // 交差するとこの点の描画
            for (int i = 0; i <= GameConstants.BoardAxis; i++)
            {
                for (int j = 0; j <= GameConstants.BoardAxis; j++)
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
        /// 引数の座標に石が置けるかどうか判定する
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool CanPutStone(int x, int y)
        {
            var alives = new StoneBox(placedStones.Except(deadStones));
            if (alives.getStoneFromCoordinate(x, y) != null)
            {
                return false;
            }
            // おいてみて自殺手ならおけない
            var nextPlan = new Stone(x, y, (turnCount + 1) % 2);
            alives.Add(nextPlan);
            if (!CanLiving(nextPlan, alives))
            {
                // 自殺手でも相手の石をとれるときは置ける
                // 自分の石
                var myStones = alives.ToLookUpByTurn()[(turnCount + 1) % 2].ToList();
                // 相手の石
                var opponentStones = alives.Except(myStones);
                foreach (var stone in opponentStones)
                {
                    if (!CanLiving(stone, alives))
                    {
                        // 劫でなければ置ける
                        return !IsKou(nextPlan, new StoneBox(placedStones.Except(deadStones)), turnCount);
                    }
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// そこに置くと劫のルールに違反するか調べる
        /// 一つ前のターンで死んだ自分の石の場所に置こうとした時、
        /// 直前のターンで相手が置いた石を取ることが出来るときは劫とみなす
        /// </summary>
        /// <param name="plan">置きたい手</param>
        /// <param name="alives">現状の盤面</param>
        /// <param name="nowTurn">ターン数</param>
        /// <returns></returns>
        private bool IsKou(Stone plan, StoneBox alives, int nowTurn)
        {
            // 一つ前のターンで死んだ自分の石か
            bool isPrevStone = false;
            foreach (var deadStone in deadLog[nowTurn - 1])
            {
                if (deadStone.turn == plan.turn)
                {
                    isPrevStone = true;
                }
            }
            if (isPrevStone)
            {
                StoneBox nextAlives = alives;
                nextAlives.Add(plan);
                // そこに置くと直前の相手の石が取れるのか
                var lastOppStone = placedStones.getStones().Last();
                if (!CanLiving(lastOppStone, nextAlives))
                {
                    return true;
                }
            }

            return false;

        }

        /// <summary>
        /// 引数の座標に石が置けるか判定する
        /// </summary>
        /// <param name="xy">Tuple(x,y)</param>
        /// <returns></returns>
        private bool CanPutStone(Tuple<int, int> xy)
        {
            return CanPutStone(xy.Item1, xy.Item2);
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
        /// <summary>
        /// 指定の座標に石を置く
        /// </summary>
        /// <param name="xy">Tuple(x,y)</param>
        private void PutStone(Tuple<int, int> xy)
        {
            PutStone(xy.Item1, xy.Item2);
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
                        DX.DrawCircle(stone.x * cellSize, stone.y * cellSize, 10, DX.GetColor(0, 0, 0), DX.TRUE);
                    }
                    else
                    {
                        DX.DrawCircle(stone.x * cellSize, stone.y * cellSize, 10, DX.GetColor(255, 255, 255), DX.TRUE);
                    }
                }
            }
        }

        /// <summary>
        /// 今マウスカーソルがどの座標を指しているのか描画する
        /// </summary>
        private void DrawCursorCoordinate()
        {
            DX.DrawBox(GameConstants.BoardSize + 10, 10, GameConstants.BoardSize + 100, 30, DX.GetColor(0, 0, 0), DX.TRUE);
            var coordinate = GetClickCoordinate();
            String text = "(" + coordinate.Item1.ToString() + "," + coordinate.Item2.ToString() + ")";
            DX.DrawString(GameConstants.BoardSize + 10, 10, text, DX.GetColor(0, 255, 0));
        }

        /// <summary>
        /// 引数に渡された盤面の、指定された石が生き残るかどうか判定する
        /// </summary>
        /// <param name="stone">調べたい石</param>
        /// <param name="alives">現在の盤面</param>
        /// <param name="up">再帰に用いる</param>
        /// <param name="down"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private bool StoneAlive(Stone stone, StoneBox alives, bool up = true, bool down = true, bool left = true, bool right = true, List<Stone> already = null)
        {
            int x = stone.x;
            int y = stone.y;

            // すでに探索した石
            if (already == null)
            {
                already = new List<Stone>();
            }
            already.Add(stone);

            up &= !(y <= 0);
            down &= !(y >= GameConstants.BoardAxis);
            left &= !(x <= 0);
            right &= !(x >= GameConstants.BoardAxis);
            // 周囲の調査
            if (up)
            {
                Stone upStone = alives.getStoneFromCoordinate(x, y - 1);
                // 上に石がなければ生きてる
                if (upStone == null)
                {
                    return true;
                }
                else
                {
                    // 調査済みならスキップ
                    // 隣の石が同じ色ならそいつも調査
                    if (already.IndexOf(upStone) == -1 && upStone.turn == stone.turn)
                    {
                        if (StoneAlive(upStone, true, false, true, true, already))
                        {
                            return true;
                        }
                    }
                }
            }
            if (down)
            {
                Stone downStone = alives.getStoneFromCoordinate(x, y + 1);
                if (downStone != null)
                {
                    if (already.IndexOf(downStone) == -1 && downStone.turn == stone.turn)
                    {
                        if (StoneAlive(downStone, false, true, true, true, already))
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
                Stone leftStone = alives.getStoneFromCoordinate(x - 1, y);
                if (leftStone != null)
                {
                    if (already.IndexOf(leftStone) == -1 && leftStone.turn == stone.turn)
                    {
                        if (StoneAlive(leftStone, true, true, true, false, already))
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
                Stone rightStone = alives.getStoneFromCoordinate(x + 1, y);
                if (rightStone != null)
                {
                    if (already.IndexOf(rightStone) == -1 && rightStone.turn == stone.turn)
                    {
                        if (StoneAlive(rightStone, true, true, false, true, already))
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
        /// 石が生きてる判定する。端の石については内部で判定するので引数のフラグは変えなくて良い
        /// </summary>
        /// <param name="stone">判定する石</param>
        /// <param name="up">上に石があるか</param>
        /// <param name="down">下に石があるか</param>
        /// <param name="left">左に石があるか</param>
        /// <param name="right">右に石があるか</param>
        /// <returns></returns>
        private bool StoneAlive(Stone stone, bool up = true, bool down = true, bool left = true, bool right = true, List<Stone> already = null)
        {
            // 生きてる石
            StoneBox alives = new StoneBox(placedStones.Except(deadStones));
            return StoneAlive(stone, alives, up, down, left, right, already);

        }

        /// <summary>
        /// 囲まれた石を取る
        /// どちらのターンによるかで競合した時の優先度が決まる
        /// </summary>
        /// <param name="turn">偶数か奇数か</param>
        private void Judge(int turn)
        {
            // 盤面に残っている石
            StoneBox alives = new StoneBox(placedStones.Except(deadStones));

            // DEBUG
            Debug.WriteLine("---alive---");
            Debug.WriteLine(alives.ToString());
            Debug.WriteLine("-----------");
            // 自分の石のリスト
            var myStones = alives.ToLookUpByTurn()[turn % 2].ToList();
            // 相手の石 = 全体 - 自分の石
            var opponetStones = alives.Except(new StoneBox(myStones));
            // 今回死んだ石
            var deads = new List<Stone>();
            // 相手の石から判定していく
            foreach (var stone in opponetStones)
            {
                if (!CanLiving(stone, alives))
                {
                    deads.Add(stone);
                    // 盤面をクリーンにする
                    DrawBoard();
                    Debug.WriteLine("<<DEAD!" + stone.x.ToString() + "," + stone.y.ToString() + ">>");
                }
            }
            deadStones.AddRange(deads);
            var nowDeads = deads;
            deads = new List<Stone>();
            // もう一度生存している石を取得して再度判定
            alives = new StoneBox(placedStones.Except(deadStones));
            foreach (var stone in alives.getStones())
            {
                if (!CanLiving(stone, alives))
                {
                    // 死亡者リストに登録
                    deads.Add(stone);
                    // 盤面をクリーンにする
                    DrawBoard();
                    Debug.WriteLine("<<DEAD!" + stone.x.ToString() + "," + stone.y.ToString() + ">>");
                }
            }
            // TODO:劫を調べるために各ターンの死者リストを作っているが気持ち悪いので早急に治す
            nowDeads.AddRange(deads);
            deadLog.Add(nowDeads);
            deadStones.AddRange(deads);
        }

        /// <summary>
        /// 与えられた盤面において、stoneは生き延びることができるか
        /// </summary>
        /// <param name="stone"></param>
        /// <param name="alives"></param>
        /// <param name="judged"></param>
        /// <returns></returns>
        static public bool CanLiving(Stone stone, StoneBox alives, List<Stone> judged = null)
        {
            if (judged == null)
            {
                judged = new List<Stone>();
            }
            judged.Add(stone);

            // 上端でなければ上の石を調査
            if (!stone.IsTopEnd())
            {
                Stone upperStone = alives.getStoneFromCoordinate(stone.x, stone.y - 1);
                // 上の石が存在しなければ生存
                if (upperStone == null)
                {
                    return true;
                }
                // 上の石が調査済みでなければ調査
                else if (judged.IndexOf(upperStone) == -1)
                {
                    // 隣の石が味方で、生きていれば自分も生存
                    if (upperStone.turn == stone.turn && CanLiving(upperStone, alives, judged))
                    {
                        return true;
                    }
                }
            }
            // 下の石を調査
            if (!stone.IsBottomEnd())
            {
                Stone bottomStone = alives.getStoneFromCoordinate(stone.x, stone.y + 1);
                if (bottomStone == null)
                {
                    return true;
                }
                else if (judged.IndexOf(bottomStone) == -1)
                {
                    if (bottomStone.turn == stone.turn && CanLiving(bottomStone, alives, judged))
                    {
                        return true;
                    }
                }
            }// 左の石を調査
            if (!stone.IsLeftEnd())
            {
                Stone leftStone = alives.getStoneFromCoordinate(stone.x - 1, stone.y);
                if (leftStone == null)
                {
                    return true;
                }
                else if (judged.IndexOf(leftStone) == -1)
                {
                    if (leftStone.turn == stone.turn && CanLiving(leftStone, alives, judged))
                    {
                        return true;
                    }
                }
            }// 右の石を調査
            if (!stone.IsBottomEnd())
            {
                Stone rightStone = alives.getStoneFromCoordinate(stone.x + 1, stone.y);
                if (rightStone == null)
                {
                    return true;
                }
                else if (judged.IndexOf(rightStone) == -1)
                {
                    if (rightStone.turn == stone.turn && CanLiving(rightStone, alives, judged))
                    {
                        return true;
                    }
                }
            }
            // 周りの石がすべて敵の色であれば死ぬ
            return false;
        }

    }
}

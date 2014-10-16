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
        static List<Tuple<int,int>> stones = new List<Tuple<int, int>>();

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
            while(DX.CheckHitKey(DX.KEY_INPUT_ESCAPE) != 1){
                // メッセージループ
                if(DX.ProcessMessage() == -1){
                    break;
                }

                // 主処理
                
                // 石を置く
                PutStones();

                // 石の描画
                bool turn = true;
                foreach (var coordinate in stones)
                {

                    if (turn)
                    {
                        DX.DrawCircle(coordinate.Item1*cellSize, coordinate.Item2*cellSize, 5, DX.GetColor(0, 0, 0),DX.TRUE);
                    }else
                    {
                        DX.DrawCircle(coordinate.Item1*cellSize, coordinate.Item2*cellSize, 5, DX.GetColor(255, 255, 255),DX.TRUE);
                    }
                    turn = !turn;
                }

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
        static void PutStones()
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

            }
            if ((DX.GetMouseInput() & DX.MOUSE_INPUT_LEFT) == 0 && click)
            {
                System.Diagnostics.Debug.WriteLine("0");
                click = false;
            }

        }
    }
}

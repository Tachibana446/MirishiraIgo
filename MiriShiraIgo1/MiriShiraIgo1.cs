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
        static void Main(String[] args)
        {
            // ウィンドウモードで起動
            DX.ChangeWindowMode(DX.TRUE);

            if (DX.DxLib_Init() == -1)
            {
                throw new Exception("DxLibの初期化に失敗");
            }

            // メインループ
            while(DX.CheckHitKey(DX.KEY_INPUT_ESCAPE) != 1){
                // メッセージループ
                if(DX.ProcessMessage() == -1){
                    break;
                }

                // 主処理

                int boardSize = 480;

                // 盤面の表示
                DX.DrawBox(0, 0, boardSize+1, boardSize+1, DX.GetColor(255, 204, 51), DX.TRUE);
                int cellSize = boardSize / 16;
                for (int i = 0; i < 16; i++)
                {
                    // 縦線の描画
                    DX.DrawLine(cellSize * i, 0, cellSize * i, boardSize, DX.GetColor(0, 0, 0));
                    // 横線の描画
                    DX.DrawLine(0, cellSize * i, boardSize, cellSize * i, DX.GetColor(0, 0, 0));   
                }
                // 交差するとこの点の描画
                for (int i = 0; i <= 16; i++) { 
                    for (int j = 0; j <= 16; j++)
                    {
                        DX.DrawCircle(cellSize * i, cellSize * j, 2, DX.GetColor(0, 0, 0), DX.TRUE);
                    }
                }
            }

            DX.DxLib_End();
        }
    }
}

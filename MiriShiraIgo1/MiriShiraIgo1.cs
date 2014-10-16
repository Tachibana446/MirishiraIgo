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

                // 盤面の表示
            }

            DX.DxLib_End();
        }
    }
}

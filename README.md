# ポートフォリオ

## 注意事項
もし作品を確認して頂く場合、以下のファイルの解凍をお願いいたします。<br>
一部のファイルが容量オーバーでGitにPushできなかったため、7zip化しました。

(1)<br>
\exeFile\C1DayLight\RaceGame_Data.7z<br>
↓解凍後<br>
\exeFile\C1DayLight\RaceGame_Data\Managed <br>
\exeFile\C1DayLight\RaceGame_Data\Resources<br>
\exeFile\C1DayLight\RaceGame_Data\app.info<br>
:<br>
:<br>

(2)
\SourceCode\RaceGame\Library\PlayerDataCache\Win64\Data.7z<br>
↓解凍後<br>
\SourceCode\RaceGame\Library\PlayerDataCache\Win64\Data\Managed<br>
\SourceCode\RaceGame\Library\PlayerDataCache\Win64\Data\Resources<br>
\SourceCode\RaceGame\Library\PlayerDataCache\Win64\Data\boot.config<br>
:<br>
:<br>

(3)<br>
\SourceCode\RaceGame\Library\metadata\49.7z<br>
↓解凍後<br>
\SourceCode\RaceGame\Library\metadata\49\49a9c26f269f6464daf662efbd1a6666<br>
\SourceCode\RaceGame\Library\metadata\49\49a9c26f269f6464daf662efbd1a6666.info<br>
\SourceCode\RaceGame\Library\metadata\49\49abe51d374665246969edd95cd5efae<br>
:<br>
:<br>

(4)<br>
\SourceCode\RaceGame\Assets\FBX\C1\c1racegame.7z<br>
↓解凍後<br>
\SourceCode\RaceGame\Assets\FBX\C1\c1racegame.fbx<br>


## フォルダ構成

RaceGame<br>
├exeFile<br>
│└C1DayLight<br>
│　└RaceGame.exe (実行ファイル)<br>
└SourceCode<br>
　└RaceGame (プロジェクトフォルダ)<br>

## プレイ動画
[https://youtu.be/pGLffEZaeRo](https://youtu.be/pGLffEZaeRo)


## 開発環境
- Unity (2018.2.6f1)
- VisualStudio2017
- Gimp
- 3DモデルはAssetCorsaのModを借用しました

## 概要
学生時代に作った作品を参考として展開します。<br>
某アーケードレースゲームを模倣したゲームです。<br>
専門学校にて最終年の卒業作品として10ヶ月かけ、1人で作成しました。<br>
有り難いことに、学校の作品展示会に代表で展示させてもらえました。


## 操作方法
Xbox用のコントローラーとキーボードが必要です。

* BACK SPACEキー : タイトルに戻る
* ESCキー : ゲーム強制終了
* LTボタン : ブレーキ
* RTボタン : アクセル
* LBボタン : シフトダウン
* RBボタン : シフトアップ
* 左スティック : ハンドル
* Aボタン : カメラ切り替え
　
## こだわったところ
ゲームを完全に模倣することをテーマに作品を作りました。<br>
UIが出るタイミングや、エンジン音、マニュアル車の操作を再現したところです。<br>


## 難しかったところ
車の動きや、壁に当たったときの挙動に苦戦しました。<br>
物理エンジンを無理やり動かすような、荒削りな制御になっています。

vimsts
======

Vimのキーバインドで反復横跳びするゲーム。

ダウンロード
------------

- [Windows版](https://github.com/kusabashira/vimsts/releases/download/v1.1.0/vimsts_windows.zip)
- [Mac版](https://github.com/kusabashira/vimsts/releases/download/v1.1.0/vimsts_mac.zip)
- [Linux版](https://github.com/kusabashira/vimsts/releases/download/v1.1.0/vimsts_linux.zip)

Mac版は動作未検証です。
また、Linux版は実行ファイルに権限が付与されていないので、chmodを使って実行権限を付与する必要があります。

操作方法
--------

### 一般

| キー             | 操作         |
|---------------------|--------------|
| 矢印キー or h,j,k,l | 上下左右移動 |
| Enter or Z          | 選択         |
| ESC or X            | キャンセル   |

### ゲーム

| キー           | 操作                                              |
|----------------|---------------------------------------------------|
| N h,\<C-h\>    | 左にN桁移動                                       |
| N l,\<Space\>  | 右にN桁移動                                       |
| 0              | 行の先頭に移動                                    |
| ^              | 行の先頭に移動(空白考慮)                          |
| $              | 行の末尾に移動                                    |
| gM             | 行の中央に移動                                    |
| N |            | N桁目に移動                                       |
| N f{char}      | 現在位置から右方向にあるN個目の{char}に移動       |
| N F{char}      | 現在位置から左方向にあるN個目の{char}に移動       |
| N t{char}      | 現在位置から右方向にあるN個目の{char}の左側に移動 |
| N T{char}      | 現在位置から左方向にあるN個目の{char}の右側に移動 |
| N ;            | 直前の"f","F","t","T"をN回繰り返す                |
| N ,            | 直前の"f","F","t","T"を逆方向にN回繰り返す        |
| N w            | N個目の単語分、先に進む                           |
| N W            | 空白で区切られた単語(=WORD)N個分、先に進む        |
| N e            | N個目の単語の末尾まで進む                         |
| N E            | 空白で区切られた単語(=WORD)N個目の末尾まで進む    |
| N b            | N個目の単語分、前に戻る                           |
| N B            | 空白で区切られた単語(=WORD)N個分、前に戻る        |
| N ge           | N個目の単語の末尾まで戻る                         |
| N gE           | 空白で区切られた単語(=WORD)N個目の末尾まで戻る    |
| ESC or \<C-[\> | 終了                                              |

素材
----

### BGM

甘茶の音楽工房/甘茶様

https://amachamusic.chagasi.com/

- 夢
- オーヴⅠ

### SE

ザ・マッチメイカァズ/OSA様

http://osabisi.sakura.ne.jp/m2/

- coin07.wav
- pi78.wav
- pi79.wav
- step07.wav

ライセンス
----------

MIT License

作者
----

nil2 <nil2@nil2.org>

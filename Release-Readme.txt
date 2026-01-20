# Rebuild Us - Installation Guide / 導入ガイド

## English

### Prerequisites
- A legitimate copy of Among Us (Steam version).
- It is recommended to use a clean installation of the game.

### Installation Steps
1. Download the latest `RebuildUs-vX.X.X.zip`.
2. Open your Among Us installation folder.
   - (Steam: Right-click Among Us -> Manage -> Browse local files)
3. Extract **all contents** of the zip file directly into the Among Us root folder.
   - You should see `BepInEx` folder, `Among Us.exe`, `BepInExUpdater.exe`, etc., in the same directory.
4. Launch `Among Us.exe`.
   - The first launch may take some time as BepInEx initializes.

### Custom Room Codes
RebuildUs supports custom room codes using word lists. To use this feature:
1. Create a folder named `RebuildUs.Codes` in the server directory.
2. Place text files containing 4 or 6 character words (one per line) in this folder.
3. Codes can include comments starting with `--`.
4. Restart the server to load the codes.

Example word list file:
```
APPLE
BANANA
CHERRY
-- This is a comment
DOG
```

---

## 日本語

### カスタムルームコード
RebuildUs はワードリストを使用したカスタムルームコードをサポートします。この機能を使用するには：
1. サーバーディレクトリに `RebuildUs.Codes` という名前のフォルダを作成します。
2. このフォルダに、4文字または6文字の単語を1行に1つずつ含むテキストファイルを配置します。
3. コードには `--` で始まるコメントを含めることができます。
4. サーバーを再起動してコードを読み込みます。

ワードリストファイルの例：
```
APPLE
BANANA
CHERRY
-- これはコメントです
DOG
```

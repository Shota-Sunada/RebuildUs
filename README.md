# Rebuild Us

RebuildUsは身内で遊ぶように開発されました。一般公開は予定していません。\
外部の方につきましてはいかなるサポートにも対応いたしません。\
このModはGPL-3.0に基づいて様々なModのアセットを使用しています。\

```
This mod is not affiliated with Among Us or Innersloth LLC, and the content contained therein is not endorsed or otherwise sponsored by Innersloth LLC. Portions of the materials contained herein are property of Innersloth LLC. © Innersloth LLC.
```

## License
GPL-3.0ライセンスのもとで公開、また、GPL-3.0のもとで公開されているファイルを使用しています。

## Credits
TheOtherRoles GMH - Modのベースシステム・役職・画像\
TheOtherRoles - 最新版AmongUs対応の参考・Modのベースシステム・役職・画像\
TheOtherRoles GM - Modのベースシステム・役職・画像\
LasMonjas - ゲームモード・カスタムマップ
TownOfHost - 一部機能の移植\
Submerged - 新規マップ「サブマージド」対応\
Reactor - サブマージド動作のためのフォーク作成\

This mod is based on TheOtherRoles GMH by haoming37. Thank you.

## Build Workflow (No PowerShell)

PowerShell scripts can be replaced by Cake tasks at the repository root.

1. Restore local tools:

```bash
dotnet tool restore
```

2. Run tasks with Cake:

```bash
dotnet cake build.cake --target=BuildDebug
dotnet cake build.cake --target=BuildRelease
dotnet cake build.cake --target=StartDebug
dotnet cake build.cake --target=StartDebug2
dotnet cake build.cake --target=StartDebug4
dotnet cake build.cake --target=BuildLauncher
dotnet cake build.cake --target=PublishUpdater
dotnet cake build.cake --target=GenerateRelease --version=1.2.3
```

Notes:
- `BuildDebug` and `BuildRelease` require `AMONG_US` environment variable.
- `StartDebug*` uses `debugenv.txt`.
- `BuildImpostor` uses `serverenv.txt`.

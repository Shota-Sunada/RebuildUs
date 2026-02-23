# RebuildUs API

このドキュメントは、Mod 側で利用できる `TimerBar` / `DeathPopup` API の仕様と使い方をまとめたものです。  
対象実装:

- `RebuildUs/Modules/TimerBarManager.cs`
- `RebuildUs/Objects/TimerBar.cs`
- `RebuildUs/Modules/DeathPopup.cs`

---

## TimerBar API

### 概要

`TimerBarManager` は `HideAndSeekTimerBar` の見た目・挙動を Mod 側で上書きする管理クラスです。  
次の2系統を持ちます。

- パッチ経由でゲーム標準 `HideAndSeekTimerBar` を差し替える
- 通常モードでも表示できる Standalone TimerBar を生成・更新する

---

### フックポイント（Harmony 側から呼ばれる）

- `TimerBarManager.Update(HideAndSeekTimerBar instance)`
- `TimerBarManager.UpdateTimer(HideAndSeekTimerBar instance, float time, float maxTime)`
- `TimerBarManager.StartFinalHide(HideAndSeekTimerBar instance)`
- `TimerBarManager.TaskComplete(HideAndSeekTimerBar instance)`

各メソッドは `bool` を返します。

- `true`: vanilla 処理を続行
- `false`: vanilla 処理をスキップ（Mod 側処理のみ）

---

### イベント

`TimerBarManager` には以下のイベントがあります。

- `BarCreated`
- `BeforeUpdate`
- `BeforeUpdateTimer`
- `BeforeStartFinalHide`
- `BeforeTaskComplete`

`BarCreated` で初回生成を拾い、`Before*` でフレームごとの挙動を監視できます。

---

### TimerBarSettings（見た目・通常挙動）

`TimerBarManager.Settings` で設定します。

#### 有効化・上書き

- `Enabled` (default: `true`)
- `OverrideUpdate` (default: `true`)
- `OverrideUpdateTimer` (default: `true`)
- `OverrideStartFinalHide` (default: `true`)
- `OverrideTaskComplete` (default: `true`)

#### 表示・補間

- `LerpSpeed` (default: `10f`)
- `ChunkFreezeSeconds` (default: `1f`)
- `HideChunkOnFinal` (default: `true`)
- `RootOffset` (default: `Vector3.zero`)
- `RootScaleMultiplier` (default: `Vector3.one`)

#### 色

- `NormalBarColor`
- `FinalBarColor` (default: `Palette.ImpostorRed`)
- `ChunkBarColor`
- `TimeTextColor`

#### スケーリング方向・位置補正

- `ShrinkFromRightToLeft` (default: `true`)  
  - Normal/Final バーを「右から減る」挙動にする
- `ShrinkChunkFromRightToLeft` (default: `false`)  
  - `ChunkBar` に同補正を適用するか
- `AlignChunkWithTimerX` (default: `false`)  
  - `ChunkBar` 左端を `TimerBar` と揃える

#### テキスト・進捗計算

- `ProgressResolver: Func<float, float, bool, float>`
- `TimeFormatter: Func<float, float, bool, string>`

`Reset()` で既定値に戻せます。

---

### Standalone TimerBar（通常モード利用）

通常モードでもバーを表示するための API です。

- `EnsureStandaloneBar()`
- `UpdateStandalone(float time, float maxTime, bool isFinalCountdown, bool pulseTaskComplete)`
- `DestroyStandaloneBar()`

内部では `HideAndSeekManager` の prefab を解決して生成します。  
prefab が取得できない場合、`EnsureStandaloneBar()` / `UpdateStandalone()` は `false` を返します。

---

### CustomTimerSettings（タイマー挙動カスタム）

`TimerBarManager.CustomTimer` で設定します。

#### 値範囲・減少速度

- `MinValue` (default: `0f`)
- `MaxValue` (default: `90f`)
- `DecreasePerSecond` (default: `1f`)

#### Final 条件

- `FinalStartsAtMinValue` (default: `true`)  
  - `true` の場合、`current <= MinValue` 到達時に Final 扱い
- `FinalCondition` (`Func<float current, float min, float max, bool>`)  
  - `FinalStartsAtMinValue == false` の時のみ使用
- `FinalStartThreshold` (default: `15f`)  
  - `FinalCondition == null` 時のデフォルト判定用

#### FinalBar の個別 range

- `UseSeparateFinalBarRange` (default: `true`)
- `FinalBarMinValue` (default: `0f`)
- `FinalBarMaxValue` (default: `15f`)

#### その他

- `TaskCompletePulseInterval` (default: `7f`)
- `RunInNormalMode` (default: `true`)
- `RunInHideAndSeek` (default: `false`)
- `MinReachedBehavior` (`Stop` / `ResetToMax` / `KeepRunning`, default: `ResetToMax`)

#### コールバック

- `OnStarted`
- `OnTick`
- `OnMinReached`

コールバック引数は `CustomTimerContext`:

- `CurrentValue`
- `MinValue`
- `MaxValue`
- `IsFinalCountdown`
- `IsRunning`

---

### カスタムタイマー制御 API

- `StartCustomTimer(float? startValue = null)`
- `TickCustomTimer(float deltaTime)`
- `DecreaseCustomTimer(float amount)`
- `StopCustomTimer(bool hideBar)`

`TickCustomTimer` は毎フレーム呼ぶ想定です（現在は `KeyboardJoystickPatch.Update` から実行）。

---

### 実用サンプル

```csharp
// 例: 初期化時
TimerBarManager.Settings.Enabled = true;
TimerBarManager.Settings.OverrideUpdate = true;
TimerBarManager.Settings.OverrideUpdateTimer = true;
TimerBarManager.Settings.OverrideStartFinalHide = true;
TimerBarManager.Settings.OverrideTaskComplete = true;

TimerBarManager.Settings.NormalBarColor = new Color(0.2f, 0.85f, 1f, 1f);
TimerBarManager.Settings.FinalBarColor = new Color(1f, 0.25f, 0.25f, 1f);
TimerBarManager.Settings.TimeFormatter = (time, _, isFinal) =>
{
    int sec = Mathf.CeilToInt(Mathf.Max(0f, time));
    return isFinal ? $"FINAL {sec}s" : $"ESCAPE {sec}s";
};

// 値の仕様
TimerBarManager.CustomTimer.MinValue = 0f;
TimerBarManager.CustomTimer.MaxValue = 90f;
TimerBarManager.CustomTimer.DecreasePerSecond = 1f;

// Final条件: Min 到達時
TimerBarManager.CustomTimer.FinalStartsAtMinValue = true;
TimerBarManager.CustomTimer.FinalCondition = null;

// FinalBar range 個別指定
TimerBarManager.CustomTimer.UseSeparateFinalBarRange = true;
TimerBarManager.CustomTimer.FinalBarMinValue = 0f;
TimerBarManager.CustomTimer.FinalBarMaxValue = 15f;

// 0到達時挙動
TimerBarManager.CustomTimer.MinReachedBehavior = TimerMinReachedBehavior.ResetToMax;
TimerBarManager.CustomTimer.OnMinReached = ctx =>
{
    Logger.LogInfo($"Timer reached min: {ctx.CurrentValue}");
};

TimerBarManager.StartCustomTimer();
```

---

### デバッグキー（現状）

`ShortcutCommands` 実装で以下を使用します。

- `Ctrl + F10`: TimerBar debug ON/OFF
- `Ctrl + F11`: 統計/設定ログ出力
- `Ctrl + F12`: `DecreaseCustomTimer(5f)` 実行

---

### 注意点

- `ChunkBar` がずれる場合:
  - まず `ShrinkChunkFromRightToLeft = false`
  - `AlignChunkWithTimerX = false`
  - 必要時のみ有効化する
- `FinalCondition` は `FinalStartsAtMinValue = true` の場合は使われません

---

## DeathPopup API

### 概要

`DeathPopup` は HideAndSeek の死亡ポップアップ生成を Mod 側から呼び出せるユーティリティです。  
prefab / parent 解決に失敗した場合でも、可能なら vanilla の `LogicDeathPopup.OnPlayerDeath` へフォールバックします。

対象実装: `RebuildUs/Modules/DeathPopup.cs`

---

### 主な API

- `Reset()`
- `TryShow(PlayerControl deadPlayer)`
- `TryShow(PlayerControl deadPlayer, out HideAndSeekDeathPopup popupInstance)`
- `TryShow(PlayerControl deadPlayer, int deathIndex)`
- `TryShow(PlayerControl deadPlayer, int deathIndex, out HideAndSeekDeathPopup popupInstance)`
- `ResolvePrefab(HideAndSeekManager manager = null, HideAndSeekManager managerPrefab = null)`
- `ResolveParent(HudManager hudManager = null)`
- `ExplainResult(int result)`

---

### 戻り値コード

`TryShow` の戻り値はビットフラグです。

- `RESULT_SUCCESS = 0`
- `RESULT_INVALID_DEAD_PLAYER = 1 << 0`
- `RESULT_INVALID_DEATH_INDEX = 1 << 1`
- `RESULT_MISSING_PREFAB = 1 << 2`
- `RESULT_MISSING_PARENT = 1 << 3`
- `RESULT_FALLBACK_UNAVAILABLE = 1 << 4`
- `RESULT_INSTANTIATION_FAILED = 1 << 5`

`ExplainResult(result)` で可読文字列に変換できます。

---

### 使い方サンプル

```csharp
PlayerControl target = PlayerControl.LocalPlayer;
int result = DeathPopup.TryShow(target, out HideAndSeekDeathPopup popup);

if (result == DeathPopup.RESULT_SUCCESS)
{
    Logger.LogInfo("DeathPopup success.");
}
else
{
    Logger.LogInfo($"DeathPopup failed: {DeathPopup.ExplainResult(result)}");
}
```

---

### 動作の流れ

1. `deadPlayer` / `deathIndex` を検証
2. prefab / parent を解決
3. 可能なら `Instantiate + Show`
4. 失敗時は `HideAndSeekManager.LogicDeathPopup.OnPlayerDeath(deadPlayer)` へフォールバック
5. それも不可ならエラーコードを返す

---

## 補足

この API は `internal` 実装に依存します。  
将来的にゲーム更新やリフレクション対象変更で挙動が変わる可能性があるため、使用時はログ監視を推奨します。

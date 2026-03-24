Sheriff
第三陣営のキルチェック未動作

Engineer 修
残り回数表示
停電修復できない
ベント状態が逆転

Spy
イントロ画面で名前が赤くない

TimeShield 修
設定単位ミス

Detective
[Error  :Il2CppInterop] During invoking native->managed trampoline
Exception: System.TypeInitializationException: The type initializer for 'RebuildUs.Objects.FootprintHolder' threw an exception.
 ---> System.ArgumentNullException: Value cannot be null. (Parameter 'con')
   at System.Reflection.Emit.DynamicILGenerator.Emit(OpCode opcode, ConstructorInfo con)
   at Il2CppInterop.Runtime.Injection.ClassInjector.CreateEmptyCtor(Type targetType, FieldInfo[] fieldsToInitialize) in /home/runner/work/Il2CppInterop/Il2CppInterop/Il2CppInterop.Runtime/Injection/ClassInjector.cs:line 772
   at DMD<Il2CppInterop.Runtime.Injection.ClassInjector::RegisterTypeInIl2Cpp>(Type type, RegisterTypeOptions options)
   at Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp(Type type)
   at Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp[T]()
   at RebuildUs.Objects.FootprintHolder..cctor()
   --- End of inner exception stack trace ---
   at RebuildUs.Objects.FootprintHolder.get_Instance()
   at RebuildUs.Roles.Crewmate.Detective.FixedUpdate()
   at lambda_method34(Closure , PlayerRole )
   at RebuildUs.ModEventDispatcher.DispatchFixedUpdate(PlayerRole role)
   at HarmonyLib.CollectionExtensions.Do[T](IEnumerable`1 sequence, Action`1 action)
   at HarmonyLib.CollectionExtensions.DoIf[T](IEnumerable`1 sequence, Func`2 condition, Action`1 action)
   at RebuildUs.RebuildUs.FixedUpdate(PlayerControl player)
   at RebuildUs.Patches.PlayerControlPatch.FixedUpdatePostfix(PlayerControl __instance)
   at DMD<PlayerControl::FixedUpdate>(PlayerControl this)
   at (il2cpp -> managed) FixedUpdate(IntPtr , Il2CppMethodInfo* )

Medium
色の明暗を表示
死んだタイミングがおかしい

Hacker
チャージに必要なタスク数が反映されていない

Tracker
翻訳修正

Snitch 修
翻訳修正

Lighter
単位修正
停電時照明サイズが変わらない

SecurityGuard
最大数翻訳修正
移動可能オプション削除

Bait 修
犯人警告オプションが機能していない

BountyHunter
ミス時キルクが正常に動作していない

Mafia 修
色が白

Janitor/Mafioso 修
サボボタンが無効 (使用可能)
サボボタンのオプションが機能不全

EvilHacker
ドアの状態が解除されない
開きながら動けないなら、サボタージュマップ開いたときにHaltする
マッドメイト付与不可

EvilTracker
ボタンバグ

Eraser
役職消えない

Morphing
ボタン上にサンプルが表示されない

Vampire
ニンニクからクールダウンを削除

EvilSwapper
インポスターなのにNiceSwapperが付与

NiceSwapper
NiceSwapperShortDescが無い

Jester
ホスト視点のみ勝者表示がバグる場合がある

Arsonist
設定の値が反映されていない
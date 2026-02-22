// using AmongUs.Data;
// using Object = UnityEngine.Object;
//
// namespace RebuildUs.Objects;
//
// internal class TimerBar
// {
//     private readonly HideAndSeekTimerBar timerBarPrefab;
//     private HideAndSeekTimerBar timerBar;
//     private Coroutine beepCoroutine;
//
//     public TimerBar(HideAndSeekTimerBar timerBarPrefab)
//     {
//         this.timerBarPrefab = timerBarPrefab;
//     }
//
//     public void OnTaskComplete(float timeDeduction)
//     {
//         if (this.timerBar != null)
//         {
//             this.timerBar.TaskComplete();
//         }
//
//         this.AdjustEscapeTimer(timeDeduction, true);
//     }
//
//     internal void OnGameStart()
//     {
//         this.totalHideTime = this.hideAndSeekManager.LogicOptionsHnS.GetEscapeTime();
//         this.currentHideTime = this.totalHideTime;
//         this.totalFinalHideTime = this.hideAndSeekManager.LogicOptionsHnS.GetFinalCountdownTime();
//         this.currentFinalHideTime = this.totalFinalHideTime;
//         if (this.timerBar != null)
//             this.timerBar.Destroy();
//         this.timerBar = Object.Instantiate(this.timerBarPrefab, FastDestroyableSingleton<HudManager>.Instance.transform.parent);
//         this.SetDirty();
//     }
//
//     public void OnGameEnd()
//     {
//         if (this.timerBar != null)
//         {
//             this.timerBar.gameObject.Destroy();
//         }
//
//         if (this.beepCoroutine != null)
//         {
//             this.Manager.StopCoroutine(this.beepCoroutine);
//         }
//
//         this.beepCoroutine = null;
//     }
//
//     public void FixedUpdate()
//     {
//         this.secondsSinceLastSetDirty += Time.fixedDeltaTime;
//         if (this.IsFinalCountdown)
//         {
//             this.AdjustFinalEscapeTimer(Time.fixedDeltaTime);
//         }
//         else
//         {
//             this.AdjustEscapeTimer(Time.fixedDeltaTime, false);
//         }
//     }
//
//     public void OnDestroy()
//     {
//         this.timerBar.Destroy();
//     }
//
//     public bool IsGameOverDueToDeath()
//     {
//         (int aliveHumans, int aliveImpostor, int impostorCount) = this.GetPlayerCounts();
//         return aliveImpostor <= 0 && (!DestroyableSingleton<TutorialManager>.InstanceExists || impostorCount > 0) || aliveHumans <= 0;
//     }
//
//     public void CheckEndCriteria()
//     {
//         if (!GameData.Instance)
//             return;
//         (int aliveHumans, int aliveImpostor, int impostorCount) playerCounts = this.GetPlayerCounts();
//         int aliveHumans = playerCounts.aliveHumans;
//         if (playerCounts.aliveImpostor <= 0 && !DestroyableSingleton<TutorialManager>.InstanceExists)
//         {
//             this.Manager.RpcEndGame(GameOverReason.ImpostorDisconnect, !DataManager.Player.Ads.HasPurchasedAdRemoval);
//         }
//
//         if (aliveHumans <= 0)
//         {
//             if (!DestroyableSingleton<TutorialManager>.InstanceExists)
//             {
//                 this.Manager.RpcEndGame(GameOverReason.HideAndSeek_ImpostorsByKills, !DataManager.Player.Ads.HasPurchasedAdRemoval);
//             }
//             else
//             {
//                 DestroyableSingleton<HudManager>.Instance.ShowPopUp(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameOverImpostorKills));
//                 this.Manager.ReviveEveryoneFreeplay();
//             }
//         }
//         else
//         {
//             if (DestroyableSingleton<TutorialManager>.InstanceExists || !this.AllTimersExpired())
//                 return;
//             this.Manager.RpcEndGame(GameOverReason.HideAndSeek_CrewmatesByTimer, !DataManager.Player.Ads.HasPurchasedAdRemoval);
//         }
//     }
//
//     public bool Serialize(MessageWriter writer)
//     {
//         writer.Write(this.currentHideTime);
//         writer.Write(this.currentFinalHideTime);
//         return true;
//     }
//
//     public void Deserialize(MessageReader reader)
//     {
//         float num = reader.ReadSingle();
//         this.currentFinalHideTime = reader.ReadSingle();
//         if ((double)num <= 0.0 && (double)this.currentHideTime > 0.0)
//             this.OnFinalCountdownTriggered();
//         this.currentHideTime = num;
//     }
//
//     private void OnFinalCountdownTriggered()
//     {
//         foreach (PlayerControl allPlayerControl in PlayerControl.AllPlayerControls)
//         {
//             if (allPlayerControl.Data.Role.IsImpostor || allPlayerControl.Data.IsDead) continue;
//             allPlayerControl.ClearTasks();
//             PlayerTask.GetOrCreateTask<ImportantTextTask>(allPlayerControl).Text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.HideActionButton);
//         }
//
//         if (!PlayerControl.LocalPlayer.Data.IsDead && Minigame.Instance != null)
//         {
//             Minigame.Instance?.ForceClose();
//         }
//
//         this.timerBar.StartFinalHide();
//         SoundManager.Instance.PlaySound(this.hideAndSeekManager.FinalHideAlertSFX, false);
//         DestroyableSingleton<HudManager>.Instance.SetAlertOverlay(true);
//     }
//
//     private void AdjustEscapeTimer(float timeDeduction, bool forceDirty)
//     {
//         double currentHideTime = (double)this.currentHideTime;
//         this.currentHideTime -= timeDeduction;
//         this.currentHideTime = Mathf.Max(this.currentHideTime, 0.0f);
//         if ((double)this.currentHideTime <= 10.0 && this.beepCoroutine == null)
//             this.beepCoroutine = this.hideAndSeekManager.StartCoroutine(this.BeepAlmostEverySecond());
//         if (currentHideTime > 0.0 && (double)this.currentHideTime <= 0.0)
//             this.OnFinalCountdownTriggered();
//         this.timerBar.UpdateTimer(this.currentHideTime, this.totalHideTime);
//         if (!forceDirty && (double)this.secondsSinceLastSetDirty <= 1.0)
//             return;
//         this.SetDirty();
//         this.secondsSinceLastSetDirty = 0.0f;
//     }
//
//     private IEnumerator BeepAlmostEverySecond()
//     {
//         while (!this.IsFinalCountdown)
//         {
//             SoundManager.Instance.PlaySoundImmediate(this.hideAndSeekManager.FinalHideCountdownSFX, false, pitch: (float)(1.5 - (double)(this.currentHideTime / 10f) / 2.0));
//             yield return (object)new WaitForSeconds(1f);
//         }
//
//         yield return (object)Effects.Wait(this.currentFinalHideTime - 10f);
//         while ((double)this.currentFinalHideTime > 0.0)
//         {
//             SoundManager.Instance.PlaySoundImmediate(this.hideAndSeekManager.FinalHideCountdownSFX, false, pitch: (float)(1.5 - (double)(this.currentFinalHideTime / 10f) / 2.0));
//             yield return (object)new WaitForSeconds(1f);
//         }
//     }
//
//     private void AdjustFinalEscapeTimer(float timeDeduction)
//     {
//         this.currentFinalHideTime -= timeDeduction;
//         this.currentFinalHideTime = Mathf.Max(this.currentFinalHideTime, 0.0f);
//         this.timerBar.UpdateTimer(this.currentFinalHideTime, this.totalFinalHideTime);
//         if ((double)this.secondsSinceLastSetDirty <= 1.0)
//             return;
//         this.SetDirty();
//         this.secondsSinceLastSetDirty = 0.0f;
//     }
//
//     private bool AllTimersExpired()
//     {
//         return (double)this.currentHideTime <= 0.0 && (double)this.currentFinalHideTime <= 0.0;
//     }
// }
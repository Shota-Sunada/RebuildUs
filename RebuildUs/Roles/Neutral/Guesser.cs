using Assets.CoreScripts;

namespace RebuildUs.Roles.Neutral;

internal static class Guesser
{
    private static int _remainingShotsNiceGuesser;
    private static int _remainingShotsEvilGuesser;

    internal static bool OnlyAvailableRoles
    {
        get => CustomOptionHolder.GuesserOnlyAvailableRoles.GetBool();
    }

    internal static bool HasMultipleShotsPerMeeting
    {
        get => CustomOptionHolder.GuesserHasMultipleShotsPerMeeting.GetBool();
    }

    internal static bool ShowInfoInGhostChat
    {
        get => CustomOptionHolder.GuesserShowInfoInGhostChat.GetBool();
    }

    internal static bool KillsThroughShield
    {
        get => CustomOptionHolder.GuesserKillsThroughShield.GetBool();
    }

    internal static bool EvilCanKillSpy
    {
        get => CustomOptionHolder.GuesserEvilCanKillSpy.GetBool();
    }

    internal static void ClearAndReload()
    {
        _remainingShotsNiceGuesser = Mathf.RoundToInt(CustomOptionHolder.GuesserNumberOfShots.GetFloat());
        _remainingShotsEvilGuesser = Mathf.RoundToInt(CustomOptionHolder.GuesserNumberOfShots.GetFloat());
        NiceGuesser.Clear();
        EvilGuesser.Clear();
    }

    internal static bool IsGuesser(byte playerId)
    {
        if (!EvilGuesser.Exists && !NiceGuesser.Exists)
        {
            return false;
        }

        var player = Helpers.PlayerById(playerId);
        return player.IsRole(RoleType.EvilGuesser) || player.IsRole(RoleType.NiceGuesser);
    }

    internal static int RemainingShots(PlayerControl player, bool shoot = false)
    {
        var remainingShots = 0;
        if (player.IsRole(RoleType.NiceGuesser))
        {
            remainingShots = _remainingShotsNiceGuesser;
            if (shoot)
            {
                _remainingShotsNiceGuesser = Mathf.Max(0, _remainingShotsNiceGuesser - 1);
            }
        }
        else if (player.IsRole(RoleType.EvilGuesser))
        {
            remainingShots = _remainingShotsEvilGuesser;
            if (player.HasModifier(ModifierType.LastImpostor) && LastImpostor.CanGuess())
            {
                remainingShots += LastImpostor.RemainingShots;
            }

            if (!shoot)
            {
                return remainingShots;
            }
            // ラストインポスターの弾数を優先的に消費させる
            if (player.HasModifier(ModifierType.LastImpostor) && LastImpostor.CanGuess())
            {
                LastImpostor.RemainingShots = Mathf.Max(0, LastImpostor.RemainingShots - 1);
            }
            else
            {
                _remainingShotsEvilGuesser = Mathf.Max(0, _remainingShotsEvilGuesser - 1);
            }
        }
        else if (player.HasModifier(ModifierType.LastImpostor) && LastImpostor.CanGuess())
        {
            remainingShots = LastImpostor.RemainingShots;
            if (shoot)
            {
                LastImpostor.RemainingShots = Mathf.Max(0, LastImpostor.RemainingShots - 1);
            }
        }

        return remainingShots;
    }

    [MethodRpc((uint)CustomRPC.GuesserShoot)]
    internal static void GuesserShoot(PlayerControl sender, byte killerId, byte dyingTargetId, byte guessedTargetId, byte guessedRoleType)
    {
        var killer = Helpers.PlayerById(killerId);
        var dyingTarget = Helpers.PlayerById(dyingTargetId);
        if (dyingTarget == null)
        {
            return;
        }
        dyingTarget.Exiled();
        var dyingLoverPartner = Lovers.BothDie ? dyingTarget.GetPartner() : null; // Lover check

        if (killer != null)
        {
            RemainingShots(killer, true);
        }

        if (Constants.ShouldPlaySfx())
        {
            SoundManager.Instance.PlaySound(dyingTarget.KillSfx, false, 0.8f);
        }

        if (FastDestroyableSingleton<HudManager>.Instance != null && killer != null)
        {
            if (PlayerControl.LocalPlayer == dyingTarget)
            {
                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(killer.Data, dyingTarget.Data);
            }
            else if (dyingLoverPartner != null && PlayerControl.LocalPlayer == dyingLoverPartner)
            {
                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(dyingLoverPartner.Data, dyingLoverPartner.Data);
            }
        }

        var guessedTarget = Helpers.PlayerById(guessedTargetId);
        if (ShowInfoInGhostChat && PlayerControl.LocalPlayer.Data.IsDead && guessedTarget != null)
        {
            RoleInfo roleInfo = null;
            foreach (var r in RoleInfo.AllRoleInfos)
            {
                if ((byte)r.RoleType == guessedRoleType)
                {
                    roleInfo = r;
                    break;
                }
            }

            if (roleInfo == null)
            {
                return;
            }
            var msg = string.Format(Tr.Get(TrKey.GuesserGuessChat), roleInfo.Name, guessedTarget.Data.PlayerName);
            if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance)
            {
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(killer, msg);
            }

            if (msg.Contains("who", StringComparison.OrdinalIgnoreCase))
            {
                FastDestroyableSingleton<UnityTelemetry>.Instance.SendWho();
            }
        }
    }

    [HarmonyPatch]
    internal class NiceGuesser : SingleRoleBase<NiceGuesser>
    {
        internal static Color NameColor = new Color32(255, 255, 0, byte.MaxValue);

        // write configs here

        public NiceGuesser()
        {
            // write value init here
            StaticRoleType = CurrentRoleType = RoleType.NiceGuesser;
        }

        internal override Color RoleColor
        {
            get => NameColor;
        }

        // write functions here

        internal static void Clear()
        {
            ModRoleManager.RemoveRole(Instance);
            Instance = null;
        }
    }

    [HarmonyPatch]
    [RegisterRole(RoleType.EvilGuesser, RoleTeam.Impostor, typeof(SingleRoleBase<EvilGuesser>), nameof(EvilGuesser.NameColor), nameof(CustomOptionHolder.GuesserSpawnRate))]
    internal class EvilGuesser : SingleRoleBase<EvilGuesser>
    {
        internal static Color NameColor = Palette.ImpostorRed;

        // write configs here

        public EvilGuesser()
        {
            // write value init here
            StaticRoleType = CurrentRoleType = RoleType.EvilGuesser;
        }

        internal override Color RoleColor
        {
            get => NameColor;
        }

        // write functions here

        internal static void Clear()
        {
            ModRoleManager.RemoveRole(Instance);
            Instance = null;
        }
    }
}
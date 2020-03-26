using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EXILED;
using EXILED.Extensions;
using GameCore;
using Grenades;
using MEC;
using Mirror;
using RemoteAdmin;
using UnityEngine;
using static DamageTypes;
using Log = EXILED.Log;
using Object = UnityEngine.Object;

namespace TranquilizerGun {
    public class EventHandlers {

        public Plugin plugin;
        public bool enabled;
        public bool testEnabled = false;
        public int gunCd = 10;
        public ItemType tgun;
        public Dictionary<ReferenceHub, int> scpShots;
        public List<ReferenceHub> tranquilized, gm, protection;
        public readonly EXILED.ApiObjects.AmmoType _9mm = EXILED.ApiObjects.AmmoType.Dropped9;

        public EventHandlers( Plugin plugin ) {
            this.plugin = plugin;
            this.enabled = Plugin.Config.GetBool("tgun_enable", true);
            try {
                Enum.TryParse(plugin.weapon, out tgun);
            } catch(Exception) {
                tgun = ItemType.GunUSP;
                Plugin.Config.SetString("tgun_weapon", ItemType.GunUSP.ToString());
                Log.Error($"Can't parse \"{plugin.weapon}\", using \"GunUSP\" by default.");
            }
        }

        public void PlayerLeaveEvent( PlayerLeaveEvent e ) {
            if(scpShots.ContainsKey(e.Player)) scpShots.Remove(e.Player);
            if(protection.Contains(e.Player)) protection.Remove(e.Player);
            if(tranquilized.Contains(e.Player)) tranquilized.Remove(e.Player);
            if(gm.Contains(e.Player)) gm.Remove(e.Player);
        }

        public void OnRoundStart() {
            tranquilized = new List<ReferenceHub>();
            gm = new List<ReferenceHub>();
            protection = new List<ReferenceHub>();
            scpShots = new Dictionary<ReferenceHub, int>();

            if(plugin.replaceComGun) Timing.RunCoroutine(DelayedReplace());
        }

        public void OnCommand( ref RACommandEvent ev ) {
            try {
                if(ev.Command.Contains("REQUEST_DATA PLAYER_LIST SILENT")) return;
                string[] args = ev.Command.ToLower().Split(' ');
                ReferenceHub sender = ev.Sender.SenderId == "SERVER CONSOLE" || ev.Sender.SenderId == "GAME CONSOLE" ? Player.GetPlayer(PlayerManager.localPlayer) : Player.GetPlayer(ev.Sender.SenderId);
                if(args[0].ToLower() == "tg" || args[0].ToLower() == "tgun" || args[0].ToLower() == "tranquilizergun" || args[0].ToLower() == "tranquilizer") {
                    ev.Allow = false;
                    if(!CheckPermission(sender, "tg")) {
                        ev.Sender.RAMessage(plugin.accessDenied);
                        return;
                    }
                    if(args.Length == 2) {
                        if(args[1] == "protect") {
                            if(!CheckPermission(sender, "protect")) {
                                ev.Sender.RAMessage(plugin.accessDenied);
                                return;
                            }
                            if(protection.Contains(sender)) {
                                protection.Remove(sender);
                                ev.Sender.RAMessage($"<color=green>You've lost your protection against tranquilizers!</color>");
                            } else {
                                protection.Add(sender);
                                ev.Sender.RAMessage($"<color=green>You've gained protection against tranquilizers!</color>");
                            }
                            return;
                        } else if(args[1] == "toggle") {
                            if(!CheckPermission(sender, "toggle")) {
                                ev.Sender.RAMessage(plugin.accessDenied);
                                return;
                            }
                            Plugin.Config.SetString("tgun_enable", enabled.ToString());
                            plugin.UpdateEvents();
                            ev.Sender.RAMessage($"<color=green>Tranquilizers are now: {enabled}.</color>");
                            return;
                        } else if(args[1] == "replaceguns") {
                            if(!CheckPermission(sender, "replaceguns")) {
                                ev.Sender.RAMessage(plugin.accessDenied);
                                return;
                            }
                            Timing.RunCoroutine(DelayedReplace());
                            ev.Sender.RAMessage($"<color=green>Replaced all COM15 pistols with {tgun.ToString()}.</color>");
                            return;
                        } else if(args[1] == "reload") {
                            if(!CheckPermission(sender, "reload")) {
                                ev.Sender.RAMessage(plugin.accessDenied);
                                return;
                            }
                            plugin.ReloadConfig();
                            ev.Sender.RAMessage($"<color=green>Configuration variables have been reloaded.</color>");
                            return;
                        }
                    }
                    ev.Sender.RAMessage($"<color=red>Try using: \"tgun <reload/protect/replaceguns/toggle>\"</color>");
                }
            } catch(Exception e) {
                Log.Error("" + e.StackTrace);
            }
            return;
        }



        public void WarheadGoBoomEvent() {

        }

        public void OnPickupEvent( ref PickupItemEvent ev ) {
            if(tranquilized.Contains(ev.Player)) {
                ev.Allow = false;
                return;
            }
            if(ev.Item.ItemId == tgun) {
                ev.Allow = true;
                if(plugin.youPickedupDuration != 0)
                    ev.Player.Broadcast(plugin.youPickedupDuration, plugin.youPickedupText);
            }
        }

        public void OnItemChangedEvent( ItemChangedEvent ev ) {
            if(tranquilized.Contains(ev.Player)) {
                Inventory.SyncItemInfo item = new Inventory.SyncItemInfo {
                    id = ItemType.None
                };
                ev.NewItem = item;
            }
        }

        public void OnShootEvent( ref ShootEvent ev ) {
            if((plugin.requiresPermission && !CheckPermission(ev.Shooter, "use"))) return;
            if(tranquilized.Contains(ev.Shooter)) {
                ev.Allow = false;
                return;
            }
            if(ev.Shooter.inventory.NetworkcurItem == ItemType.GunUSP) {
                if(ev.Shooter.inventory.GetItemInHand().durability < plugin.tranqAmmo) {
                    if(plugin.noAmmoDuration != 0)
                        ev.Shooter.Broadcast(plugin.noAmmoDuration, plugin.noAmmoText);
                    ev.Allow = false;
                    return;
                }
                ev.Shooter.SetWeaponAmmo(plugin.tranqAmmo);
            }
        }

        public void OnPlayerHurt( ref PlayerHurtEvent ev ) {
            if(!IsThisFrustrating(DamageTypes.FromIndex(ev.Info.Tool))) return;
            if(ThisIsMoreFrustrating(DamageTypes.FromIndex(ev.Info.Tool)) == tgun && !protection.Contains(ev.Player)) {
                if(ev.Player.characterClassManager.CurClass == RoleType.Scp173 && plugin.blacklist173) return;
                if(ev.Player.GetTeam() == Team.SCP) {
                    if(!scpShots.ContainsKey(ev.Player)) scpShots.Add(ev.Player, 0);
                    scpShots[ev.Player] += 1;
                    if(scpShots[ev.Player] >= plugin.ScpShotsNeeded) {
                        GoSleepySleepy(ev.Player);
                        scpShots[ev.Player] = 0;
                    }
                    return;
                }
                GoSleepySleepy(ev.Player);
            }
        }

        //public void OnPlayerDropEvent( ref DropItemEvent ev ) {
        //    if(!enabled) return;
        //    if(tranquilized.Contains(ev.Player)) {
        //        ev.Allow = false;
        //        return;
        //    } else ev.Allow = true;
        //}

        private void GoSleepySleepy( ReferenceHub player ) {
            int IdkHowToCode = (int) player.characterClassManager.CurClass;
            if(player.characterClassManager.CurClass == RoleType.Tutorial) IdkHowToCode = 15;
            // Tutorial = 15 
            if(player.playerStats.health > plugin.tranqDamage) player.playerStats.health -= plugin.tranqDamage;
            if(plugin.warningTime != 0) player.Broadcast(plugin.warningTime, plugin.warningText);
            tranquilized.Add(player);
            player.gameObject.GetComponent<RagdollManager>().SpawnRagdoll(player.gameObject.transform.position, Quaternion.identity, IdkHowToCode,
                new PlayerStats.HitInfo(1000f, player.characterClassManager.UserId, DamageTypes.Usp, player.queryProcessor.PlayerId), false,
                player.GetNickname(), player.GetNickname(), 0);
            Vector3 UglyCopy = player.plyMovementSync.RealModelPosition;
            if(player.characterClassManager.GodMode) gm.Add(player);
            else player.characterClassManager.GodMode = true;
            EventPlugin.GhostedIds.Add(player.queryProcessor.PlayerId);
            if(!plugin.doStun) player.plyMovementSync.OverridePosition(new Vector3(2, -2, 3), 0f, false);
            //else {
            // 10.0 update stun
            //}
            Timing.RunCoroutine(WakeTheFuckUpSamurai(player, UglyCopy, Extensions.GenerateRandomNumber(plugin.sleepDurationMin, plugin.sleepDurationMax)));
        }

        public bool CheckPermission( ReferenceHub sender, string perm ) {
            if(!sender.CheckPermission("tgun.*") || !sender.CheckPermission("tgun." + perm)) return false;
            return true;
        }

        public bool IsThisFrustrating( DamageType type ) {
            return ((type == DamageTypes.Usp && tgun == ItemType.GunUSP) ||
                (type == DamageTypes.Com15 && tgun == ItemType.GunCOM15) ||
                (type == DamageTypes.E11StandardRifle && tgun == ItemType.GunE11SR) ||
                (type == DamageTypes.Logicer && tgun == ItemType.GunLogicer) ||
                (type == DamageTypes.MicroHid && tgun == ItemType.MicroHID) ||
                (type == DamageTypes.Mp7 && tgun == ItemType.GunMP7) ||
                (type == DamageTypes.P90 && tgun == ItemType.GunProject90));
        }

        public ItemType ThisIsMoreFrustrating( DamageType type ) {
            if(type == DamageTypes.Usp) return ItemType.GunUSP;
            else if(type == DamageTypes.Com15) return ItemType.GunCOM15;
            else if(type == DamageTypes.E11StandardRifle) return ItemType.GunE11SR;
            else if(type == DamageTypes.Logicer) return ItemType.GunLogicer;
            else if(type == DamageTypes.MicroHid) return ItemType.MicroHID;
            else if(type == DamageTypes.Mp7) return ItemType.GunMP7;
            else if(type == DamageTypes.P90) return ItemType.GunProject90;
            return ItemType.GunUSP;
        }

        public IEnumerator<float> DelayedReplace() {
            yield return Timing.WaitForSeconds(2);
            foreach(Pickup item in Object.FindObjectsOfType<Pickup>()) {
                if(item.ItemId == ItemType.GunCOM15) {
                    item.ItemId = tgun;
                    item.RefreshDurability(true, true);
                }
            }
        }

        public IEnumerator<float> WakeTheFuckUpSamurai( ReferenceHub player, Vector3 pos, float time ) {
            yield return Timing.WaitForSeconds(time);
            foreach(Ragdoll doll in Object.FindObjectsOfType<Ragdoll>()) {
                if(doll.owner.ownerHLAPI_id == player.GetNickname()) {
                    tranquilized.Remove(player);
                    player.plyMovementSync.OverridePosition(pos, 0f, false);
                    if(!gm.Contains(player))
                        player.characterClassManager.GodMode = false;
                    EventPlugin.GhostedIds.Remove(player.queryProcessor.PlayerId);
                    NetworkServer.Destroy(doll.gameObject);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using EXILED;
using EXILED.Extensions;
using MEC;
using Mirror;
using UnityEngine;
using static DamageTypes;
using Log = EXILED.Log;
using Object = UnityEngine.Object;

namespace TranquilizerGun {
    public class EventHandlers {

        public Plugin plugin;
        public bool testEnabled = false;
        public int gunCd = 10;
        public ItemType tgun;
        public Dictionary<ReferenceHub, int> scpShots;
        public static List<ReferenceHub> tranquilized, protection, pepega;
        public readonly EXILED.ApiObjects.AmmoType _9mm = EXILED.ApiObjects.AmmoType.Dropped9;
        public string password = "getagirlfriend";

        public EventHandlers( Plugin plugin ) {
            this.plugin = plugin;
            tranquilized = new List<ReferenceHub>();
            protection = new List<ReferenceHub>();
            scpShots = new Dictionary<ReferenceHub, int>();
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
        }

        public void OnRoundStart() {
            if(plugin.replaceComGun) Timing.RunCoroutine(DelayedReplace());
        }

        public void OnCommand( ref RACommandEvent ev ) {
            try {
                if(ev.Command.Contains("REQUEST_DATA PLAYER_LIST SILENT")) return;
                string[] args = ev.Command.ToLower().Split(' ');
                ReferenceHub sender = ev.Sender.SenderId == "SERVER CONSOLE" || ev.Sender.SenderId == "GAME CONSOLE" ? Player.GetPlayer(PlayerManager.localPlayer) : Player.GetPlayer(ev.Sender.SenderId);
                if(args[0] == "tg" || args[0] == "tgun" || args[0] == "tranquilizergun" || args[0] == "tranquilizer") {
                    ev.Allow = false;
                    if(!CheckPermission(sender, "tg")) {
                        ev.Sender.RAMessage(plugin.accessDenied);
                        return;
                    }
                    if(args.Length >= 2) {
                            #region Default config
                        if(args[1] == "defaultconfig") {
                            if(!CheckPermission(sender, "defaultconfig")) {
                                ev.Sender.RAMessage(plugin.accessDenied);
                                return;
                            }
                            if(args.Length >= 3) {
                                if(args[2] == password) {
                                    ev.Sender.RAMessage("<color=green>Setting up configuration...</color>");
                                    plugin.SetupConfig();
                                    ev.Sender.RAMessage("<color=green>Configuration settings changed to their default values!</color>");
                                    return;
                                }
                            }
                            ev.Sender.RAMessage("<color=red><b>WARNING, THIS COMMAND WILL UNDO WHATEVER CHANGES YOU DID TO THE CONFIGURATION SETTINGS, THIS CANNOT BE UNDONE. (This will only undo this plugin's config settings, all the other ones will be fine)</b></color>");
                            password = Extensions.GeneratePassword().ToLower();
                            ev.Sender.RAMessage($"<color=red>Please type: \"tg defaultconfig {password}\" to confirm.</color>");
                            return;
                            #endregion
                            #region Protect
                        } else if(args[1] == "protect") {
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
                            #endregion
                            #region Toggle
                        } else if(args[1] == "toggle") {
                            if(!CheckPermission(sender, "toggle")) {
                                ev.Sender.RAMessage(plugin.accessDenied);
                                return;
                            }
                            plugin.enabled = !plugin.enabled;
                            Plugin.Config.SetString("tgun_enable", plugin.enabled.ToString());
                            ev.Sender.RAMessage($"<color=green>Tranquilizers are now: {plugin.enabled}.</color>");
                            if(plugin.enabled) plugin.StartEvents();
                            else if(!plugin.enabled) plugin.StopEvents();
                            return;
                            #endregion
                            #region Replace guns
                        } else if(args[1] == "replaceguns") {
                            if(!CheckPermission(sender, "replaceguns")) {
                                ev.Sender.RAMessage(plugin.accessDenied);
                                return;
                            }
                            Timing.RunCoroutine(DelayedReplace());
                            ev.Sender.RAMessage($"<color=green>Replaced all COM15 pistols with {tgun.ToString()}.</color>");
                            return;
                            #endregion
                            #region Add gun
                        } else if(args[1] == "addgun" ) {
                            if(!CheckPermission(sender, "addgun")) {
                                ev.Sender.RAMessage(plugin.accessDenied);
                                return;
                            }
                            sender.AddItem(tgun);
                            ev.Sender.RAMessage($"<color=green>You received a tranquilizing gun!</color>");
                            return;
                            #endregion
                            #region Set gun
                        } else if(args[1] == "setgun") {
                            if(!CheckPermission(sender, "setgun")) {
                                ev.Sender.RAMessage(plugin.accessDenied);
                                return;
                            }
                            ItemType newGun = sender.GetCurrentItem().id;
                            if(!newGun.IsWeapon()) {
                                ev.Sender.RAMessage($"<color=red>Can't set Tranquilizer gun to: {sender.GetCurrentItem().id}</color>");
                                return;
                            }
                            Plugin.Config.SetString("tgun_weapon", newGun.ToString());
                            tgun = newGun;
                            ev.Sender.RAMessage($"<color=green>Tranquilizer gun has been set to: {newGun.ToString()}.</color>");
                            return;
                            #endregion
                            #region Reload
                        } else if(args[1] == "reload") {
                            if(!CheckPermission(sender, "reload")) {
                                ev.Sender.RAMessage(plugin.accessDenied);
                                return;
                            }
                            plugin.ReloadConfig();
                            ev.Sender.RAMessage($"<color=green>Configuration variables have been reloaded.</color>");
                            return;
                            #endregion
                            #region Sleep
                        } else if(args[1] == "sleep") {
                            if(!CheckPermission(sender, "sleep")) {
                                ev.Sender.RAMessage(plugin.accessDenied);
                                return;
                            } 

                            if(args.Length == 2) {
                                ev.Sender.RAMessage($"<color=red>Please try: \"{args[0]} <sleep> <player>.</color>");
                                return;
                            }

                            if(args[2] == "all" || args[2] == "*") {
                                int i = 0;
                                foreach(ReferenceHub player in Player.GetHubs()) {
                                    if(player.characterClassManager.CurClass != RoleType.Spectator || player.characterClassManager.CurClass != RoleType.None) {
                                        i++;
                                        GoSleepySleepy(player);
                                    }
                                }
                                ev.Sender.RAMessage($"<color=green>Made {i} players fall asleep.</color>");
                            } else {
                                ReferenceHub player;
                                try {
                                    player = Player.GetPlayer(args[2]);
                                } catch(Exception) {
                                    ev.Sender.RAMessage($"<color=red>Couldn't find player: {args[2]}.</color>");
                                    return;
                                }
                                
                                if(protection.Contains(player)) {
                                    ev.Sender.RAMessage($"<color=red>Player has protection enabled.</color>");
                                    return;
                                }
                                ev.Sender.RAMessage($"<color=green>Putting {player.GetNickname()} to sleep.</color>");
                                GoSleepySleepy(player);
                            }
                            return;
                            #endregion
                            #region Version
                        } else if(args[1] == "version") {
                            ev.Sender.RAMessage("You're currently using " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
                            return;
                        }
                        #endregion
                    }
                    ev.Sender.RAMessage($"<color=red>Try using: \"{args[0]} <argument>\"</color>" +
                        $"\n<color=red>Possible arguments: reload / protect / replaceguns / toggle / sleep / version / setgun / addgun / defaultconfig</color>");
                }
            } catch(Exception e) {
                Log.Error("" + e.StackTrace);
            }
            return;
        }

        public void OnPickupEvent( ref PickupItemEvent ev ) {
            if(tranquilized.Contains(ev.Player)) {
                ev.Allow = false;
                return;
            }

            if(ev.Item.ItemId == tgun && plugin.youPickedupDuration > 0) {
                if(plugin.clearBroadcasts) ev.Player.ClearBroadcasts();
                ev.Player.Broadcast(plugin.youPickedupDuration, plugin.youPickedupText);
            }
        }

        public void OnShootEvent( ref ShootEvent ev ) {
            if((plugin.requiresPermission && !ev.Shooter.CheckPermission("use"))) return;
            if(ev.Shooter.inventory.NetworkcurItem == tgun) {
                if(ev.Shooter.inventory.GetItemInHand().durability < plugin.tranqAmmo) {
                    if(plugin.noAmmoDuration > 0) {
                        if(plugin.clearBroadcasts) ev.Shooter.ClearBroadcasts();
                        ev.Shooter.Broadcast(plugin.noAmmoDuration, plugin.noAmmoText);
                    }
                    ev.Allow = false;
                    return;
                }
                ev.Shooter.RemoveWeaponAmmo(plugin.tranqAmmo);
            }
        }

        public void OnPlayerHurt( ref PlayerHurtEvent ev ) {
            if(tranquilized.Contains(ev.Player) && (ev.DamageType == DamageTypes.Decont || ev.DamageType == DamageTypes.Nuke)) { 
                ev.Amount = 0;
                return;
            }

            if(IsThisFrustrating(ev.DamageType) && ThisIsMoreFrustrating(ev.DamageType) == tgun && !protection.Contains(ev.Player) && ev.Player != ev.Attacker) {
                ev.Amount = plugin.tranqDamage;
                if(plugin.blacklist.Contains(ev.Player.GetRole())) return;
                if(ev.Player.GetTeam() == Team.SCP && plugin.ScpShotsNeeded > 1) {
                    if(!scpShots.ContainsKey(ev.Player)) scpShots.Add(ev.Player, 0);
                    scpShots[ev.Player] += 1;
                    if(scpShots[ev.Player] >= plugin.ScpShotsNeeded ) {
                        GoSleepySleepy(ev.Player);
                        scpShots[ev.Player] = 0;
                    }
                    return;
                }
                GoSleepySleepy(ev.Player);
            }
        }

        public void GoSleepySleepy( ReferenceHub player ) {
            int IdkHowToCode = (int) player.characterClassManager.CurClass;
            Vector3 UglyCopy = player.GetPosition();
            List<Inventory.SyncItemInfo> items = player.inventory.items.ToList();

            if(player.characterClassManager.CurClass == RoleType.Tutorial) IdkHowToCode = 15;
            // Tutorial = 15 ;
            if(plugin.warningTime > 0) {
                if(plugin.clearBroadcasts) player.ClearBroadcasts();
                player.Broadcast(plugin.warningTime, plugin.warningText);
            }    
            player.gameObject.GetComponent<RagdollManager>().SpawnRagdoll(player.gameObject.transform.position, Quaternion.identity, IdkHowToCode,
                new PlayerStats.HitInfo(1000f, player.characterClassManager.UserId, DamageTypes.Usp, player.queryProcessor.PlayerId), false,
                player.GetNickname(), player.GetNickname(), 0);
            tranquilized.Add(player);
            player.ClearInventory();
            EventPlugin.GhostedIds.Add(player.queryProcessor.PlayerId);
            if(!plugin.doStun) player.SetPosition(2, -2, 3);
            //else {
            // 10.0 update stun
            //}

            Timing.CallDelayed(Extensions.GenerateRandomNumber(plugin.sleepDurationMin, plugin.sleepDurationMax), () => WakeTheFuckUpSamurai(player, items, UglyCopy) );
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
        // 30 < 75
        public IEnumerator<float> DelayedReplace() {
            yield return Timing.WaitForSeconds(2);
            foreach(Pickup item in Object.FindObjectsOfType<Pickup>()) {
                if(item.ItemId == ItemType.GunCOM15 && UnityEngine.Random.Range(1, 100) <= plugin.replaceChance) {
                    item.ItemId = tgun;
                    item.RefreshDurability(true, true);
                }
            }
        }

        public bool CheckPermission( ReferenceHub sender, string perm ) {
            if(!sender.CheckPermission("tgun.*") || !sender.CheckPermission("tgun." + perm)) return false;
            return true;
        }

        public static void WakeTheFuckUpSamurai( ReferenceHub player, List<Inventory.SyncItemInfo> items, Vector3 pos) {
            player.plyMovementSync.OverridePosition(pos, 0f, false);
            tranquilized.Remove(player);
            player.SetInventory(items);
            EventPlugin.GhostedIds.Remove(player.queryProcessor.PlayerId);
            foreach(Ragdoll doll in Object.FindObjectsOfType<Ragdoll>()) {
                if(doll.owner.ownerHLAPI_id == player.GetNickname()) {
                    NetworkServer.Destroy(doll.gameObject);
                }
            }
            if(Map.IsNukeDetonated) {
                if(player.GetCurrentRoom().Zone != EXILED.ApiObjects.ZoneType.Surface) player.Kill();
                else foreach(Lift l in Map.Lifts) if(l.elevatorName.ToLower() == "gatea" || l.elevatorName.ToLower() == "gateb")
                        foreach(Lift.Elevator e in l.elevators)
                            if(e.target.name == "ElevatorChamber (1)")
                                if(Vector3.Distance(player.GetPosition(), e.target.position) <= 3.6f) player.Kill();
            }
        }
    }
}
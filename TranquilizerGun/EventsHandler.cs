using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using Mirror;
using PlayableScps;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TranquilizerGun {
    public class EventsHandler {

        private Plugin plugin;
        public List<string> tranquilized, armored;
        public Dictionary<string, int> scpShots;
        bool allArmorEnabled = false;

        public string password = "getagirlfriend";

        public EventsHandler(Plugin plugin) {
            this.plugin = plugin;
            tranquilized = new List<string>();
            armored = new List<string>();
        }

        public void RoundEnd() {
            tranquilized.Clear();
            armored.Clear();
        }

        public void RoundStart() => Timing.RunCoroutine(DelayedReplace());

        /// <inheritdoc cref="OnDied(DiedEventArgs)"/>
        public void HurtEvent(HurtingEventArgs ev) {
            try {
                if(ev.Attacker == null || ev.Attacker == ev.Target ||
                    ev.Attacker.Inventory.curItem != ItemType.GunUSP || ev.Attacker.Inventory.curItem != ItemType.GunCOM15 || TranqConfig.blacklist.Contains(ev.Target.Role))
                    return;
                else if(tranquilized.Contains(ev.Target.UserId)
                    && (ev.DamageType == DamageTypes.Decont || ev.DamageType == DamageTypes.Nuke) && !TranqConfig.usingEffects) {
                    ev.Amount = 0;
                    return;
                } else if(IsTranquilizerDamage(ev.DamageType) && !tranquilized.Contains(ev.Target.UserId)) {
                    string id = ev.Target.UserId;
                    if(TranqConfig.specialRoles.Keys.Contains(ev.Target.Role)) {
                        if(!scpShots.ContainsKey(id))
                            scpShots.Add(id, 0);
                        scpShots[id] += 1;
                        if(scpShots[id] >= TranqConfig.specialRoles[ev.Target.Role]) {
                            Sleep(ev.Target);
                            scpShots[id] = 0;
                        }
                        return;
                    }

                    if(TranqConfig.FriendlyFire && ev.Target.Side == ev.Attacker.Side)
                        return;

                    Sleep(ev.Target);
                }
            } catch(Exception e) {
                e.Print("HurtEvent (TranqHandler)");
            }
        }

        public void OnCommand(SendingRemoteAdminCommandEventArgs ev) {
            try {
                if(ev.Name.Contains("REQUEST_DATA PLAYER_LIST"))
                    return;

                string cmd = ev.Name.ToLower();
                // reload / protect / replaceguns / toggle / sleep / version / setgun / addgun / defaultconfig

                if(cmd.Equals("tg") || cmd.Equals("tgun") || cmd.Equals("tranqgun") || cmd.Equals("tranquilizergun")) {
                    ev.IsAllowed = false;
                    if(ev.Arguments.Count > 1) {
                        switch(ev.Arguments[0].ToLower()) {
                            case "reload":
                            case "reloadconfig":
                                // TODO
                                return;
                            case "protect":
                            case "protection":
                            case "armor":
                                if(ev.Arguments.Count > 2) {
                                    string argument = ev.Arguments[1];
                                    if(argument.ToLower() == "all" || argument == "*") {
                                        int amountArmored = 0;
                                        foreach(Player p in Player.List) {
                                            if(allArmorEnabled && armored.Contains(p.UserId)) {
                                                armored.Remove(p.UserId);
                                                amountArmored++;
                                            } else if(!allArmorEnabled && !armored.Contains(p.UserId)) {
                                                armored.Add(p.UserId);
                                                amountArmored++;
                                            }
                                        }
                                        ev.ReplyMessage = allArmorEnabled ? $"<color=#4ce300>Tranquilizer protection has been disabled for {amountArmored} players.</color>" : $"<color=#4ce300>Tranquilizer protection has been enabled for {amountArmored} players.</color>";
                                        allArmorEnabled = !allArmorEnabled;
                                    } else {
                                        Player p = Player.Get(argument);

                                        if(p == null) {
                                            ev.ReplyMessage = $"<color=red>Couldn't find player <b>{argument}</b>.</color>";
                                            return;
                                        }

                                        ToggleArmor(p, out string newMessage);
                                        ev.ReplyMessage = newMessage;
                                        return;
                                    }
                                } else {
                                    ToggleArmor(ev.Sender, out string newMessage);
                                    ev.ReplyMessage = newMessage;
                                }
                                return;
                            case "replaceguns":
                                int a = 0;
                                foreach(Pickup item in Object.FindObjectsOfType<Pickup>()) {
                                    if(item.ItemId == ItemType.GunCOM15 && UnityEngine.Random.Range(1, 100) <= TranqConfig.replaceChance) {
                                        item.ItemId = TranqConfig.tranquilizer;
                                        item.RefreshDurability(true, true);
                                        a++;
                                    }
                                }
                                ev.ReplyMessage = $"<color=#4ce300>A total of {a} COM-15 pistols have been replaced.</color>";
                                return;
                            case "sleep":
                                if(ev.Arguments.Count > 2) {
                                    string argument = ev.Arguments[1];
                                    if(argument.ToLower() == "all" || argument == "*") {
                                        int amountSleeping = 0;
                                        foreach(Player p in Player.List) {
                                            if(p.Side == Side.None && !tranquilized.Contains(p.UserId)) {
                                                Sleep(p);
                                                amountSleeping++;
                                            }
                                        }
                                        ev.ReplyMessage = $"<color=#4ce300>A total of {amountSleeping} players have been put to sleep.</color>";
                                    } else {
                                        Player p = Player.Get(argument);

                                        if(p == null) {
                                            ev.ReplyMessage = $"<color=red>Couldn't find player <b>{argument}</b>.</color>";
                                            return;
                                        } else if(tranquilized.Contains(p.UserId)) {
                                            ev.ReplyMessage = "<color=red>You're already sleeping...?</color>";
                                            return;
                                        }

                                        ev.ReplyMessage = $"<color=#4ce300>{p.Nickname} has been forced to sleep. Tell him sweet dreams!</color>";
                                        return;
                                    }
                                } else {
                                    Sleep(ev.Sender);
                                    ev.ReplyMessage = $"<color=#4ce300>You've been forced to sleep. Sweet dreams!</color>";
                                }
                                return;
                            case "version":
                                ev.ReplyMessage = "You're currently using " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                                return;
                            case "receivegun":
                            case "addgun":
                            case "givegun":
                                if(ev.Arguments.Count > 2) {
                                    string argument = ev.Arguments[1];
                                    if(argument.ToLower() == "all" || argument == "*") {
                                        int amountGiven = 0;
                                        foreach(Player p in Player.List) {
                                            if(p.Side == Side.None) {
                                                ev.Sender.AddItem(Extensions.GetTranquilizerItem());
                                                amountGiven++;
                                            }
                                        }
                                        ev.ReplyMessage = $"<color=#4ce300>A total of {amountGiven} players received Tranquilizers.</color>";
                                    } else {
                                        Player p = Player.Get(argument);

                                        if(p == null) {
                                            ev.ReplyMessage = $"<color=red>Couldn't find player <b>{argument}</b>.</color>";
                                            return;
                                        }

                                        ev.Sender.AddItem(Extensions.GetTranquilizerItem());
                                        ev.ReplyMessage = $"<color=#4ce300>{p.Nickname} received a Tranquilizer.</color>";
                                        return;
                                    }
                                } else {
                                    ev.Sender.AddItem(Extensions.GetTranquilizerItem());
                                    ev.ReplyMessage = $"<color=#4ce300>Enjoy your Tranquilizer!</color>";
                                }
                                return;
                            case "defaultconfig":
                            case "resetconfig":
                                // TODO
                                return;
                            case "toggle":
                                if(TranqConfig.IsEnabled) {
                                    plugin.UnregisterEvents();
                                    TranqConfig.IsEnabled = false;
                                    ev.ReplyMessage = $"<color=#4ce300>The plugin has now been disabled!</color>";
                                } else {
                                    plugin.RegisterEvents();
                                    TranqConfig.IsEnabled = true;
                                    ev.ReplyMessage = $"<color=#4ce300>The plugin has now been enabled!</color>";
                                }
                                return;
                        }

                        ev.ReplyMessage = 
                            $"\n<color=#4ce300>--- [ TranqGun Help ] ---</color>" +
                            $"\n<color=#006eff>Reload:</color> <color=#f7ff9c>Reloads the configuration variables. (Not the same as \"resetconfig\")</color>" +
                            $"\n<color=#006eff>Protection:</color> <color=#f7ff9c>Grants you special protection against Tranquilizers.</color>" +
                            $"\n<color=#006eff>ReplaceGuns:</color> <color=#f7ff9c>Replaces any COM-15s with Tranquilizers.</color>" +
                            $"\n<color=#006eff>Sleep:</color> <color=#f7ff9c>Forces the sleep method on someone.</color>" +
                            $"\n<color=#006eff>Setgun:</color> <color=#f7ff9c>The gun you're holding will now be the Tranquilizer.</color>" +
                            $"\n<color=#006eff>AddGun:</color> <color=#f7ff9c>Add a Tranquilizer to your inventory.</color>" +
                            $"\n<color=#006eff>ResetConfig:</color> <color=#f7ff9c>Resets the configuration variables to their default ones. (Not the same as \"reload\")</color>" +
                            $"\n<color=#006eff>Toggle:</color> <color=#f7ff9c>Toggles the plugin's features on/off.</color>" +
                            $"\n<color=#006eff>Version:</color> <color=#f7ff9c>Check the installed version of this plugin.</color>";
                    }
                }
            } catch(Exception e) {
                e.Print("OnCommand");
            }
        }

        public void Sleep(Player player) {
            try {
                // Initialize variables & add player to list
                Vector3 oldPos = player.Position;
                PlayerEffectsController controller = player.ReferenceHub.playerEffectsController;
                tranquilized.Add(player.Nickname);
                float sleepDuration = UnityEngine.Random.Range(TranqConfig.sleepDurationMin, TranqConfig.sleepDurationMax);

                // Broadcast message (if enabled)
                if(TranqConfig.tranquilizedBroadcastDuration > 0) {
                    if(TranqConfig.clearBroadcasts)
                        player.ClearBroadcasts();
                    player.Broadcast(TranqConfig.tranquilizedBroadcastDuration, TranqConfig.tranquilizedBroadcast, Broadcast.BroadcastFlags.Normal);
                }

                if(TranqConfig.dropItems)
                    player.Inventory.ServerDropAll();

                if(TranqConfig.usingEffects) {
                    EnableEffects(controller);
                } else {
                    // Spawn a Ragdoll
                    PlayerStats.HitInfo hitInfo = new PlayerStats.HitInfo(1000f, player.UserId, DamageTypes.Falldown, player.Id);
                    
                    player.GameObject.GetComponent<RagdollManager>().SpawnRagdoll(
                        oldPos, player.GameObject.transform.localRotation, Vector3.zero,
                        (int) player.Role, hitInfo, false, player.Nickname, player.Nickname, 0);

                    // Apply effects
                    controller.EnableEffect<Amnesia>(sleepDuration);
                    controller.EnableEffect<Scp268>(sleepDuration);
                }

                if(TranqConfig.teleportAway)
                    player.Position = TranqConfig.newPos;

                Timing.CallDelayed(sleepDuration, () => Wake(player, oldPos));

            } catch(Exception e) {
                e.Print("Sleeping " + player.Nickname);
            }
        }

        public void Wake(Player player, Vector3 oldPos) {
            try {
                tranquilized.Remove(player.UserId);

                foreach(Ragdoll doll in Object.FindObjectsOfType<Ragdoll>()) {
                    if(doll.owner.ownerHLAPI_id == player.Nickname) {
                        NetworkServer.Destroy(doll.gameObject);
                    }
                }

                player.ReferenceHub.GetComponent<Scp096>().

                if(TranqConfig.teleportAway) {
                    player.Position = oldPos;

                    if(Warhead.IsDetonated) {
                        if(player.CurrentRoom.Zone != ZoneType.Entrance)
                            player.Kill();
                        else
                            foreach(Lift l in Map.Lifts)
                                if(l.elevatorName.ToLower() == "gatea" || l.elevatorName.ToLower() == "gateb")
                                    foreach(Lift.Elevator e in l.elevators)
                                        if(e.target.name == "ElevatorChamber (1)")
                                            if(Vector3.Distance(player.Position, e.target.position) <= 3.6f)
                                                player.Kill();
                    }
                }
            } catch(Exception e) {
                e.Print("Sleeping " + player.Nickname);
            }
        }

        public void EnableEffects(PlayerEffectsController controller) {
            if(TranqConfig.amnesia) {
                controller.EnableEffect<Amnesia>(TranqConfig.amnesiaDuration);
            }

            if(TranqConfig.asphyxiated) {
                controller.EnableEffect<Asphyxiated>(TranqConfig.asphyxiatedDuration);
            }

            if(TranqConfig.blinded) {
                controller.EnableEffect<Blinded>(TranqConfig.blindedDuration);
            }

            if(TranqConfig.concussed) {
                controller.EnableEffect<Concussed>(TranqConfig.concussedDuration);
            }

            if(TranqConfig.deafened) {
                controller.EnableEffect<Deafened>(TranqConfig.deafenedDuration);
            }

            if(TranqConfig.disabled) {
                controller.EnableEffect<Disabled>(TranqConfig.disabledDuration);
            }

            if(TranqConfig.ensnared) {
                controller.EnableEffect<Ensnared>(TranqConfig.ensnaredDuration);
            }

            if(TranqConfig.exhausted) {
                controller.EnableEffect<Exhausted>(TranqConfig.exhaustedDuration);
            }

            if(TranqConfig.flash) {
                controller.EnableEffect<Flashed>(TranqConfig.flashDuration);
            }

            if(TranqConfig.poisoned) {
                controller.EnableEffect<Poisoned>(TranqConfig.poisonedDuration);
            }
        }

        public bool IsTranquilizerDamage(DamageTypes.DamageType damageType) {
            if(TranqConfig.useBothPistols) {
                return damageType == DamageTypes.Usp || damageType == DamageTypes.Com15;
            } else {
                return (damageType == DamageTypes.Usp && TranqConfig.tranquilizer == ItemType.GunUSP) || (damageType == DamageTypes.Com15 && TranqConfig.tranquilizer == ItemType.GunCOM15);
            }
        }

        public IEnumerator<float> DelayedReplace() {
            yield return Timing.WaitForSeconds(2f);
            foreach(Pickup item in Object.FindObjectsOfType<Pickup>()) {
                if(item.ItemId == ItemType.GunCOM15 && UnityEngine.Random.Range(1, 100) <= TranqConfig.replaceChance) {
                    item.ItemId = TranqConfig.tranquilizer;
                    item.RefreshDurability(true, true);
                }
            }
        }

        // I'm fucking lazy 
        private void ToggleArmor(Player p, out string ReplyMessage) {
            if(armored.Contains(p.UserId)) {
                armored.Remove(p.UserId);
                ReplyMessage = $"<color=red>{p.Nickname} is no longer protected against Tranquilizers.</color>";
            } else {
                armored.Add(p.UserId);
                ReplyMessage = $"<color=#4ce300>{p.Nickname} is now protected against Tranquilizers.</color>";
            }
        }
    }
}

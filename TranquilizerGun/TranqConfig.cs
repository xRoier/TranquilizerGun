using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Loader;
using UnityEngine;

namespace TranquilizerGun {
    public class TranqConfig : IConfig {

        public static ItemType tranquilizer;
        public static bool IsEnabled, clearBroadcasts, useBothPistols, usingEffects, teleportAway, dropItems, replaceCom;
        public static Vector3 newPos;

        public static ushort tranquilizedBroadcastDuration;
        public static string tranquilizedBroadcast;

        public static ushort pickedUpBroadcastDuration;
        public static string pickedUpBroadcast;

        public static string shotsLeftHintText;

        public static float sleepDurationMax, sleepDurationMin;
        public static ushort ScpShotsNeeded;
        public static bool FriendlyFire;
        public static ushort replaceChance;

        public static bool doBlacklist;
        public static List<RoleType> blacklist;
        public static bool doSpecialRoles;
        public static Dictionary<RoleType, ushort> specialRoles;

        #region PlayerEffects
        public static bool amnesia;
        public static float amnesiaDuration;

        public static bool disabled;
        public static float disabledDuration;

        public static bool flash;
        public static float flashDuration;

        public static bool blinded;
        public static float blindedDuration;

        public static bool concussed;
        public static float concussedDuration;
                
        public static bool deafened;
        public static float deafenedDuration;
                
        public static bool ensnared;
        public static float ensnaredDuration;
                
        public static bool poisoned;
        public static float poisonedDuration;
                
        public static bool asphyxiated;
        public static float asphyxiatedDuration;
                
        public static bool exhausted;
        public static float exhaustedDuration;
        #endregion

        public string Prefix => "tranquilizergun";

        bool IConfig.IsEnabled { get; set; }

        public void Reload() {
            tranquilizer = GetTranquilizerGun();
            IsEnabled = PluginManager.YamlConfig.GetBool($"{Prefix}.enabled", true);
            clearBroadcasts = PluginManager.YamlConfig.GetBool($"{Prefix}.broadcast.clearbroadcasts", true);
            useBothPistols = PluginManager.YamlConfig.GetBool($"{Prefix}.usebothpistols", true);
            usingEffects = PluginManager.YamlConfig.GetBool($"{Prefix}.usingeffects", true);
            teleportAway = PluginManager.YamlConfig.GetBool($"{Prefix}.teleportplayer", true);
            dropItems = PluginManager.YamlConfig.GetBool($"{Prefix}.playersdropitems", true);
            newPos = new Vector3(PluginManager.YamlConfig.GetFloat($"{Prefix}.newposition.x"), PluginManager.YamlConfig.GetFloat($"{Prefix}.newposition.y"), PluginManager.YamlConfig.GetFloat($"{Prefix}.newposition.z"));
            tranquilizedBroadcastDuration = PluginManager.YamlConfig.GetUShort($"{Prefix}.broadcast.sleep.duration", 3);
            tranquilizedBroadcast = PluginManager.YamlConfig.GetString($"{Prefix}.broadcast.sleep.text", "<color=red>You fell asleep...</color>");
            tranquilizedBroadcastDuration = PluginManager.YamlConfig.GetUShort($"{Prefix}.broadcast.pickup.duration", 3);
            tranquilizedBroadcast = PluginManager.YamlConfig.GetString($"{Prefix}.broadcast.pickup.text", "<color=green><b>You picked up a tranquilizer gun!</b></color> \nEvery shot uses %ammo ammo, so count your bullets!");
            tranquilizedBroadcast = PluginManager.YamlConfig.GetString($"{Prefix}.ammo.text", "<color=red></color>");
            sleepDurationMin = PluginManager.YamlConfig.GetFloat($"{Prefix}.sleep.duration.min", 3);
            sleepDurationMax = PluginManager.YamlConfig.GetFloat($"{Prefix}.sleep.duration.max", 5);
            FriendlyFire = PluginManager.YamlConfig.GetBool($"{Prefix}.friendlyfire", true);
            replaceCom = PluginManager.YamlConfig.GetBool($"{Prefix}.replacecompistol.enabled", true);
            replaceChance = PluginManager.YamlConfig.GetUShort($"{Prefix}.replacecompistol.chance", 75);
            doBlacklist = PluginManager.YamlConfig.GetBool($"{Prefix}.blacklist.enabled", true);
            blacklist = BlacklistedRoles();
            doSpecialRoles = PluginManager.YamlConfig.GetBool($"{Prefix}.special.enabled", true);
            specialRoles = SpecialRoles();
            amnesia = PluginManager.YamlConfig.GetBool($"{Prefix}.effects.amnesia.enabled", true);
            amnesiaDuration = PluginManager.YamlConfig.GetFloat($"{Prefix}.effects.amnesia.duration", 3);
            disabled = PluginManager.YamlConfig.GetBool($"{Prefix}.effects.disabled.enabled", true);
            disabledDuration = PluginManager.YamlConfig.GetFloat($"{Prefix}.effects.disabled.duration", 3);
            flash = PluginManager.YamlConfig.GetBool($"{Prefix}.effects.flash.enabled", true);
            flashDuration = PluginManager.YamlConfig.GetFloat($"{Prefix}.effects.flash.duration", 3);
            blinded = PluginManager.YamlConfig.GetBool($"{Prefix}.effects.blinded.enabled", true);
            blindedDuration = PluginManager.YamlConfig.GetFloat($"{Prefix}.effects.blinded.duration", 3);
            concussed = PluginManager.YamlConfig.GetBool($"{Prefix}.effects.concussed.enabled", true);
            concussedDuration = PluginManager.YamlConfig.GetFloat($"{Prefix}.effects.concussed.duration", 3);
            deafened = PluginManager.YamlConfig.GetBool($"{Prefix}.effects.deafened.enabled", true);
            deafenedDuration = PluginManager.YamlConfig.GetFloat($"{Prefix}.effects.deafened.duration", 3);
            ensnared = PluginManager.YamlConfig.GetBool($"{Prefix}.effects.esnared.enabled", true);
            ensnaredDuration = PluginManager.YamlConfig.GetFloat($"{Prefix}.effects.esnared.duration", 3);
            poisoned = PluginManager.YamlConfig.GetBool($"{Prefix}.effects.poisoned.enabled", true);
            poisonedDuration = PluginManager.YamlConfig.GetFloat($"{Prefix}.effects.poisoned.duration", 3);
            asphyxiated = PluginManager.YamlConfig.GetBool($"{Prefix}.effects.asphyxiated.enabled", true);
            asphyxiatedDuration = PluginManager.YamlConfig.GetFloat($"{Prefix}.effects.asphyxiated.duration", 3);
            exhausted = PluginManager.YamlConfig.GetBool($"{Prefix}.effects.exhausted.enabled", true);
            exhaustedDuration = PluginManager.YamlConfig.GetFloat($"{Prefix}.effects.exhausted.duration", 3);
        }

        #region Suicide
        //public void Restart() {
        //    PluginManager.YamlConfig.SetString($"{Prefix}.gun", "GunUSP");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.enabled", "true");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.broadcast.clearbroadcasts", "true");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.usebothpistols", "true");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.usingeffects", "true");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.teleportplayer", "true");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.playersdropitems", "true");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.newposition.x");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.newposition.y");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.newposition.z");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.broadcast.sleep.duration", "3");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.broadcast.sleep.text", "<color=red>You fell asleep...</color>");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.sleep.duration.min", "3");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.sleep.duration.max", "5");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.friendlyfire", "true");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.replacecompistol.enabled", "true");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.replacecompistol.chance", "75");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.blacklist.enabled", "true");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.blacklist.roles", "Scp173, Scp106");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.special.enabled", "true");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.special.roles", "Scp173:3, Scp106:2");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.effects.amnesia.enabled", "true");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.effects.amnesia.duration", "3");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.effects.disabled.enabled", "true");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.effects.disabled.duration", "3");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.effects.flash.enabled", "true");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.effects.flash.duration", "3");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.effects.blinded.enabled", "true");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.effects.blinded.duration", "3");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.effects.concussed.enabled", "true");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.effects.concussed.duration", "3");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.effects.deafened.enabled", "true");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.effects.deafened.duration", "3");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.effects.esnared.enabled", "true");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.effects.esnared.duration", "3");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.effects.poisoned.enabled", "true");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.effects.poisoned.duration", "3");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.effects.asphyxiated.enabled", "true");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.effects.asphyxiated.duration", "3");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.effects.exhausted.enabled", "true");
        //    PluginManager.YamlConfig.SetString($"{Prefix}.effects.exhausted.duration", "3");
        //}
        #endregion

        public ItemType GetTranquilizerGun() {
            if(Enum.TryParse($"{Prefix}.gun", true, out ItemType type))
                return type;
            else
                Log.Error("Couldn't parse TranqGun.");
            return ItemType.GunUSP;
        }

        public List<RoleType> BlacklistedRoles() {
            List<RoleType> l = new List<RoleType>();
            if(doBlacklist) {
                try {
                    string[] bl = Regex.Replace(PluginManager.YamlConfig.GetString($"{Prefix}.blacklist.roles", "Scp173, Scp106"), @"\s+", "").Split(',');
                    foreach(string r in bl) {
                        if(Enum.TryParse(r, true, out RoleType role)) {
                            l.Add(role);
                        } else
                            Log.Error($"Couldn't parse role: {r}.");
                    }
                } catch(Exception e) {
                    e.Print("Loading Blacklisted Roles");
                }
            }
            return l;
        }

        public Dictionary<RoleType, ushort> SpecialRoles() {
            Dictionary<RoleType, ushort> l = new Dictionary<RoleType, ushort>();
            if(doSpecialRoles) {
                try {
                    string[] specialRoles = Regex.Replace(PluginManager.YamlConfig.GetString($"{Prefix}.special.roles", "Scp173:3, Scp106:2"), @"\s+", "").Split(',');
                    foreach(string o in specialRoles) {
                        string[] option = Regex.Replace(o, @"\s+", "").Split(':');
                        if(Enum.TryParse(option[0], true, out RoleType role) && ushort.TryParse(option[1], out ushort shots)) {
                            l.Add(role, shots);
                        } else Log.Error($"Couldn't load {o}.");
                    }
                } catch(Exception e) {
                    e.Print("Loading Special Roles");
                }
            }
            return l;
        }

    }
}

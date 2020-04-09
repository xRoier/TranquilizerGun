using System;
using EXILED;

namespace TranquilizerGun {
    public class Plugin : EXILED.Plugin {

        public EventHandlers handlers;

        #region Config Variables
        public bool enabled;

        public string accessDenied;
        public string warningText;
        public uint warningTime;

        public uint noAmmoDuration;
        public string noAmmoText;
        public uint youPickedupDuration;
        public string youPickedupText;

        public string weapon;
        public float sleepDurationMin;
        public float sleepDurationMax;
        public int tranqAmmo;
        public int tranqDamage;

        public int ScpShotsNeeded;
        public bool clearBroadcasts;
        public bool replaceComGun;
        public bool requiresPermission;
        public bool blacklist173;
        public bool doStun;
        #endregion

        public override void OnEnable() {
            try {
                Log.Debug("Tranquilizer Gun plugin detected, loading configuration file...");
                ReloadConfig();
                Log.Debug("Initializing EventHandlers...");
                handlers = new EventHandlers(this);

                Events.RemoteAdminCommandEvent += handlers.OnCommand;
                enabled = Plugin.Config.GetBool("tgun_enable", true);

                if(enabled)
                    StartEvents();
                Log.Info("Plugin loaded correctly!");
            } catch(Exception e) {
                Log.Error("Problem loading plugin: " + e.StackTrace);
            }
        }

        public override void OnDisable() {
            if(enabled)
                StopEvents();
            Events.RemoteAdminCommandEvent -= handlers.OnCommand;

            handlers = null;
        }

        public override void OnReload() {
        }

        public void StartEvents() {
            Events.ShootEvent += handlers.OnShootEvent;
            Events.PickupItemEvent += handlers.OnPickupEvent;
            Events.RoundStartEvent += handlers.OnRoundStart;
            Events.PlayerHurtEvent += handlers.OnPlayerHurt;
        }

        public void StopEvents() {
            Events.ShootEvent -= handlers.OnShootEvent;
            Events.PickupItemEvent -= handlers.OnPickupEvent;
            Events.RoundStartEvent -= handlers.OnRoundStart;
            Events.PlayerHurtEvent -= handlers.OnPlayerHurt;
        }

        public void ReloadConfig() {
            Config.Reload();

            #region TGun Config

            // Text

            weapon = Config.GetString("tgun_weapon", "GunUSP");
            ScpShotsNeeded = Config.GetInt("tgun_scp_shotsneeded", 2);
            tranqAmmo = Config.GetInt("tgun_ammo", 18) - 1;
            tranqDamage = Config.GetInt("tgun_damage", 1);
            sleepDurationMin = Config.GetFloat("tgun_sleepduration_min", 3f);
            sleepDurationMax = Config.GetFloat("tgun_sleepduration_max", 5f);
            doStun = Config.GetBool("tgun_stun", false);
            replaceComGun = Config.GetBool("tgun_replacecomgun", true);

            accessDenied = Config.GetString("tgun_accessdenied", "<color=red>Access denied.</color>");
            warningText = Config.GetString("tgun_warning_text", "<color=red>You fell asleep...</color>");
            warningTime = Config.GetUInt("tgun_warning_duration", 3);
            youPickedupDuration = Config.GetUInt("tgun_pickedup_duration", 2);
            youPickedupText = Config.GetString("tgun_pickedup_text", "<color=green><b>You picked up a tranquilizer gun!</b></color> \nEvery shot uses %ammo ammo, so count your bullets!").Replace("%ammo", $"{tranqAmmo + 1}");
            noAmmoDuration = Config.GetUInt("tgun_noammo_duration", 2);
            noAmmoText = Config.GetString("tgun_noammo_text", "<color=red>You need %ammo ammo to fire your gun!</color>").Replace("%ammo", $"{tranqAmmo + 1}");

            clearBroadcasts = Config.GetBool("tgun_clearbroadcasts", true);
            requiresPermission = Config.GetBool("tgun_usespermission", false);
            blacklist173 = Config.GetBool("tgun_blacklist_173", true);
            #endregion
        }

        public override string getName { get; } = "TranquilizerGun - Uriel & SebasCapo";
    }
}
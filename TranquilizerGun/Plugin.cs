using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events.Handlers;
using Exiled.Loader;
using UnityEngine;

namespace TranquilizerGun {
    public class Plugin : Exiled.API.Features.Plugin {

        public override IConfig Config { get; } = new TranqConfig();

        public EventsHandler handler;

        public override void OnEnabled() {
            handler = new EventsHandler(this);

            RegisterEvents();
            Exiled.Events.Handlers.Server.SendingRemoteAdminCommand += handler.OnCommand;

            Log.Info($"{Name} has been enabled!");
        }

        public override void OnDisabled() {
            Exiled.Events.Handlers.Server.SendingRemoteAdminCommand -= handler.OnCommand;
            UnregisterEvents();

            handler = null;
            Log.Info($"{Name} has been disabled!");
        }

        public override void OnReloaded() => Log.Info($"{Name} has been reloaded!");

        public void RegisterEvents() {
            Exiled.Events.Handlers.Player.Hurting += handler.HurtEvent;
            Exiled.Events.Handlers.Server.RoundStarted += handler.RoundStart;
        }

        public void UnregisterEvents() {
            Exiled.Events.Handlers.Player.Hurting -= handler.HurtEvent;
            Exiled.Events.Handlers.Server.RoundStarted -= handler.RoundStart;
        }

    }
}

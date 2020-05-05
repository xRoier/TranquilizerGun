# Tranquilizer Gun
Remade from the ground, here it's the long awaited Tranquilizer Gun plugin! Allowing you to sleep both players and SCPs (Highly configurable).

### What does it do?
This plugin lets you put people to sleep using a pistol randomly found in LCZ & HCZ, one shot uses all your ammo and you will need to shoot SCPs twice to put them to sleep (Everything here is configurable, even the weapon!).

### Installation
As with any EXILED plugin, you must place the TranquilizerGun.dll file inside of your "%appdata%/Roaming/Plugins" folder.

And... obviously include [EXILED](https://github.com/galaxy119/EXILED "EXILED").

### Commands
Arguments inside &lt;&gt; are required. [] means it's optional.
| Command | Description | Arguments |
| ------------- | ------------------------------ | -------------------- |
| `tg`   | Plugin's main command, sends info. | **protect/toggle/reload/replaceguns**|
- Toggle: Toggles all of the plugin's functions.
- Reload: Reloads every configuration variable.
- ReplaceGuns: Forces the replace of all COM15 with "tgun_weapon". (Config variable)
- Protect: Protection against "T-Guns" and Sleep command. (Good for administrators!)
- Sleep: Force Sleep on someone.
- SetGun: Sets the configuration value of `tgun_weapon`.
- AddGun: Gives you whatever item is set inside `tgun_weapon`.
- DefaultConfig: Sets every configuration value to their default one. (It will only change this plugin's variables)
- Version: Shows you what version of this plugin you're using.

### Configuration
These are the variables that should be added to your 7777-config.yml. Or simply download/copy&paste the [config-file example](https://github.com/cerberusServers/TranquilizerGun/blob/master/Examples/7777-config.yml)

**THIS IS NOT OBLIGATORY, YOU CAN JUST PLUG THE DLL AND IT WILL WORK WITHOUT ANY CONFIGURATION WHATSOEVER**
| Variable  | Description | Default value |
| ------------- | ------------- | ------------- |
| tgun_enable | Enables or disables each and every one of the plugin's functionalities | `true` |
| tgun_weapon | Changes what the plugin considers as a tranquilizer gun | `GunUSP` |
| tgun_ammo | How much ammunition does one shot take? | `18` |
| tgun_damage | Damage dealt to the player by the Tranquilizer Gun | `1` |
| tgun_sleepduration_min | Minimum duration time someone can fall asleep for | `3.0` |
| tgun_sleepduration_max | Minimum duration time someone can fall asleep for | `5.0` |
| tgun_usespermission | Do tranquilizer guns require permissions? | `false` |
| ~~tgun_blacklist_173~~ | ~~Setting this to true makes it so SCP-173 can't be put to sleep (Personally not recommended)~~ | ~~`true`~~ |
| tgun_blacklist_toggle | Setting this to true will enable the Blacklist system | `false` |
| tgun_blacklist | Add whatever Roles who shouldn't be slept using the Tranquilizer Gun | `Scp173, Scp106` |
| tgun_replacecomgun | After the round starts, all pistols are replaced with "tgun_weapon" | `true` |
| tgun_replacechance | After the round starts, all pistols have X chance to be replaced with "tgun_weapon" (100% Chance by default) | `100` |
| tgun_accessdenied | Message displayed when access to command is denied | `<color=red>Access denied.</color>` |
| tgun_warning_duration | Duration of message displayed when someone falls asleep **(Use 0 to disable)** | `3` |
| tgun_warning_text | Message displayed when someone falls asleep | `<color=red>You fell asleep...</color>` |
| tgun_pickedup_duration | Duration of message displayed when picks up the tranq. gun **(Use 0 to disable)** | `2` |
| tgun_pickedup_text | Message displayed when someone picks up the tranq. gun | `<color=green><b>You picked up a tranquilizer gun!</b></color> \nEvery shot uses %ammo ammo, so count your bullets!` |
| tgun_noammo_duration | Duration of message displayed when whoever holds the tgun is out of ammo **(Use 0 to disable)** | `1` |
| tgun_noammo_text | Message displayed when whoever holds the tgun is out of ammo  | `<color=red>You need %ammo ammo to fire your gun!</color>` |
| tgun_clearbroadcasts | Whether or not to clear previous broadcasts when displaying one (This will avoid making some texts last a lot) | `true` |
| tgun_friendlyfire | Are players able to sleep their teammates? | `true` |

### Permissions
These are the permissions that should be added to your permissions.yml inside your "%appdata%/Roaming/Plugins/Exiled Permissions" folder.
| Permission  | This permission belongs to |
| ------------- | ------------- |
| tgun.tg | `tg` and it's arguments | 
| tgun.protect | `tg protect` | 
| tgun.toggle | `tg toggle` | 
| tgun.replaceguns | `tg replaceguns` |
| tgun.sleep | `tg sleep` |
| tgun.setgun | `tg setgun` |
| tgun.addgun | `tg addgun` |
| tgun.defaultconfig | `tg defaultconfig` |
| tgun.reload | `tg reload` | 
| tgun.use | `Shooting the TGun (If tgun_usespermission is set to true)` | 
| tgun.* | `All above` | 

### Planned Changes:
- Add more configurable options. (Open to suggestions)
- Changing the "sleeping" system. (Mostly waiting for new SCP:SL patch to hit for effects like slowing someone, stunning them, blurry their vision and stuff like that!)
- Make every config setting changeable in-game (Or at least most of them, same way it works with [Lights](https://github.com/SebasCapo/Lights))
- ~~SCPs Blacklist. (Configurable too)~~

### F.A.Q.:
- **Using older versions of EXILED make this plugin not work:**
*This will not be fixed, but I still get people asking why this plugin doesn't work when they don't have EXILED up to date, so if this plugin doesn't work, try updating to the recommended EXILED version stated in the downloads tab.*

- **The message above says I don't have any bullets but my gun seems like it's shooting, is this a bug?**
*Sadly, most (if not all) animations can't be disabled since they're client-side, everything you see made in EXILED is only server-side, even changing people's sizes is server-side. I still haven't found a way to do it, but once I do, don't worry that I'll immediately upload a fix. For the meantime, don't worry, even if it looks like you're shooting, you're not, **IT DOES NOT MAKE YOU SHOOT MORE THAN YOU SHOULD!** (This can be kinda avoided by changing `tgun_ammo` for a value that will leave you at 0 ammo once you made all your shots)*

- **Sorry, I don't know how to configure all of this, should I place it inside config_gameplay.txt?**
*No, as with most plugins, you need to check your YOURPORTHERE-config.yml file (It's on the same folder as your Plugins folder, NOT INSIDE of it). Config settings won't appear on their own, (Unless you typed the `tgun defaultconfig` command) so you should each config setting shown here... Or be a smart guy and copy-paste our [config example!](https://github.com/cerberusServers/TranquilizerGun/blob/master/Examples/7777-config.yml) (Note: Anything you don't have in your config-file, will automatically use their default value)*

- **How do I download?**
*There's a **releases** tab that's above the green `Clone or download` button. Or... just press [here!](https://github.com/cerberusServers/TranquilizerGun/releases)*

### That'd be all
Thanks for passing by, have a nice day! :)

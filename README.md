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
| `tg`   | Plugin's main command, sends info. | **protect/toggle/reload/replaceguns/sleep/version**|
- Toggle: Toggles all of the plugin's functions.
- Reload: Reloads every configuration variable.
- ReplaceGuns: Forces the replace of all COM15 with "tgun_weapon". (Config variable).
- Protect: Protection against "T-Guns". (Good for administrators!)
- Sleep: Force the sleeping method on someone. (Or everyone...)
- Version: Get plugin's current version.
- Setgun: Sets the "tgun_weapon" variable to "whatever" you have in your hand.
- Addgun: Adds "whatever" you have in your "tgun_weapon" variable to your inventory.

### Configuration
These are the variables that should be added to your 7777-config.yml. Or simply download/copy&paste the [config-file example](https://github.com/ImUrX/TranquilizerGun/blob/master/Examples/7777-config.yml)

**THIS IS NOT OBLIGATORY, YOU CAN JUST PLUG THE DLL AND IT WILL WORK WITHOUT ANY CONFIGURATION WHATSOEVER**
| Variable  | Description | Default value |
| ------------- | ------------- | ------------- |
| tgun_enable | Enables or disables each and every one of the plugin's functionalities | `true` |
| tgun_weapon | Changes what the plugin considers as a tranquilizer gun | `GunUSP` |
| tgun_ammo | How much ammunition does one shot take? | `18` |
| tgun_damage | Damage dealt to the player by the Tranquilizer Gun | `1` |
| tgun_sleepduration_min | Minimum duration time someone can fall asleep for | `3.0` |
| tgun_sleepduration_max | Maximum duration time someone can fall asleep for | `5.0` |
| tgun_usespermission | Do tranquilizer guns require permissions? | `false` |
| tgun_scp_shotsneeded | How many shots does a SCP need to take before being tranquilized **(Use 1 or less to disable)** | `2` |
| tgun_blacklist_173 | Setting this to true makes it so SCP-173 can't be put to sleep (Personally recommended) | `true` |
| tgun_replacecomgun | After the round starts, all pistols are replaced with "tgun_weapon" | `true` |
| tgun_accessdenied | Message displayed when access to command is denied | `<color=red>Access denied.</color>` |
| tgun_warning_duration | Duration of message displayed when someone falls asleep **(Use 0 or less to disable)** | `3` |
| tgun_warning_text | Message displayed when someone falls asleep | `<color=red>You fell asleep...</color>` |
| tgun_pickedup_duration | Duration of message displayed when picks up the tranq. gun **(Use 0 or less to disable)** | `2` |
| tgun_pickedup_text | Message displayed when someone picks up the tranq. gun | `<color=green><b>You picked up a tranquilizer gun!</b></color> \nEvery shot uses %ammo ammo, so count your bullets!` |
| tgun_noammo_duration | Duration of message displayed when whoever holds the tgun is out of ammo **(Use 0 or less to disable)** | `1` |
| tgun_noammo_text | Message displayed when whoever holds the tgun is out of ammo  | `<color=red>You need %ammo ammo to fire your gun!</color>` |
| tgun_clearbroadcasts | ***NEW** Everytime a messaged is broadcasted to the player, it'll clear all previous broadcasts | `true` |

### Permissions
These are the permissions that should be added to your permissions.yml inside your "%appdata%/Roaming/Plugins/Exiled Permissions" folder.
| Permission  | Action |
| ------------- | ------------- |
| tgun.tg | `Tranquilizer Gun's main command` | 
| tgun.toggle | `Let's you toggle the whole game functionalities!` | 
| tgun.protect | `Let's you toggle your protection against people using tranq. guns!` | 
| tgun.sleep | `Applies the "Sleep" method on one or all players` | 
| tgun.replaceguns | `Forces a replacement of all COM15 pistols to whatever you set "tgun_weapon" to` | 
| tgun.reload | `Reloads every config variable available!` | 
| tgun.setgun | `Sets the "tgun_weapon" variable to "whatever" you have in your hand` |
| tgun.addgun | `Adds "whatever" you have in your "tgun_weapon" variable to your inventory` |
| tgun.use | `If "tgun_usespermission" is set to true, this will let you use your Tranquilizer Gun` |
| tgun.* | `All` | 

### Planned Changes:
- Add more configurable options. (Open to suggestions)
- Changing the "sleeping" system. (Mostly waiting for new SCP:SL patch to hit for more dev-tools)
- SCPs Blacklist. (Configurable too)

### That'd be all
Thanks for passing by, have a nice day! :)

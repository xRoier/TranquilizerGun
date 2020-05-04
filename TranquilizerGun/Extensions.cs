using System;
using System.Collections.Generic;
using System.Linq;

namespace TranquilizerGun {
    public static class Extensions {
        public static void RAMessage( this CommandSender sender, string message, bool success = true ) =>
            sender.RaReply("<color=green>TranquilizerGun</color>#" + message, success, true, string.Empty);

        public static void Broadcast( this ReferenceHub rh, uint time, string message ) => 
            rh.GetComponent<Broadcast>().TargetAddElement(rh.scp079PlayerScript.connectionToClient, message, time, false);

        public static void RemoveWeaponAmmo( this ReferenceHub rh, int amount ) {
            rh.inventory.items.ModifyDuration(
            rh.inventory.items.IndexOf(rh.inventory.GetItemInHand()),
            rh.inventory.GetItemInHand().durability - amount);
        }

        public static void SetWeaponAmmo( this ReferenceHub rh, int amount ) {
            rh.inventory.items.ModifyDuration(
            rh.inventory.items.IndexOf(rh.inventory.GetItemInHand()),
            amount);
        }

        public static bool IsWeapon( this ItemType type ) =>
            (ItemType.GunCOM15 == type || ItemType.GunE11SR == type || ItemType.GunLogicer == type
            || ItemType.GunMP7 == type || ItemType.GunProject90 == type || ItemType.GunUSP == type
            || ItemType.MicroHID == type);

        public static string GeneratePassword() {
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 5)
              .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }

        public static float GenerateRandomNumber( float min, float max ) {
            if(max <= min) return min;
            return UnityEngine.Random.Range(min, max + 1);
        }

        public static int GenerateRandomNumber( int min, int max ) {
            if(max <= min) return min;
            return UnityEngine.Random.Range(min, max + 1);
        }
    }
}
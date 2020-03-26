namespace TranquilizerGun {
    public static class Extensions {
        public static void RAMessage( this CommandSender sender, string message, bool success = true ) =>
            sender.RaReply("<color=green>TranquilizerGun</color>#" + message, success, true, string.Empty);

        public static void Broadcast( this ReferenceHub rh, uint time, string message ) => 
            rh.GetComponent<Broadcast>().TargetAddElement(rh.scp079PlayerScript.connectionToClient, message, time, false);

        public static void SetWeaponAmmo(this ReferenceHub rh, int amount) {
            rh.inventory.items.ModifyDuration(
            rh.inventory.items.IndexOf(rh.inventory.GetItemInHand()),
            rh.inventory.GetItemInHand().durability - amount);
        }

        public static float GenerateRandomNumber( float min, float max ) {
            if(max + 1 <= min) return min;
            return (float) new System.Random().NextDouble() * ((max + 1) - min) + min;
        }
    }
}
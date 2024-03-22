using Unity.VisualScripting;

public enum EquipmentSlots
{
    None,
    SideArm,
    PrimeryWeapon,
    SecondaryWeapon,
    TretiaryWeapon
}

public enum AmmunitionType
{
    None,
    Pistol,
    Shotgun,
    AutoRifle,
    SniperRifle
}

public enum WeaponType
{
    None,
    Pistol,
    Shotgun,
    AutoRifle,
    SniperRifle
}

public enum WeaponFireMode
{
    Single,
    FullAuto
}

public enum AnimationLayers
{
    Pistol = 2,
    Shotgun = 3,
    AutoRifle = 4,
    SniperRifle = 5,
}

public static class Constants
{
    public const string WeaponHolderSocket = "Mannequin/root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r/lowerarm_r/hand_r/SocketWeaponHolder";
    public const string MuzzleSocket = "SocketMuzzle";
    public const string IKTagKey = "LeftHandIK";

    // Animation layer index
    public const int PistolLayer = 2;
    public const int ShotgunLayer = 3;
    public const int AutoRifleLayer = 4;
    public const int SniperLayer = 5;
}

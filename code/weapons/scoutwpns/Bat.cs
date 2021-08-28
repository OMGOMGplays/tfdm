using Sandbox;

[Library("tfdm_bat", Title = "Bat")]

partial class Bat : BaseDmWeapon 
{
    public override string ViewModelPath => "models/weapons/hvywpnswpns/c_heavy_shotgun.vmdl";
	public override float PrimaryRate => 2;
	public override AmmoType AmmoType => AmmoType.Bat;
	public override int ClipSize => 8;
	public override float ReloadTime => 0.5f;
	public override int Bucket => 2;

    public override void Spawn() 
    {
        base.Spawn();

        SetModel("models/weapons/hvywpnswpns/c_heavy_shotgun.vmdl");
    }

    public override void AttackPrimary() 
    {
        
    }
}
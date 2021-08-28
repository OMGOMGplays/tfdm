using Sandbox;

[Library("tfdm_scoutpistol", Title = "Pistol")]
partial class ScoutPistol : BaseDmWeapon 
{
	public override string ViewModelPath => "models/weapons/hvywpnswpns/c_heavy_shotgun.vmdl";
	public override float PrimaryRate => 1;
	public override AmmoType AmmoType => AmmoType.Pistol;
	public override int ClipSize => 6;
	public override float ReloadTime => 0.5f;
	public override int Bucket => 1;

    public override void Spawn() 
    {
        base.Spawn();

        SetModel("models/rust_weapons/rust_pistol.vmdl");
    }

    public override void AttackPrimary() 
    {
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if (!TakeAmmo(1))
		{
			DryFire();
			return;
		}

		(Owner as AnimEntity).SetAnimBool("b_attack", true);

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();
		PlaySound("chicken");

		//
		// Shoot the bullets
		//
		ShootBullet(0.1f, 1.5f, 5.0f, 3.0f);
    }

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create("particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle");
		Particles.Create("particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point");

		if (Owner == Local.Pawn)
		{
			new Sandbox.ScreenShake.Perlin(0.5f, 4.0f, 1.0f, 0.5f);
		}

		ViewModelEntity?.SetAnimBool("fire", true);
		CrosshairPanel?.CreateEvent("fire");
	}

	public override void SimulateAnimator(PawnAnimator anim)
	{
		anim.SetParam("holdtype", 2); // TODO this is shit
		anim.SetParam("aimat_weight", 1.0f);
	}

	public override void Reload()
	{
		PlaySound( "pistol_reload_scout" );

		if ( IsReloading )
			return;

		if ( AmmoClip >= ClipSize )
			return;

		TimeSinceReload = 0;

		if ( Owner is DeathmatchPlayer player )
		{
			if ( player.AmmoCount( AmmoType ) <= 0 )
				return;

			StartReloadEffects();
		}

		IsReloading = true;

		(Owner as AnimEntity).SetAnimBool( "b_reload", true );

		StartReloadEffects();
	}
}
using Sandbox;
using System;


[Library( "tfdm_scoutpistol", Title = "Pistol" )]

partial class ScoutPistol : BaseDmWeapon
{ 
	public override string ViewModelPath => "models/weapons/scoutwpns/v_pistol.vmdl";

	float ChickenRandomizer = Rand.Float(1, 1000); 

	public override float PrimaryRate => 7.5f;
	public override AmmoType AmmoType => AmmoType.Pistol;
	public override int ClipSize => 12;
	public override float ReloadTime => 1f;
	public override int Bucket => 1;

	public float SoundRNG = Rand.Float(0.1f, 0.2f);

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/weapons/scoutwpns/w_pistol.vmdl" );  

		AmmoClip = 12;
	}

	public override void AttackPrimary() 
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( !TakeAmmo( 1 ) )
		{
			DryFire();
			return;
		}

		(Owner as AnimEntity).SetAnimBool( "b_attack", true );

		if (ChickenRandomizer < 1000 || ChickenRandomizer == 1) 
		{
			PlaySound("pistol_shoot");
		}

		if (ChickenRandomizer == 1000)
		{
			PlaySound("chicken");
		}
		
		if (AmmoClip == 0) 
		{
			Reload();
		}

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();

		ShootBullet(0.1f, 1.5f, 5.0f, 3.0f);
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );
		Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );

		ViewModelEntity?.SetAnimBool( "fire", true );

		if ( IsLocalPawn )
		{
			new Sandbox.ScreenShake.Perlin(1.0f, 1.5f, 2.0f);
		}

		CrosshairPanel?.CreateEvent( "fire" );
	}
}

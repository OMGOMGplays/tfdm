using Sandbox;
using System;


[Library( "tfdm_scattergun", Title = "Scattergun" )]

partial class Scattergun : BaseDmWeapon
{ 
	public override string ViewModelPath => "models/weapons/scoutwpns/v_scattergun.vmdl";

	float ChickenRandomizer = Rand.Float(1, 1000); 

	public override float PrimaryRate => 1.5f;
	public override AmmoType AmmoType => AmmoType.Buckshot;
	public override int ClipSize => 6;
	public override float ReloadTime => 0.5f;
	public override int Bucket => 0;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/weapons/scoutwpns/w_scattergun.vmdl" );  

		AmmoClip = 6;

		FinishReload();
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
			PlaySound("scatter_gun_shoot");
		}

		if (ChickenRandomizer == 1000)
		{
			PlaySound("chicken");
		}

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();

		if (AmmoClip == 0) 
		{
			Reload();
		}

		if (AmmoClip == 6) 
		{
			FinishReload();
		}

		//
		// Shoot the bullets
		//
		for ( int i = 0; i < 10; i++ )
		{
			ShootBullet( 0.15f, 0.3f, 6f, 3.0f );
		}
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );

		ViewModelEntity?.SetAnimBool( "fire", true );

		if ( IsLocalPawn )
		{
			new Sandbox.ScreenShake.Perlin(1.0f, 1.5f, 2.0f);
		}

		CrosshairPanel?.CreateEvent( "fire" );
	}

	public override void OnReloadFinish()
	{
		IsReloading = false;

		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( AmmoClip >= ClipSize )
			return;

		if ( Owner is DeathmatchPlayer player )
		{
			var ammo = player.TakeAmmo( AmmoType, 1 );
			if ( ammo == 0 )
				return;

			AmmoClip += ammo;

			if ( AmmoClip < ClipSize )
			{
				Reload();
				PlaySound("scatter_gun_reload");
				Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );
			}
			else
			{
				FinishReload();
				(Owner as AnimEntity).SetAnimBool("b_finishreload", true);
			}
		}
	}

	protected virtual void FinishReload()
	{
		ViewModelEntity?.SetAnimBool( "reload_finished", true );
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 0 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}
}

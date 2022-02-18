using Sandbox;
using System;


[Library( "tfdm_grenadelauncher", Title = "Grenade Launcher" )]

partial class GrenadeLauncher : BaseDmWeapon
{ 
	public override string ViewModelPath => "models/weapons/demowpns/v_grenadelauncher.vmdl";

	public override float PrimaryRate => 1.5f;
	public override AmmoType AmmoType => AmmoType.PipeGrenade;
	public override int ClipSize => 4;
	public override float ReloadTime => 0.7f;
	public override int Bucket => 0;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/weapons/demowpns/w_grenadelauncher.vmdl" );  

		AmmoClip = 4;

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

		if (AmmoClip == 0) 
		{
			Reload();
		}

		(Owner as AnimEntity).SetAnimBool( "b_attack", true );

		ShootEffects();
		ShootGrenade();
		PlaySound("grenade_launcher_shoot");
	}

	private void ShootGrenade()
	{
		var grenade = new Grenade
		{
			Position = Owner.EyePosition + Owner.EyeRotation.Forward * 50,
			Rotation = Owner.EyeRotation,
		};

		//TODO: Should be replaced with an actual grenade model
		grenade.Velocity = Owner.EyeRotation.Forward * 2000;
		grenade.Owner = Owner;

		grenade.ExplodeAsync(3f);
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
				PlaySound("grenade_launcher_drum_load");
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
		PlaySound("grenade_launcher_drum_close");
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 0 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}
}

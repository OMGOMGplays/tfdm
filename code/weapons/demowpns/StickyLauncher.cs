using Sandbox;
using System;


[Library( "tfdm_stickybomblauncher", Title = "Stickybomb Launcher" )]

partial class StickybombLauncher : BaseDmWeapon
{ 
	public override string ViewModelPath => "models/weapons/demowpns/v_stickybomb_launcher.vmdl";

	public override float PrimaryRate => 1.5f;
	public override AmmoType AmmoType => AmmoType.StickyGrenade;
	public override int ClipSize => 8;
	public override float ReloadTime => 0.7f;
	public override int Bucket => 1;

	public int numberOfStickies = 0;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/weapons/scoutwpns/w_scattergun.vmdl" );  

		AmmoClip = 8;

		numberOfStickies = 0;

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

		PlaySound("stickybomblauncher_shoot");

		numberOfStickies++;
	}

	private void ShootGrenade()
	{
		if (Host.IsClient)
			return;

		var grenade = new Prop
		{
			Position = Owner.EyePos + Owner.EyeRot.Forward * 75,
			Rotation = Owner.EyeRot,
		};

		grenade.SetModel("models/weapons/demowpns/w_stickybomb.vmdl");
		grenade.Velocity = Owner.EyeRot.Forward * 1000;
	}

	public override void AttackSecondary() 
	{
		if (numberOfStickies > 0) 
		{
			TimeSincePrimaryAttack = 0;
			TimeSinceSecondaryAttack = -0.5f;

			PlaySound("stickybomblauncher_det");

			numberOfStickies = 0;
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
				PlayReloadSound();

				if (Input.Pressed(InputButton.Attack2) && numberOfStickies > 0) 
				{
					PlaySound("stickybomblauncher_det");

					numberOfStickies = 0;
				}
			}
			else
			{
				FinishReload();
				(Owner as AnimEntity).SetAnimBool("b_finishreload", true);
			}
		}
	}

    protected virtual async void PlayReloadSound() 
    {
        PlaySound("stickybomblauncher_boltback");

        await GameTask.Delay(335);
        PlaySound("stickybomblauncher_boltforward");
    }

	protected virtual void FinishReload()
	{
		ViewModelEntity?.SetAnimBool( "reload_finished", true );
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 1 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}
}

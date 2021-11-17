using Sandbox;
using System;

[Library("tfdm_sniperrifle", Title = "Sniper Rifle")]

partial class SniperRifle : BaseDmWeapon 
{
	public override string ViewModelPath => "models/weapons/sniperwpns/v_sniperrifle.vmdl";
	public override float PrimaryRate => 0.5f;
	public override float SecondaryRate => 1;
	public override AmmoType AmmoType => AmmoType.SniperAmmo;
	public override int ClipSize => 1;
	public override float ReloadTime => 2.0f;
	public override int Bucket => 0;

	[Net]
	public bool Zoomed { get; set; }

    public override void Spawn() 
    {
        base.Spawn();

		SetModel( "models/weapons/hvywpnswpns/w_shotgun.vmdl" );  

		AmmoClip = 1;
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

		PlaySound("wa");

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();

		if (AmmoClip == 0) 
		{
			Reload();
		}

        ShootBullet( 0f, 0.3f, 25f, 3.0f );

		if (Zoomed) 
		{
	        ShootBullet( 0f, 0.3f, 25f, 3.0f );
		}
    }

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		// Zoomed = Input.Down( InputButton.Attack2 );

		if (Input.Pressed(InputButton.Attack2)) 
		{
			Zoomed = !Zoomed;
		}

		(Owner as AnimEntity).SetAnimBool("b_zoom", Zoomed);

		if (Zoomed) 
		{
			ViewModelEntity?.SetBodyGroup("Hide_Weapon", 1);
		}

		if (!Zoomed) 
		{
			ViewModelEntity?.SetBodyGroup("Hide_Weapon", 0);
		}

		if (Input.Pressed(InputButton.Attack1) && Zoomed) 
		{
			Zoomed = !Zoomed;
		}
	}

	public override void PostCameraSetup( ref CameraSetup camSetup )
	{
		base.PostCameraSetup( ref camSetup );

		if ( Zoomed )
		{
			camSetup.FieldOfView = 20;
		}
	}

	public override void BuildInput( InputBuilder owner ) 
	{
		if ( Zoomed )
		{
			owner.ViewAngles = Angles.Lerp( owner.OriginalViewAngles, owner.ViewAngles, 0.2f );
		}
	}

	public override void SimulateAnimator(PawnAnimator anim) 
	{
		anim.SetParam("holdtype", 0);
	}
}
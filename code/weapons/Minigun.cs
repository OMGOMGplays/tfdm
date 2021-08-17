using Sandbox;
using System;

[Library("tfdm_minigun", Title = "Minigun")]

partial class Minigun : BaseDmWeapon 
{
    public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";

    public override float PrimaryRate => 30.0f;
    public override int ClipSize => 300;
    public override float ReloadTime => 99999.0f;

    public override void Spawn() 
    {
        base.Spawn();

        SetModel("weapons/rust_smg/rust_smg.vmdl");
        AmmoClip = 300;
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

		(Owner as AnimEntity).SetAnimBool( "b_minigunattack", true );

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();
		PlaySound("minigun_shoot");

		//
		// Shoot the bullets
		//
		ShootBullet( 0.1f, 1.5f, 5.0f, 3.0f );

	}

	public override void SimulateAnimator(PawnAnimator anim) 
	{
		anim.SetParam("holdtype", 0);
		anim.SetParam("aimat_weight", 1.0f);
	}
}
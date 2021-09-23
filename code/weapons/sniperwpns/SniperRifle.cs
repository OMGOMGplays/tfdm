using Sandbox;
using System;

[Library("tfdm_sniperrifle", Title = "Sniper Rifle")]

partial class SniperRifle : BaseDmWeapon 
{
	public override string ViewModelPath => "models/hvwpns/hvywpnswpns/c_shotgun.vmdl";
	public override float PrimaryRate => 0.5f;
	public override float SecondaryRate => 1;
	public override AmmoType AmmoType => AmmoType.SniperAmmo;
	public override int ClipSize => 1;
	public override float ReloadTime => 1.5f;
	public override int Bucket => 0;

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

		PlaySound("sniper_item_birdhead_round_start02");

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();

		if (AmmoClip == 0) 
		{
			Reload();
		}

        ShootBullet( 0f, 0.3f, 9.0f, 3.0f );
    }

	public override void SimulateAnimator(PawnAnimator anim) 
	{
		anim.SetParam("holdtype", 0);
	}
}
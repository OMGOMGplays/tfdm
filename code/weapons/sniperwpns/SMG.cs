using Sandbox;
using System;

[Library("tfdm_smg", Title = "SMG")]

partial class SMG : BaseDmWeapon 
{
    public override string ViewModelPath => "models/weapons/scoutwpns/v_scattergun.vmdl";

	public override float PrimaryRate => 10f;
	public override AmmoType AmmoType => AmmoType.SMGAmmo;
	public override int ClipSize => 25;
	public override float ReloadTime => 1.5f;
	public override int Bucket => 1;

    public override void Spawn()
    {
        base.Spawn();

        SetModel("models/weapons/scoutwpns/w_scattergun.vmdl");

        AmmoClip = 25;
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
		
		if (AmmoClip == 0) 
		{
			Reload();
			return;
		}

		// Tell the clients to play the shoot effects
		//
		ShootEffects();

		ShootBullet(0.1f, 1.5f, 8f, 3.0f);
    }

    public override void SimulateAnimator(PawnAnimator anim) 
    {
        anim.SetParam("holdtype", 1);
    }
}
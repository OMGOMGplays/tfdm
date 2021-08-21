using Sandbox;
using System;

[Library("tfdm_minigun", Title = "Minigun")]

partial class Minigun : BaseDmWeapon 
{
    public override string ViewModelPath => "models/weapons/c_minigun.vmdl";

    public override float PrimaryRate => 15.0f;
    public override int ClipSize => 300;
	public override int Bucket => 0;
	public override AmmoType AmmoType => AmmoType.Minigun;

    public override void Spawn() 
    {
        base.Spawn();

        SetModel("models/weapons/w_minigun.vmdl");
        AmmoClip = 300;
	}

    public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( !TakeAmmo( 1 ) )
		{
			DryFire();
			(Owner as AnimEntity).SetAnimBool("b_minigunend", true);
			ViewModelEntity?.SetAnimBool("spooldown", true);
			return;
		}

		
		if (Input.Down(InputButton.Attack1) == true) 
		{
			AttackPrimaryStart();

			ShootEffects();

			ShootBullet( 0.1f, 1.5f, 5.0f, 3.0f );
			
			return;
		}

	}

	public override bool CanReload() 
	{
		return false;
	}

	public void AttackPrimaryStart() 
	{
		(Owner as AnimEntity).SetAnimBool("b_minigunstart", true);
		ViewModelEntity?.SetAnimBool("spoolup", true);
	}

	public void AttackPrimaryEnd() 
	{
		(Owner as AnimEntity).SetAnimBool("b_minigunend", true);
		ViewModelEntity?.SetAnimBool("spooldown", true);
	}

	public override void SimulateAnimator(PawnAnimator anim) 
	{
		anim.SetParam("holdtype", 0);
		anim.SetParam("aimat_weight", 1.0f);
	}
}
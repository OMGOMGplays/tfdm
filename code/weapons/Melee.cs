using Sandbox;
using System;

[Library("tfdm_melee", Title = "Melee")]

partial class Melee : BaseDmWeapon 
{
    public override string ViewModelPath => "models/weapons/c_heavy_arms.vmdl";
    public override float PrimaryRate => 1.0f;
	public override int Bucket => 2;
	public override AmmoType AmmoType => AmmoType.Melee;

    public override void Spawn() 
    {
        base.Spawn();

        SetModel("");
        AmmoClip = 1;
    }

	public override void AttackPrimary()
	{
		if ( MeleeAttack() )
		{
			OnMeleeHit();
		}
		else
		{
			OnMeleeMiss();
		}

		PlaySound( "" );
	}
	
	public override bool CanReload() 
	{
		return false;
	}

	private bool MeleeAttack()
	{
		var forward = Owner.EyeRot.Forward;
		forward = forward.Normal;

		bool hit = false;

		foreach ( var tr in TraceBullet( Owner.EyePos, Owner.EyePos + forward * 80, 20.0f ) )
		{
			if ( !tr.Entity.IsValid() ) continue;

			tr.Surface.DoBulletImpact( tr );

			hit = true;

			if ( !IsServer ) continue;

			using ( Prediction.Off() )
			{
				var damageInfo = DamageInfo.FromBullet( tr.EndPos, forward * 100, 25 )
					.UsingTraceResult( tr )
					.WithAttacker( Owner )
					.WithWeapon( this );

				tr.Entity.TakeDamage( damageInfo );
			}
		}

		return hit;
	}

	[ClientRpc]
	private void OnMeleeMiss()
	{
		Host.AssertClient();

		if ( IsLocalPawn )
		{
			_ = new Sandbox.ScreenShake.Perlin();
		}

        (Owner as AnimEntity).SetAnimBool("b_attack", true);

		PlaySound("boxing_gloves_swing2");

		ViewModelEntity?.SetAnimBool( "attack", true );
	}

	[ClientRpc]
	private void OnMeleeHit()
	{
		Host.AssertClient();

		if ( IsLocalPawn )
		{
			_ = new Sandbox.ScreenShake.Perlin( 1.0f, 1.0f, 3.0f );
		}

        (Owner as AnimEntity).SetAnimBool("b_attack", true);

		PlaySound("fist_hit_world1");

		ViewModelEntity?.SetAnimBool( "attack", true );
	}

    public override void SimulateAnimator(PawnAnimator anim) 
    {
        anim.SetParam("holdtype", 3);
    }
}
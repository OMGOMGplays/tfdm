using Sandbox;
using System;
using System.Linq;
using System.Numerics;
using System.Threading;

partial class DeathmatchPlayer : Player
{
	private TimeSince timeSinceInAir;

	[ServerCmd( "changeclass" )]
    public static void ChangeClass(string Class)
    {
		var caller = ConsoleSystem.Caller;

		if (caller == null) return;

		if (ConsoleSystem.Caller.Pawn is DeathmatchPlayer player)
		{
			if (Class == "Scout" || Class == "scout") 
			{
				Scout = true;
				Heavy = false;
				Demoman = false;
				Sniper = false;

				// Sandbox.UI.ChatBox.Say("You will respawn as Scout");
			}

			if (Class == "Heavy" || Class == "heavy") 
			{
				Scout = false;
				Heavy = true;
				Demoman = false;
				Sniper = false;

				// Sandbox.UI.ChatBox.Say("You will respawn as Heavy");
			}

			if (Class == "Demoman" || Class == "Demo" || Class == "demoman" || Class == "demo") 
			{
				Scout = false;
				Heavy = false;
				Demoman = true;
				Sniper = false;

				// Sandbox.UI.ChatBox.Say("You will respawn as Demoman");
			}

			if (Class == "Sniper" || Class == "sniper") 
			{
				Scout = false;
				Heavy = false;
				Demoman = false;
				Sniper = true;

				// Sandbox.UI.ChatBox.Say("You will respawn as Sniper");
			}

			Sandbox.UI.ChatBox.Say($"You will respawn as {Class}");
		}
	}

	private int numberOfJumps;

	static bool Scout = true;
	static bool Heavy = false;
	static bool Demoman = false;
	static bool Sniper = false;


	public bool SupressPickupNotices { get; private set; }

	public DeathmatchPlayer()
	{
		Inventory = new DmInventory(this);
	}

	public virtual void CheckJumpButton()
	{	
		if (GroundEntity == null) 
		{
			return;	
		}
	}

	public override void Respawn()
	{
		numberOfJumps = 0;

		// var player = DeathmatchPlayer;

		if (Scout == true) 
		{
			SetModel( "models/scout/scout.vmdl" );

			Inventory.Add(new Scattergun(), true);
			Inventory.Add(new ScoutPistol());
			Inventory.Add(new Bat());

			Controller = new ScoutWalkController();
		}

		if (Heavy == true) 
		{
			SetModel("models/hvwpns/hvywpns.vmdl");

			Inventory.Add(new Minigun(), true);
			Inventory.Add(new Shotgun());
			Inventory.Add(new Fists());	

			Controller = new HeavyWalkController();
		}

		if (Demoman == true) 
		{
			SetModel("models/demo/demo.vmdl");

			Inventory.Add(new GrenadeLauncher(), true);
			// Inventory.Add(new StickybombLauncher());
			Inventory.Add(new Bottle());

			Controller = new DemoWalkController();
		}

		if (Sniper == true) 
		{
			SetModel("models/sniper/sniper.vmdl");

			Inventory.Add(new SniperRifle(), true);
			Inventory.Add(new SMG());
			Inventory.Add(new Kukri());

			Controller = new SniperWalkController();
		}

		Animator = new StandardPlayerAnimator();
		Camera = new FirstPersonCamera();
		  
		EnableAllCollisions = true; 
		EnableDrawing = true; 
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		ClearAmmo();

		SupressPickupNotices = true;

		if (Heavy == true) 
		{
			GiveAmmo(AmmoType.Buckshot, 20);
		}

		if (Scout == true) 
		{
			GiveAmmo(AmmoType.Pistol, 36);
			GiveAmmo(AmmoType.Buckshot, 32);
		}

		if (Demoman == true) 
		{
			GiveAmmo(AmmoType.PipeGrenade, 16);
			GiveAmmo(AmmoType.StickyGrenade, 24);
		}

		if (Sniper == true) 
		{
			GiveAmmo(AmmoType.SniperAmmo, 24);
			GiveAmmo(AmmoType.SMGAmmo, 75);
		}

		Health = 100;

		SupressPickupNotices = false;

		base.Respawn();
	}

	public override void OnKilled()
	{
		base.OnKilled();

		Inventory.DropActive();
		Inventory.DeleteContents();

		BecomeRagdollOnClient( LastDamage.Force, GetHitboxBone( LastDamage.HitboxIndex ) );

		Controller = null;
		Camera = new SpectateRagdollCamera();

		if (Heavy == true)
		{
			PlaySound("heavy_painsevere03");
		}

		if (Scout == true) 
		{
			PlaySound("scout_painsevere03");
		}

		if (Demoman == true) 
		{
			PlaySound("demoman_paincrticialdeath01");
		}

		EnableAllCollisions = false;
		EnableDrawing = false;
        numberOfJumps = 0;		
	}


	public override void Simulate( Client cl )
	{
		//if ( cl.NetworkIdent == 1 )
		//	return;

		base.Simulate( cl );

		//
		// Input requested a weapon switch
		//
		if ( Input.ActiveChild != null )
		{
			ActiveChild = Input.ActiveChild;
		}

		if ( LifeState != LifeState.Alive )
			return;

		TickPlayerUse();

		if ( Input.Pressed( InputButton.View ))
		{
			if ( Camera is ThirdPersonCamera )
			{
				Camera = new FirstPersonCamera();
			}
			else
			{
				Camera = new ThirdPersonCamera();
			}
		}

		if (Scout == true) 
		{
			if (GroundEntity == null)
			{
				if (timeSinceInAir > 0.1f && Input.Pressed(InputButton.Jump))
				{
					if (numberOfJumps < 2)
					{
						DoubleJump();
					}
				}
			}
			
			else
			{
				numberOfJumps = 0;
				timeSinceInAir = 0;
			}
		}

		SimulateActiveChild( cl, ActiveChild );

		//
		// If the current weapon is out of ammo and we last fired it over half a second ago
		// lets try to switch to a better wepaon
		//
		if ( ActiveChild is BaseDmWeapon weapon && !weapon.IsUsable() && weapon.TimeSincePrimaryAttack > 0.5f && weapon.TimeSinceSecondaryAttack > 0.5f )
		{
			SwitchToBestWeapon();
		}
	}

	public void SwitchToBestWeapon()
	{
		var best = Children.Select( x => x as BaseDmWeapon )
			.Where( x => x.IsValid() && x.IsUsable() )
			.OrderByDescending( x => x.BucketWeight )
			.FirstOrDefault();

		if ( best == null ) return;

		ActiveChild = best;
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );
	}

	Rotation lastCameraRot = Rotation.Identity;

	public override void PostCameraSetup( ref CameraSetup setup )
	{
		base.PostCameraSetup( ref setup );

		if ( lastCameraRot == Rotation.Identity )
			lastCameraRot = setup.Rotation;

		var angleDiff = Rotation.Difference( lastCameraRot, setup.Rotation );
		var angleDiffDegrees = angleDiff.Angle();
		var allowance = 5.0f;

		if ( angleDiffDegrees > allowance )
		{
			// We could have a function that clamps a rotation to within x degrees of another rotation?
			lastCameraRot = Rotation.Lerp( lastCameraRot, setup.Rotation, 1.0f - (allowance / angleDiffDegrees) );
		}
		else
		{
			//lastCameraRot = Rotation.Lerp( lastCameraRot, Camera.Rotation, Time.Delta * 0.2f * angleDiffDegrees );
		}

		// uncomment for lazy cam
		//camera.Rotation = lastCameraRot;

		if ( setup.Viewer != null )
		{
			AddCameraEffects( ref setup );
		}
	}

	float walkBob = 0;
	float lean = 0;
	float fov = 0;

	private void AddCameraEffects( ref CameraSetup setup )
	{
		var speed = Velocity.Length.LerpInverse( 0, 320 );
		var forwardspeed = Velocity.Normal.Dot( setup.Rotation.Forward );

		var left = setup.Rotation.Left;
		var up = setup.Rotation.Up;

		if ( GroundEntity != null )
		{
			walkBob += Time.Delta * 25.0f * speed;
		}

		setup.Position += up * MathF.Sin( walkBob ) * speed * 2;
		setup.Position += left * MathF.Sin( walkBob * 0.6f ) * speed * 1;

		// Camera lean
		lean = lean.LerpTo( Velocity.Dot( setup.Rotation.Right ) * 0.01f, Time.Delta * 15.0f );

		var appliedLean = lean;
		appliedLean += MathF.Sin( walkBob ) * speed * 0.2f;
		setup.Rotation *= Rotation.From( 0, 0, appliedLean );

		speed = (speed - 0.7f).Clamp( 0, 1 ) * 3.0f;

		fov = fov.LerpTo( speed * 20 * MathF.Abs( forwardspeed ), Time.Delta * 2.0f );

		setup.FieldOfView += fov;

	//	var tx = new Sandbox.UI.PanelTransform();
	//	tx.AddRotation( 0, 0, lean * -0.1f );

	//	Hud.CurrentPanel.Style.Transform = tx;
	//	Hud.CurrentPanel.Style.Dirty(); 

	}

	DamageInfo LastDamage;

	public override void TakeDamage( DamageInfo info )
	{
		LastDamage = info;

		// hack - hitbox 0 is head
		// we should be able to get this from somewhere
		if ( info.HitboxIndex == 0 && Sniper == true )
		{
			info.Damage *= 2.0f;
		}
		
		base.TakeDamage( info );

		if ( info.Attacker is DeathmatchPlayer attacker && attacker != this )
		{
			// Note - sending this only to the attacker!
			attacker.DidDamage( To.Single( attacker ), info.Position, info.Damage, Health.LerpInverse( 100, 0 ) );

			TookDamage( To.Single( this ), info.Weapon.IsValid() ? info.Weapon.Position : info.Attacker.Position );
		}
	}

	[ClientRpc]
	public void DidDamage( Vector3 pos, float amount, float healthinv )
	{
		Sound.FromScreen( "hitsound" )
			.SetPitch( 1 + healthinv * 1 );

	}

	[ClientRpc]
	public void TookDamage( Vector3 pos )
	{
		//DebugOverlay.Sphere( pos, 5.0f, Color.Red, false, 50.0f );

		DamageIndicator.Current?.OnHit( pos );
	}

    public virtual void DoubleJump()
    {
        float flGroundFactor = 1.25f;

        float flMul = 268.3281572999747f * 1.2f;

        Vector3 startx = new Vector3(); //LocalVelocity.x;
        Vector3 starty = new Vector3(); //LocalVelocity.y;

        if (Input.Down(InputButton.Left))
        {
            //starty = LocalVelocity.y * .25f;
            starty = Rotation.Left;
        }
        if (Input.Down(InputButton.Right))
        {
            //starty = LocalVelocity.y * 1.25f;
            starty = Rotation.Right;
        }
        if (Input.Down(InputButton.Back))
        {
            //startx = LocalVelocity.x * 1.25f;
            startx = Rotation.Backward;
        }
        if (Input.Down(InputButton.Forward))
        {
            //startx = LocalVelocity.x * .25f;
            startx = Rotation.Forward;
            
        }

        //Log.Info("Global: " + EyeRot);
        //Log.Info("Local: " + EyeRotLocal);

        //Velocity = Velocity.AddClamped((startx * 100.0f) + (starty * 100f) + Velocity.WithZ(flMul * flGroundFactor), 500.0f);

        Velocity = (startx * 100.0f) + (starty * 100f) + Velocity.WithZ(flMul * flGroundFactor);

        // Velocity -= new Vector3(0, 0, Pawn.Gravity) * Time.Delta;

        //Log.Info("Local: " + LocalVelocity.x + " " + LocalVelocity.y + " " + LocalVelocity.z);
        //Log.Info("Global: " + Pawn.Velocity.x + " " + Pawn.Velocity.y + " " + Pawn.Velocity.z);

        // Pawn.AirMove();

        // Pawn.AddEvent("jump");

		numberOfJumps = 2;
    }
}

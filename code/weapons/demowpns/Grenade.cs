using System;
using Sandbox;

[Library("grenade")]
partial class Grenade : Prop
{
    public override void Spawn()
    {
        base.Spawn();
        SetModel( "models/weapons/demowpns/w_grenade_grenadelauncher.vmdl" );
    }

    protected override void OnPhysicsCollision( CollisionEventData eventData )
    {
        base.OnPhysicsCollision( eventData );

        var grenade = this;
        var other = eventData.Entity;
        if ( this == other ) return;

        if ( other is not DeathmatchPlayer ) return;

        // We cant touch ourselves.
        if ( other == Owner ) return;
        // var Enemy = Entity;
        // Enemy = other;
    }
}


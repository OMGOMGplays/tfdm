
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

public class SniperScope : Panel
{

    bool Scoping {get; set;}

	public SniperScope()
	{
		StyleSheet.Load( "/ui/SniperScope.scss" );

		for( int i=0; i<5; i++ )
		{
			var p = Add.Panel( "element" );
			p.AddClass( $"el{i}" );
		}
	}

	public override void Tick()
	{
		base.Tick();
		this.PositionAtCrosshair();

		SetClass( "scoping", !Scoping );

        if (Input.Pressed(InputButton.Attack2))
            Scoping = false;
	}

	[PanelEvent]
	public void FireEvent()
	{
	}
}

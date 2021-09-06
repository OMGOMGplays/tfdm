
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Vitals : Panel
{
	public Label Health;

	public Vitals()
	{
		Health = Add.Label( "100", "health" );
	}

	public override void Tick()
	{
		var player = Local.Pawn;
		if ( player == null ) return;

		Health.Text = $"{player.Health.CeilToInt()}";
		Health.SetClass( "danger", player.Health <= 75.0f );
		Health.SetClass( "danger2", player.Health <= 50.0f );
		Health.SetClass( "danger3", player.Health <= 25.0f );
		Health.SetClass( "dead", player.Health == 0.0f );		
	}
}

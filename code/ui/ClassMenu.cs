using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class ClassMenu : Panel
{
	public ClassMenu()
	{
		StyleSheet.Load( "/ui/ClassMenu.scss" );

		Add.Label( "Class Select Menu", "header" );

		AddChild<ClassMenuButtons>();
	}

	public override void Tick()
	{
		SetClass( "open", Input.Down( InputButton.Menu ) );
	}
}

public class ClassMenuButtons : Panel
{
	public ClassMenuButtons()
	{
		Add.Label( "Classes", "section" );

		Add.Label( "Scout", "button" ).AddEventListener( "onclick", () => {
			ConsoleSystem.Run( "changeclass Scout" );
		} );

		Add.Label( "Heavy", "button" ).AddEventListener( "onclick", () => {
			ConsoleSystem.Run( "changeclass Heavy" );
		} );

		Add.Label( "Demoman", "button" ).AddEventListener( "onclick", () => {
			ConsoleSystem.Run( "changeclass Demoman" );
		} );

		Add.Label("Main Console Commands", "section");

		Add.Label("Killbind", "button").AddEventListener("onclick", () => {
			ConsoleSystem.Run("kill");
		});
		
		Add.Label("Add a bot", "button").AddEventListener("onclick", () => {
			ConsoleSystem.Run("bot_add");
		});
	}
}
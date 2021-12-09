﻿
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;

[Library]
public partial class DeathmatchHud : HudEntity<RootPanel>
{
	public DeathmatchHud()
	{
		if ( !IsClient )
			return;

		RootPanel.StyleSheet.Load( "/ui/DeathmatchHud.scss" );

		RootPanel.AddChild<Vitals>();
		RootPanel.AddChild<Ammo>();

		RootPanel.AddChild<DamageIndicator>();

		RootPanel.AddChild<InventoryBar>();
		RootPanel.AddChild<PickupFeed>();
		
		RootPanel.AddChild<ChatBox>();
		RootPanel.AddChild<KillFeed>();
		RootPanel.AddChild<Scoreboard>();
		RootPanel.AddChild<VoiceList>();
		RootPanel.AddChild<ClassMenu>();
	}

	[ClientRpc]
	public void OnPlayerDied( string victim, string attacker = null )
	{
		Host.AssertClient();
	}

	[ClientRpc]
	public void ShowDeathScreen( string attackerName )
	{
		Host.AssertClient();
	}
}

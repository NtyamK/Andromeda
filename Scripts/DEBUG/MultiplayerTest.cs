using Godot;
using System;
using PlayFab;
using PlayFab.ClientModels;

public partial class MultiplayerTest : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
		{
			TitleId = PlayFabSettings.TitleId,
			CreateAccount = true,
			CustomId = "23242"
		};

		PlayFabClientAPI.LoginWithCustomID(request, OnPl)
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Join button clicked		
	private void _on_join_pressed()
	{
		GD.Print("Clicked");
	}
}

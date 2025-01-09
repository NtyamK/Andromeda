using Godot;
using System;

/* Playfab dependencies */
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.ServerModels;

public partial class NetworkManager : Node2D
{

	// Port: Wall through the firewall to establish a connection.
	const int PORT = 25565;

	// Get access to the UI elements underneath. 
	[Export] TextEdit emailTextEdit;
	[Export] TextEdit passwordTextEdit;
	[Export] Control uiControl;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		
		// PlayFab integration
		PlayFabSettings.staticSettings.TitleId = "7882E";
		PlayFabSettings.staticSettings.DeveloperSecretKey = "YING8THDD6GCIYS86CCMHHRMCP3SEJJR8SE4RYQ51EY97RPRUN";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void _on_login_button_pressed()
	{
		// Create the client
		ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
		peer.CreateClient("127.0.0.1", PORT);
		if (peer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Disconnected)
		{
			GD.Print("Client connection failed");
			return;
		}
		Multiplayer.MultiplayerPeer = peer;

		// Register the player
		LoginPlayer();
	}

	private void _on_register_button_pressed()
	{
		// Create the request
		RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest
		{
			Email = emailTextEdit.Text,
			Password = passwordTextEdit.Text, 
			RequireBothUsernameAndEmail = false
		};
		
		var userRegister = PlayFabClientAPI.RegisterPlayFabUserAsync(request);
	}

	private async void LoginPlayer()
	{
		// Create the request
		LoginWithEmailAddressRequest request = new LoginWithEmailAddressRequest
		{
			Email = emailTextEdit.Text,
			Password = passwordTextEdit.Text
		};

		var userLogin = PlayFabClientAPI.LoginWithEmailAddressAsync(request);
		PlayFabResult<LoginResult> loginCredentials = await userLogin;
		Rpc("ServerAuthenticateLogin", loginCredentials.Result.PlayFabId);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private async void ServerAuthenticateLogin(string playerId)
	{
		AuthenticateSessionTicketRequest request = new AuthenticateSessionTicketRequest
		{
			SessionTicket = playerId
		};

		var ticketAuth = PlayFabServerAPI.AuthenticateSessionTicketAsync(request);
		var newAuth = await ticketAuth;

		if (newAuth != null)
		{
			GD.Print("Login succeded");
		} else 
		{
			GD.Print("Login failed");
		}
	}
}

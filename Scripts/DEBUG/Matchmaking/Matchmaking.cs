using Godot;
using System;
using PlayFab;
using PlayFab.ServerModels;
using PlayFab.ClientModels;

public partial class Matchmaking : Node2D
{
	// Playfab
	private string playFabTitleId = "7882E";  // Get from PlayFab Game Manager
    private string developerSecretKey = "YING8THDD6GCIYS86CCMHHRMCP3SEJJR8SE4RYQ51EY97RPRUN";  // Get from PlayFab Game Manager

	// SERVER PARAMETERS
	const int SERVER_PORT = 8080;
	const string SERVER_IP = "127.0.0.1";

	// PLAYER OBJECT
	[Export] public PackedScene playerScene { get; set; }
	[Export] public Node2D playerGroup { get; set; }
	[Export] public Control UI { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PlayFabSettings.staticSettings.TitleId = playFabTitleId; // Set PlayFab title
        PlayFabSettings.staticSettings.DeveloperSecretKey = developerSecretKey;  // Set your secret key
	}
	private void OnLoginSuccess(LoginResult result)
    {
        GD.Print("Login successful!");
    }

    private void OnLoginError(PlayFabError error)
    {
        GD.Print("Login failed: " + error.ErrorMessage);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Button pressed callback
	private void _on_find_match_label_pressed()
	{
		UI.Hide();
		BecomeClient();
		LoginClient();
	}

	private void _on_host_match_button_pressed()
	{
		UI.Hide();
		BecomeHost();
	}

	// Create a host server
	private void BecomeHost()
	{
		/* Create the server */
		ENetMultiplayerPeer ServerPeer = new ENetMultiplayerPeer();
		ServerPeer.CreateServer(SERVER_PORT);

		MultiplayerApi Multiplayer = GetTree().GetMultiplayer();

		Multiplayer.MultiplayerPeer = ServerPeer;
		Multiplayer.PeerConnected += OnPeerConnected;
		Multiplayer.PeerDisconnected += OnPeerDisconnected;

		OnPeerConnected(1);
	}
	// Handle client connection
	private void BecomeClient()
	{
		ENetMultiplayerPeer ClientPeer = new ENetMultiplayerPeer();
		ClientPeer.CreateClient(SERVER_IP, SERVER_PORT);

		MultiplayerApi Multiplayer = GetTree().GetMultiplayer();

		Multiplayer.MultiplayerPeer = ClientPeer;
	}

	private async void LoginClient()
	{
		LoginWithEmailAddressRequest request = new LoginWithEmailAddressRequest
		{
			Email = "zacharia@mesaeret.com",
			Password = "123456"
		};

		var userLogin = PlayFabClientAPI.LoginWithEmailAddressAsync(request);
		PlayFabResult<LoginResult> loginCredentials = await userLogin;
		Rpc("ServerAuthentificateLogin", loginCredentials.Result.PlayFabId);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private async void ServerAuthentificateLogin(string playerId)
	{
		AuthenticateSessionTicketRequest request = new AuthenticateSessionTicketRequest
		{
			SessionTicket = playerId
		};
		var ticketAuth = PlayFabServerAPI.AuthenticateSessionTicketAsync(request);
		var newAuth = await ticketAuth;

		if (newAuth != null)
		{
			GD.Print("Login successful");
		}else{
			GD.Print("Login failed");
		}
	}

	private void OnPeerDisconnected(long id)
	{
		throw new NotImplementedException();
	}

	private void OnPeerConnected(long id)
	{
		// Add player to the game
		var newPlayer = playerScene.Instantiate();
		newPlayer.Name = id.ToString();

		playerGroup.AddChild(newPlayer);
	}
}

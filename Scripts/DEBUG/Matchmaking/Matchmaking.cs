using Godot;
using System;


public partial class Matchmaking : Node2D
{
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

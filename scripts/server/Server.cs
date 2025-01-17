using Godot;
using Microsoft.Playfab.Gaming.GSDK.CSharp;
using PlayFab;
using System;
using System.Collections.Generic;

public partial class Server : Node
{
	private const float ReadyForPlayersDelay = 0.5f;
    private const float ShutdownDelay = 5f;

	static private List<ConnectedPlayer> players = new List<ConnectedPlayer>();
    public override void _Ready()
    {
        NetworkManager.instance.startService += CreateServer;
    }
    private void CreateServer(int port)
	{
		ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
		peer.CreateServer(port);
		if (peer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Disconnected)
		{
			OS.Alert("Error creating server");
		}
		Multiplayer.MultiplayerPeer = peer;
		GD.Print("Server created successfully");

		// Set callbacks
		Multiplayer.MultiplayerPeer.PeerConnected += OnPlayerConnected;
		Multiplayer.MultiplayerPeer.PeerDisconnected += OnPlayerDisconnected;

		// Start the playfab server
		StartRemoteServer();
	}

    private void StartRemoteServer()
	{
		/* Logs */
		GameserverSDK.LogMessage("[ServerStartUp].StartRemoteServer");
		GD.Print("[ServerStartUp].StartRemoteServer");
		
		/* Start the server */
		GameserverSDK.Start();
		GameserverSDK.RegisterMaintenanceCallback(OnMaintenance);
		GameserverSDK.RegisterShutdownCallback(OnShutdown);
		GameserverSDK.RegisterHealthCallback(IsHealthy);

		// Get the server ready for incoming connections
		ReadyForPlayers();
	}

	async void ReadyForPlayers()
	{
		await ToSignal(GetTree().CreateTimer(ReadyForPlayersDelay), "timeout");
		GameserverSDK.LogMessage("Notifying PlayFab: Ready for players.");
        GD.Print("Notifying PlayFab: Ready for players.");
        GameserverSDK.ReadyForPlayers();
	}

	static void OnPlayerConnected(long playfabID)
	{
		// When a new player connects, you can let PlayFab know by adding it to the vector of players and calling updateConnectedPlayers
		players.Add(new ConnectedPlayer(playfabID.ToString()));
		GameserverSDK.UpdateConnectedPlayers(players);
	}

	private void OnPlayerDisconnected(long playfabID)
	{	
		ConnectedPlayer player = players.Find(x => x.PlayerId.Equals(playfabID.ToString(), StringComparison.OrdinalIgnoreCase));
		players.Remove(player);
		GameserverSDK.UpdateConnectedPlayers(players);
		CheckPlayerCountToShutdown();
	}

	private void CheckPlayerCountToShutdown()
	{
		if (players.Count <= 0) StartShutdownProcess();
	}

	private void StartShutdownProcess()
	{
		GD.Print("[StartShutdownProcess]: Server is shutting down.");
		GameserverSDK.LogMessage("[StartShutdownProcess]: Server is shutting down.");

		ShutdownServer();
	}

	async void ShutdownServer()
	{
		await ToSignal(GetTree().CreateTimer(ShutdownDelay), "timeout");
		GetTree().Quit();
	}
	
	static void OnShutdown()
	{
		GameserverSDK.LogMessage("Shutting down...");
    	/* Perform any necessary cleanup and end the program */
	}
	static void OnMaintenance(DateTimeOffset time)
	{
		/* Perform any necessary cleanup, notify your players, etc. */
	}
	static bool IsHealthy()
	{
		// Return whether your game server should be considered healthy
		return true;
	}
}

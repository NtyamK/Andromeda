using Godot;
using PlayFab;
using System;

public partial class NetworkManager : Node
{

	[Export] public bool isServer = true;
	const int PORT = 8080;

	public delegate void StartService(int port);
	public StartService startService;
	public static NetworkManager instance;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InitializePlayFab();
		instance = this;
		if (isServer)
		{
			AddChild(new Server());
		}else
		{
			AddChild(new Client());
		}

		startService?.Invoke(PORT);
	}

	private void InitializePlayFab()
    {
        // Set your PlayFab Title ID here
        PlayFabSettings.staticSettings.TitleId = "80966";

        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            GD.PrintErr("PlayFab TitleId is not set. Please ensure it is configured properly.");
        }
        else
        {
            GD.Print("PlayFab initialized with Title ID: ", PlayFabSettings.staticSettings.TitleId);
        }
    }

}

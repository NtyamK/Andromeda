using Godot;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Client : Node
{
	public class Configuration
	{
		public string buildId = "";
		public string ipAddress = "";
		public int port = 0;
	}

	Configuration ClientConfig = new Configuration();
	ENetMultiplayerPeer peer;
	public override void _Ready()
    {
        NetworkManager.instance.startService += CreateClient;
    }
	private void CreateClient(int port)
	{
		ClientConfig.port = port;
		// Get the peer ready
		peer = new ENetMultiplayerPeer();
		
		// Login the client
		LoginRemoteUser();
	}

	public async void LoginRemoteUser()
	{
		GD.Print("[ClientStartUp].LoginRemoteUser");

		// We need to login a user to get at PlayFab APIs.
		LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
		{
			TitleId = PlayFabSettings.staticSettings.TitleId,
			CreateAccount = true,
			CustomId = GUIDUtility.getUniqueID()
		};

		var userLogin = PlayFabClientAPI.LoginWithCustomIDAsync(request);
		PlayFabResult<LoginResult> loginCredentials = await userLogin;

		if (loginCredentials.Error != null) 
		{
			OnLoginError(loginCredentials.Error);
		}else
		{
			await OnPlayFabLoginSuccess(loginCredentials.Result);
		}
	}

	private void OnLoginError(PlayFabError response)
	{
		GD.Print(response.ToString());
	}

	private async Task OnPlayFabLoginSuccess(LoginResult response)
	{
		GD.Print("[OnPlayFabLoginSuccess]. " + response.ToString());
		if (ClientConfig.ipAddress == "")
		{
			await RequestMultiplayerServer(); 
		}else
		{
			ConnectRemoteClient();
		}
	}
	private async Task RequestMultiplayerServer()
	{
		GD.Print("[ClientStartUp].RequestMultiplayerServer");
		var requestData = new RequestMultiplayerServerRequest()
		{
			BuildId = ClientConfig.buildId,
			SessionId = System.Guid.NewGuid().ToString(),
			PreferredRegions = new List<string>() { AzureRegion.WestEurope.ToString() }
		};

		var serverTask = PlayFabMultiplayerAPI.RequestMultiplayerServerAsync(requestData);
		PlayFabResult<RequestMultiplayerServerResponse> serverResponse = await serverTask;
		
		if (serverResponse.Error != null)
		{
			OnRequestMultiplayerServerError(serverResponse.Error);
		}else 
		{
			OnRequestMultiplayerServer(serverResponse.Result);
		}
	}
	private void ConnectRemoteClient(RequestMultiplayerServerResponse response = null)
	{
		if (response == null)
		{
			peer.CreateClient(ClientConfig.ipAddress, ClientConfig.port);
		}
		else
		{
			ClientConfig.ipAddress 	= response.IPV4Address;
			ClientConfig.port 		= response.Ports[0].Num; 
			peer.CreateClient(response.IPV4Address, response.Ports[0].Num);
		}

		if (peer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Disconnected)
		{
			OS.Alert("Error creating client");
		}
		Multiplayer.MultiplayerPeer = peer;
		GD.Print("Client created successfully");
	}

	private void OnRequestMultiplayerServer(RequestMultiplayerServerResponse response)
	{
		GD.Print(response.ToString());
		ConnectRemoteClient(response);
	}
	private void OnRequestMultiplayerServerError(PlayFabError error)
	{		
		if (error.ErrorDetails != null)
		{
			GD.Print("[OnRequestMultiplayerServerError] Error Details: " + error.ErrorMessage + error.ErrorDetails);
		}
	}
}

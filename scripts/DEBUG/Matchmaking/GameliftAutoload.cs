using Godot;
using System;
using Amazon.GameLift;
using Amazon.GameLift.Model;

public partial class GameliftAutoload : Node
{
    public override void _Ready()
    {
        base._Ready();
        try
        {
            Amazon.GameLift.Server.Model.ProcessParameters parameters =
                new Amazon.GameLift.Server.Model.ProcessParameters(
                    OnGameSessionStart,
                    OnGameSessionUpdate,
                    OnGameSessionTerminate,
                    OnHealthCheck,
                    null // Log parameters
                );

            var initOutcome = Amazon.GameLift.Server.GameLiftServerAPI.InitSDK();
            if (initOutcome.Success)
            {
                GD.Print("GameLift SDK initialized.");
                Amazon.GameLift.Server.GameLiftServerAPI.ProcessReady(parameters);
            }
            else
            {
                GD.PrintErr("Failed to initialize GameLift SDK.");
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"GameLift initialization error: {ex.Message}");
        }
    }

    private void OnGameSessionStart(GameSession gameSession)
    {
        GD.Print("Game session started: " + gameSession.GameSessionId);
        Amazon.GameLift.Server.GameLiftServerAPI.ActivateGameSession();
    }

    private void OnGameSessionUpdate(UpdateGameSession updateGameSession)
    {
        GD.Print("Game session updated.");
    }

    private void OnGameSessionTerminate()
    {
        GD.Print("Game session terminated.");
        Amazon.GameLift.Server.GameLiftServerAPI.TerminateGameSession();
        GetTree().Quit();
    }

    private bool OnHealthCheck()
    {
        GD.Print("Health check performed.");
        return true; // Return false if the server is unhealthy
    }
}

using System;

// This code defines a GameStateManager that manages the state of a game.
// It uses an enum to represent different game states and implements a Singleton pattern
public enum GameState
{
    NotStarted,
    InProgress,
    GameOver,
    Victory
}

// This class manages the state of the game, allowing transitions between different states
// and checking conditions for starting, ending, and winning the game.
// It follows the Singleton pattern to ensure only one instance exists throughout the game lifecycle.
public class GameStateManager
{
    private static GameStateManager instance;

    public static GameStateManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameStateManager();
            }
            return instance;
        }
    }

    private GameState currentState;

    public GameState CurrentState
    {
        get => currentState;
        private set => currentState = value;
    }

    // Private constructor to prevent direct instantiation
    private GameStateManager()
    {
        currentState = GameState.NotStarted;  // Initial state
    }

    // Start the game
    public void StartGame()
    {
        if (currentState == GameState.NotStarted)
        {
            currentState = GameState.InProgress;
            Console.WriteLine("Game Started!");
        }
    }

    // End the game
    public void EndGame(bool isVictory)
    {
        currentState = GameState.GameOver;
        if (isVictory)
        {
            Console.WriteLine("Victory! You won!");
        }
        else
        {
            Console.WriteLine("Game Over! You lost.");
        }
    }

    // Check win condition
    public void CheckWinCondition(int playerHP)
    {
        if (playerHP <= 0)
        {
            EndGame(false);  // Player lost
        }
        // Other win conditions can be added
    }

    // Check if game is in progress
    public bool IsGameInProgress()
    {
        return currentState == GameState.InProgress;
    }
}

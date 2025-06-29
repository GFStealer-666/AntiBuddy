using UnityEngine;
using System;
public class TurnManager : MonoBehaviour
{
    private GameStateManager gameStateManager;
    private PlayerManager playerManager;
    private PathogenManager pathogenManager;

    private bool isPlayerTurn;

    void Start()
    {
        gameStateManager = GameStateManager.Instance;
        playerManager = new PlayerManager(new Player(100)); // Example player setup
        pathogenManager = new PathogenManager();

        isPlayerTurn = true;  // Player starts first
    }

    void Update()
    {
        if (gameStateManager.IsGameInProgress())
        {
            if (isPlayerTurn)
            {
                PlayerTurn();
            }
            else
            {
                PathogenTurn();
            }
        }
    }

    // Player's Turn Logic
    void PlayerTurn()
    {
        // 1. Draw Card(s)
        // For simplicity, we assume card drawing is handled in PlayerController or UIManager

        // 2. Handle Player's Actions: e.g., Use Card, Attack, Heal
        // Example actions triggered from buttons or UI (via PlayerManager)
        if (Input.GetKeyDown(KeyCode.Space))  // Example attack input
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.H))  // Example heal input
        {
            Heal();
        }

        // 3. End Player's Turn and switch to pathogen turn
        if (Input.GetKeyDown(KeyCode.Return))  // End turn when 'Enter' is pressed
        {
            EndPlayerTurn();
        }
    }

    // Handle the player's attack
    void Attack()
    {
        // Example: Attack the first pathogen
        UpdateUI();
    }

    // Handle player healing
    void Heal()
    {
        playerManager.HealPlayer(20);  // Heal 20 HP
        UpdateUI();
    }

    // End Player's Turn and Switch to Pathogen's Turn
    void EndPlayerTurn()
    {
        Debug.Log("Player's Turn Ended");
        isPlayerTurn = false;  // Switch to pathogen's turn
        CheckWinConditions();  // Check if player has won
    }

    // Pathogen's Turn Logic
    void PathogenTurn()
    {
        // // 1. Pathogen Attacks Player
        // pathogenManager.PathogenAttack(playerManager.GetPlayer());

        // // 2. Check if Player is Defeated
        // if (playerManager.GetPlayer().HP <= 0)
        // {
        //     gameStateManager.EndGame(false);  // Player loses
        // }

        // // 3. End Pathogen's Turn and Switch to Player's Turn
        // EndPathogenTurn();
    }

    // End Pathogen's Turn and start next player's turn
    void EndPathogenTurn()
    {
        Debug.Log("Pathogen's Turn Ended");
        isPlayerTurn = true;  // Switch back to player turn
        CheckWinConditions();  // Check if pathogens are defeated
    }

    // Check if the player has won or lost
    void CheckWinConditions()
    {
        // If there are no pathogens left, the player wins
        if (pathogenManager.AllPathogensDefeated())
        {
            gameStateManager.EndGame(true);  // Player wins
        }
    }

    // Update the UI (e.g., health, tokens, pathogen status)
    void UpdateUI()
    {
        // You can update the UI with the latest player stats, cards, etc.
    }
}

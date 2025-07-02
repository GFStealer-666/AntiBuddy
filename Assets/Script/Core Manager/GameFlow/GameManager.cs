using UnityEngine;
public class GameManager : MonoBehaviour
{
    private GameStateManager gameStateManager;
    private PlayerManager playerManager;
    private DeckManager deckManager;

    private Player player;

    void Start()
    {
        // Create GameStateManager (Singleton)
        gameStateManager = GameStateManager.Instance;

        // Initialize Player and PlayerManager
        player = new Player(100);
        playerManager = new PlayerManager(player);

        // Initialize DeckManager and add some cards
        deckManager = new DeckManager();


        // Start the game
        gameStateManager.StartGame();

        // Example of drawing a card and using it
        playerManager.DrawCards(5);
    }
}

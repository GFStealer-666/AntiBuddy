using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// UI component for displaying cards in the field area.
/// Cards in the field are completely non-interactive - no clicking, no hover effects.
/// They serve as a visual reminder of what cards the player has in play.
/// </summary>
public class CardFieldUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform fieldContainer;
    public GameObject cardUIPrefab; // The same CardUI prefab used in hand, but non-clickable
    public CardField cardField;

    [Header("Field Settings")]
    public float cardSpacing = 10f;

    private List<GameObject> cardUIObjects = new List<GameObject>();

    void Awake()
    {
        if (cardField == null)
            cardField = FindFirstObjectByType<CardField>();
    }
    private void OnEnable()
    {
        CardField.OnFieldChanged += UpdateFieldDisplay;
    }

    void OnDestroy()
    {
        CardField.OnFieldChanged -= UpdateFieldDisplay;
    }

    void UpdateFieldDisplay(List<CardSO> cardsInField)
    {
        if (fieldContainer == null || cardUIPrefab == null) return;

        // Clear existing card UIs
        ClearFieldUI();

        // Create CardUI for each card in field
        for (int i = 0; i < cardsInField.Count; i++)
        {
            CreateCardUIInField(cardsInField[i], i);
        }

        Debug.Log($"CardFieldUI: Updated display with {cardsInField.Count} cards");
    }

    void ClearFieldUI()
    {
        foreach (var cardObj in cardUIObjects)
        {
            if (cardObj != null)
                Destroy(cardObj);
        }
        cardUIObjects.Clear();
    }

    void CreateCardUIInField(CardSO card, int index)
    {
        if (card == null) return;

        GameObject cardUIObj = Instantiate(cardUIPrefab, fieldContainer);
        CardUI cardUI = cardUIObj.GetComponent<CardUI>();

        if (cardUI != null)
        {
            // Initialize the card UI but make it completely non-interactive
            cardUI.Initialize(card);
            
            // Disable the button completely to prevent clicking
            Button cardButton = cardUIObj.GetComponent<Button>();
            if (cardButton != null)
            {
                cardButton.interactable = false;
                cardButton.enabled = false; // Completely disable button component
            }
            
            // Remove all pointer event handlers to prevent hover effects
            var pointerHandlers = cardUIObj.GetComponents<MonoBehaviour>();
            foreach (var handler in pointerHandlers)
            {
                if (handler is IPointerEnterHandler || handler is IPointerExitHandler || handler is IPointerClickHandler)
                {
                    (handler as MonoBehaviour).enabled = false;
                }
            }
            
            // Set the card as blocked to prevent any hover effects in CardUI logic
            cardUI.SetBlocked(true);
            
            // Position the card (optional - layout group can handle this too)
            RectTransform cardRect = cardUIObj.GetComponent<RectTransform>();
            if (cardRect != null)
            {
                cardRect.anchoredPosition = new Vector2(index * cardSpacing, 0);
            }

            cardUIObjects.Add(cardUIObj);
            Debug.Log($"CardFieldUI: Created non-interactive UI for {card.cardName} in field position {index}");
        }
        else
        {
            Debug.LogError("CardFieldUI: CardUI prefab missing CardUI component!");
            Destroy(cardUIObj);
        }
    }
}

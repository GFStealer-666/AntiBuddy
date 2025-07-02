using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CardFieldUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform fieldContainer;
    public GameObject cardSlotPrefab;
    public CardField cardField;

    [Header("Slot Visuals")]
    public Color emptySlotColor = Color.gray;
    public Color occupiedSlotColor = Color.white;

    private List<GameObject> slotUIObjects = new List<GameObject>();

    void Start()
    {
        if (cardField == null)
            cardField = FindFirstObjectByType<CardField>();

        // Subscribe to field changes
        CardField.OnFieldChanged += UpdateFieldDisplay;

        InitializeFieldUI();
    }

    void OnDestroy()
    {
        CardField.OnFieldChanged -= UpdateFieldDisplay;
    }

    void InitializeFieldUI()
    {
        if (fieldContainer == null || cardSlotPrefab == null) return;

        // Clear existing UI
        foreach (var obj in slotUIObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        slotUIObjects.Clear();

        // Create slot UI objects
        var fieldSlots = cardField.GetFieldSlots();
        for (int i = 0; i < fieldSlots.Count; i++)
        {
            GameObject slotObj = Instantiate(cardSlotPrefab, fieldContainer);
            slotUIObjects.Add(slotObj);
            
            // Set up slot visual
            UpdateSlotVisual(slotObj, fieldSlots[i]);
        }
    }

    void UpdateFieldDisplay(List<CardSO> activeCards)
    {
        if (cardField == null) return;

        var fieldSlots = cardField.GetFieldSlots();
        
        for (int i = 0; i < fieldSlots.Count && i < slotUIObjects.Count; i++)
        {
            UpdateSlotVisual(slotUIObjects[i], fieldSlots[i]);
        }
    }

    void UpdateSlotVisual(GameObject slotObj, CardFieldSlot slot)
    {
        if (slotObj == null) return;

        // Update slot background color
        Image slotImage = slotObj.GetComponent<Image>();
        if (slotImage != null)
        {
            slotImage.color = slot.isOccupied ? occupiedSlotColor : emptySlotColor;
        }

        // Update card text/icon
        TextMeshProUGUI slotText = slotObj.GetComponentInChildren<TextMeshProUGUI>();
        if (slotText != null)
        {
            if (slot.isOccupied && slot.card != null)
            {
                slotText.text = slot.card.cardName;
            }
            else
            {
                slotText.text = "Empty Slot";
            }
        }

        // Update card icon if available
        Image cardIcon = slotObj.transform.Find("CardIcon")?.GetComponent<Image>();
        if (cardIcon != null)
        {
            if (slot.isOccupied && slot.card != null && slot.card.cardIcon != null)
            {
                cardIcon.sprite = slot.card.cardIcon;
                cardIcon.gameObject.SetActive(true);
            }
            else
            {
                cardIcon.gameObject.SetActive(false);
            }
        }
    }

    public void HighlightAvailableSlots(bool highlight)
    {
        var fieldSlots = cardField.GetFieldSlots();
        
        for (int i = 0; i < fieldSlots.Count && i < slotUIObjects.Count; i++)
        {
            if (!fieldSlots[i].isOccupied)
            {
                Image slotImage = slotUIObjects[i].GetComponent<Image>();
                if (slotImage != null)
                {
                    slotImage.color = highlight ? Color.green : emptySlotColor;
                }
            }
        }
    }
}

// using UnityEngine;
// using UnityEditor;

// [CustomEditor(typeof(DeckManager))]
// public class DeckManagerEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         // Draw the default inspector
//         DrawDefaultInspector();
        
//         DeckManager deckManager = (DeckManager)target;
        
//         // Add space before buttons
//         EditorGUILayout.Space(10);
        
//         // Header for weighted card management
//         EditorGUILayout.LabelField("Weighted Card Management", EditorStyles.boldLabel);
        
//         // Buttons for weighted card management
//         EditorGUILayout.BeginHorizontal();
        
//         if (GUILayout.Button("Add All Base Cards"))
//         {
//             deckManager.AddAllBaseCardsToWeightedList();
//         }
        
//         if (GUILayout.Button("Add Missing Cards"))
//         {
//             deckManager.AddMissingBaseCardsToWeightedList();
//         }
        
//         EditorGUILayout.EndHorizontal();
        
//         EditorGUILayout.BeginHorizontal();
        
//         if (GUILayout.Button("Auto-Balance"))
//         {
//             deckManager.AutoBalanceWeightedCards();
//         }
        
//         if (GUILayout.Button("Random Weights"))
//         {
//             deckManager.RandomWeightedCards();
//         }
        
//         EditorGUILayout.EndHorizontal();
        
//         EditorGUILayout.BeginHorizontal();
        
//         if (GUILayout.Button("Validate Weights"))
//         {
//             deckManager.ValidateWeightedCards();
//         }
        
//         EditorGUILayout.EndHorizontal();
        
//         // Individual card management section
//         EditorGUILayout.Space(10);
//         EditorGUILayout.LabelField("Individual Card Management", EditorStyles.boldLabel);
        
//         if (deckManager.baseCards != null && deckManager.baseCards.Count > 0)
//         {
//             EditorGUILayout.LabelField("Add individual cards from base cards list:", EditorStyles.miniLabel);
            
//             // Create buttons for each base card
//             int buttonsPerRow = 2;
//             int currentButton = 0;
            
//             foreach (var baseCard in deckManager.baseCards)
//             {
//                 if (baseCard == null) continue;
                
//                 // Start new row if needed
//                 if (currentButton % buttonsPerRow == 0)
//                 {
//                     if (currentButton > 0) EditorGUILayout.EndHorizontal();
//                     EditorGUILayout.BeginHorizontal();
//                 }
                
//                 // Check if card is already in weighted list
//                 bool isInList = deckManager.IsCardInWeightedList(baseCard);
                
//                 // Change button color if already added
//                 Color originalColor = GUI.backgroundColor;
//                 if (isInList)
//                 {
//                     GUI.backgroundColor = Color.green;
//                 }
                
//                 string buttonText = isInList ? $"✓ {baseCard.cardName}" : $"+ {baseCard.cardName}";
                
//                 if (GUILayout.Button(buttonText))
//                 {
//                     if (isInList)
//                     {
//                         // Remove if already in list
//                         deckManager.RemoveCardFromWeightedList(baseCard);
//                     }
//                     else
//                     {
//                         // Add if not in list
//                         deckManager.AddSpecificCardToWeightedList(baseCard, 0.2f);
//                     }
//                 }
                
//                 // Restore original color
//                 GUI.backgroundColor = originalColor;
                
//                 currentButton++;
//             }
            
//             // End the last row if it was started
//             if (currentButton > 0)
//             {
//                 EditorGUILayout.EndHorizontal();
//             }
//         }
//         else
//         {
//             EditorGUILayout.HelpBox("No base cards available. Add cards to the base cards list first.", MessageType.Warning);
//         }
        
//         // Add space and info
//         EditorGUILayout.Space(5);
        
//         // Show current status
//         if (deckManager.useWeightedGeneration)
//         {
//             EditorGUILayout.HelpBox("Using weighted generation. Make sure percentages add up to 100%!", MessageType.Info);
            
//             // Show quick stats
//             if (deckManager.weightedCards != null && deckManager.weightedCards.Count > 0)
//             {
//                 float totalPercentage = 0f;
//                 foreach (var weightedCard in deckManager.weightedCards)
//                 {
//                     totalPercentage += weightedCard.weightPercentage;
//                 }
                
//                 string statusText = $"Weighted Cards: {deckManager.weightedCards.Count} | Total Weight: {totalPercentage:P1}";
//                 if (Mathf.Abs(totalPercentage - 1.0f) > 0.01f)
//                 {
//                     EditorGUILayout.HelpBox(statusText + " ⚠️ Not balanced!", MessageType.Warning);
//                 }
//                 else
//                 {
//                     EditorGUILayout.HelpBox(statusText + " ✅ Balanced!", MessageType.Info);
//                 }
//             }
//         }
//         else
//         {
//             EditorGUILayout.HelpBox("Using random generation. Enable weighted generation to use percentages.", MessageType.Info);
//         }
        
//         // Deck testing buttons
//         EditorGUILayout.Space(10);
//         EditorGUILayout.LabelField("Deck Testing", EditorStyles.boldLabel);
        
//         EditorGUILayout.BeginHorizontal();
        
//         if (GUILayout.Button("Generate Deck"))
//         {
//             if (Application.isPlaying)
//             {
//                 deckManager.RegenerateDeck();
//             }
//             else
//             {
//                 Debug.LogWarning("Deck generation only works in Play Mode!");
//             }
//         }
        
//         if (GUILayout.Button("Get Deck Stats"))
//         {
//             if (Application.isPlaying)
//             {
//                 var composition = deckManager.GetDeckComposition();
//                 Debug.Log("=== Current Deck Composition ===");
//                 foreach (var kvp in composition)
//                 {
//                     Debug.Log($"{kvp.Key}: {kvp.Value} cards");
//                 }
//                 Debug.Log($"Total cards: {deckManager.GetRemainingCards()}");
//             }
//             else
//             {
//                 Debug.LogWarning("Deck stats only available in Play Mode!");
//             }
//         }
        
//         EditorGUILayout.EndHorizontal();
//     }
// }

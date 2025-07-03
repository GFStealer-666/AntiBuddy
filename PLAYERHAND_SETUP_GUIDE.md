# PlayerHandCard System Setup Guide

## Overview
The PlayerHandCard system integrates DeckManager and Player scripts to provide a complete UI and interaction system for managing cards in the player's hand. This system handles card display, selection, drag/drop, targeting, and playing.

## Components Created

### 1. PlayerHandCard.cs
- **Purpose**: Represents individual cards in the player's hand with full UI interaction
- **Features**: 
  - Card display with name, description, and image
  - Hover effects and selection highlighting
  - Drag and drop for targeting pathogens
  - Click to select/play cards
  - Tooltip integration
  - Pathogen blocking validation

### 2. PlayerHand.cs
- **Purpose**: Manages the collection of cards in the player's hand
- **Features**:
  - Integration with DeckManager for drawing cards
  - Flexible layout system (linear or arc)
  - Card selection management
  - Turn-based card drawing
  - Hand size management
  - Animation support

### 3. PathogenUI.cs
- **Purpose**: UI representation of pathogens that can be targeted by cards
- **Features**:
  - Display pathogen stats (HP, attack, name)
  - Health bar visualization
  - Targeting highlights
  - Damage visual effects

### 4. TooltipManager.cs
- **Purpose**: Displays detailed information when hovering over cards or pathogens
- **Features**:
  - Card tooltips with full details
  - Pathogen information display
  - Mouse following or fixed positioning
  - Configurable delays

### 5. GameManager.cs
- **Purpose**: Coordinates game state and card blocking system
- **Features**:
  - Pathogen management
  - Card blocking validation
  - Turn management
  - Game state tracking

## Setup Instructions

### 1. Create Card Prefab
1. Create a UI GameObject in your Canvas
2. Add the following components:
   - `PlayerHandCard` script
   - `Image` for card background
   - `TextMeshProUGUI` for card name
   - `TextMeshProUGUI` for card description
   - `Image` for card artwork
   - `CanvasGroup` for alpha control
   - `GameObject` for selection highlight (child object)

### 2. Create Hand Container
1. Create a UI GameObject for the hand container
2. Add `PlayerHand` script
3. Assign references:
   - Card Prefab (from step 1)
   - Hand Container Transform
   - Player reference
   - DeckManager reference

### 3. Setup Layout
Configure layout settings in PlayerHand:
- **Linear Layout**: Use Unity's Layout Groups
- **Arc Layout**: Enable `useArcLayout` and configure arc parameters

### 4. Create Pathogen UI
1. Create UI GameObject for each pathogen
2. Add `PathogenUI` script
3. Add UI elements:
   - Pathogen image
   - Health bar (Slider)
   - Name and stat text
   - Target highlight effect

### 5. Setup Tooltip System
1. Create tooltip panel UI
2. Add `TooltipManager` script
3. Configure tooltip elements:
   - Title text
   - Description text
   - Card image
   - Panel background

### 6. Initialize Game Manager
1. Add `GameManager` script to a GameObject
2. Assign references to Player, PlayerHand, DeckManager
3. Configure pathogen list

## Integration with Existing Systems

### DeckManager Integration
```csharp
// PlayerHand automatically draws cards from DeckManager
CardSO drawnCard = deckManager.DrawCard();
playerHand.AddCardToHand(drawnCard);
```

### Player Integration
```csharp
// Cards are automatically added to Player.Hand when drawn
// Playing cards removes them from both UI and Player.Hand
bool success = player.PlayCard(cardData, target);
```

### Pathogen Blocking System
```csharp
// Cards check if they're blocked before allowing play
bool isBlocked = gameManager.IsCardBlocked(cardType);
```

## Events System

### PlayerHand Events
- `OnCardDrawn`: Fired when a card is drawn
- `OnCardPlayed`: Fired when a card is played
- `OnCardSelected`: Fired when a card is selected
- `OnHandFull`: Fired when hand reaches max size
- `OnHandEmpty`: Fired when hand is empty

### PlayerHandCard Events
- `OnCardSelected`: Fired when this card is selected
- `OnCardPlayed`: Fired when this card is played
- `OnCardPlayedOnTarget`: Fired when played on a specific target

### Usage Example
```csharp
playerHand.OnCardPlayed += (card) => {
    Debug.Log($"Player played {card.cardName}");
};
```

## Configuration Options

### Hand Settings
- `maxHandSize`: Maximum cards in hand (default: 7)
- `startingHandSize`: Cards drawn at game start (default: 5)
- `cardsPerTurn`: Cards drawn each turn (default: 1)

### Layout Settings
- `cardSpacing`: Distance between cards
- `useArcLayout`: Use curved hand layout
- `arcRadius`: Radius of the arc
- `maxArcAngle`: Maximum angle span

### Visual Settings
- `hoverScale`: Scale multiplier on hover
- `animationDuration`: Speed of animations
- `normalColor`, `hoverColor`, `selectedColor`: Card state colors

## Best Practices

1. **Performance**: Pool card objects instead of creating/destroying them
2. **Accessibility**: Add keyboard navigation support
3. **Mobile**: Consider touch-friendly sizing and interactions
4. **Animation**: Use Unity's Tween libraries for smoother animations
5. **Error Handling**: Validate all references before use

## Troubleshooting

### Common Issues
1. **Cards not appearing**: Check card prefab setup and references
2. **Drag not working**: Ensure CanvasGroup and EventSystem are configured
3. **Layout issues**: Verify container RectTransform settings
4. **Blocking not working**: Check GameManager pathogen references

### Debug Tips
- Enable debug logs in each component
- Use Unity's UI Debugger for layout issues
- Test with different hand sizes and card counts
- Verify all component references are assigned

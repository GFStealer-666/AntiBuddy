# Immunity Boardgame - Project Architecture Summary

## 🏗️ Core Architecture

### Event-Driven Design
The project uses a robust event system where components communicate through C# events rather than direct references, ensuring:
- **Loose Coupling**: Components can be modified independently
- **Scalability**: Easy to add new features without breaking existing code
- **Debuggability**: Clear event flow makes issues easier to track

### Game Flow
```
GameManager (Central Hub)
├── TurnManager (Turn phases)
├── PlayerManager (Player actions)
├── PathogenManager (Enemy management)
├── ShopManager (Item purchasing)
└── GameLogUI (Event logging)
```

## 🎯 Key Systems

### 1. Game Logging System
**Primary Class**: `GameLogUI`
- **Purpose**: Comprehensive event tracking and display
- **Features**: 
  - Real-time on-screen log display
  - Persistent memory storage
  - JSON export capability
  - Categorized message types
  - Debug tools and statistics

### 2. Card & Item System
**Base Classes**: `CardSO` → `ItemSO`
- **Unified Approach**: Items inherit from cards for consistent handling
- **Event Integration**: All cards/items log their effects
- **Types**:
  - **Immune Cards**: Macrophage, B-Cell, Cytotoxic, Natural Killer, Helper T-Cell
  - **Consumable Items**: Vitamin, Wash Hands, Wear Mask, Adrenaline, Cytokine, Immunostimulant

### 3. Player Management
**Primary Classes**: `PlayerManager`, `PlayerCards`, `PlayerHandUI`
- **Hand Management**: Real-time card tracking and display
- **Event Broadcasting**: All player actions trigger appropriate events
- **Debug Support**: Inspector tools for debugging hand state

### 4. Turn System
**Primary Class**: `TurnManager`
- **Phase Management**: Clear turn phases (Player, Pathogen, GameOver)
- **Event Broadcasting**: Phase changes notify all interested systems
- **Integration**: Works seamlessly with logging and UI systems

## 📊 Data Flow

### Typical Game Action Flow:
1. **Player Action** → `PlayerManager`
2. **Event Fired** → All subscribed systems notified
3. **GameLogUI** → Logs the action with appropriate category
4. **UI Updates** → Hand/display updates automatically
5. **Game State** → Updated through event system

### Log Message Flow:
1. **Action Occurs** → Game system calls `GameManager.Log___`
2. **GameLogUI** → Creates `LogMessage` with metadata
3. **Persistent Storage** → Message added to permanent list
4. **Display Queue** → Message queued for UI display
5. **Visual Update** → Message appears on screen with formatting

## 🎨 UI Architecture

### Log Display System
- **GameLogUI**: Main logging controller and display manager
- **GameLogEntry**: Individual log entry component (for prefab-based display)
- **Color Coding**: Different colors for different action types
- **Auto-scrolling**: Always shows latest messages
- **Memory Management**: Configurable limits to prevent memory issues

### Player Interface
- **PlayerHandUI**: Real-time hand display
- **CardUI/CardFieldUI**: Individual card display and interaction
- **ShopDisplayUI**: Item purchasing interface

## 🔧 Technical Features

### Performance Optimizations
- **Message Queuing**: Prevents frame drops during heavy logging
- **Memory Management**: Automatic cleanup of old log entries
- **Event Unsubscription**: Proper cleanup prevents memory leaks
- **Coroutine Processing**: Smooth UI updates without blocking

### Debug & Development Tools
- **Context Menu Commands**: Quick testing and log management
- **Inspector Integration**: Real-time debugging in Unity Inspector
- **Statistics Tracking**: Live counts of different log categories
- **Export Functionality**: JSON export for external analysis

## 🚀 Extensibility Points

### Easy to Add:
1. **New Card Types**: Inherit from `CardSO`, implement effect, add logging
2. **New Events**: Create event in appropriate manager, add GameLogUI handler
3. **New Log Categories**: Add to `LogCategory` enum, update color scheme
4. **UI Enhancements**: Log display system supports rich formatting

### Future Development Ready:
- **File I/O**: JSON structure complete, just needs file system calls
- **Analytics**: All data collected and categorized for analysis
- **Replay System**: Event log provides complete game history
- **Multiplayer**: Event system architecture supports network events

## 📋 Development Guidelines

### Adding New Features:
1. **Create Event**: Add event to appropriate manager
2. **Add Logging**: Implement logging in GameManager
3. **Update UI**: Subscribe to events in UI components
4. **Test**: Use debug tools to verify integration

### Code Standards:
- **Events**: Use C# events for inter-component communication
- **Logging**: All significant actions should be logged
- **Cleanup**: Always unsubscribe from events in OnDestroy
- **Categories**: Use appropriate LogCategory for each message type

This architecture provides a solid foundation for continued development while maintaining code quality and extensibility.

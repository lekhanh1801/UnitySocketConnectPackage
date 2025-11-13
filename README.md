# WebSocket Client for Unity

A robust, easy-to-use WebSocket client library for Unity game development.

## Features

- 🚀 Easy connection management
- 📡 Automatic reconnection support
- 🎮 Thread-safe for Unity
- 📦 JSON serialization built-in
- 🔄 Event-driven architecture

## Installation

### Method 1: Unity Package Manager
1. Open Package Manager
2. Click "+" → "Add package from git URL"
3. Enter: `https://github.com/yourusername/websocket-client-unity.git`

### Method 2: Manual
1. Download `.unitypackage`
2. Double-click to import

## Quick Start

```csharp
public class MyGameManager : MonoBehaviour
{
    private GameWebSocketClient client;
    
    private void Start()
    {
        client = new GameWebSocketClient("ws://yourserver.com:8080");
        client.OnMessageReceived += HandleMessage;
        client.ConnectAsync();
    }
    
    private void HandleMessage(string message)
    {
        Debug.Log($"Received: {message}");
    }
}
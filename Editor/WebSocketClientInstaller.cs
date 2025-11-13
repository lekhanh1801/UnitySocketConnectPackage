using UnityEditor;
using UnityEngine;
using System.IO;

namespace WebSocketClientPackage.Editor
{
    public class WebSocketClientInstaller : EditorWindow
    {
        [MenuItem("Tools/WebSocket Client/Installation Guide")]
        public static void ShowWindow()
        {
            GetWindow<WebSocketClientInstaller>("WebSocket Client Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("WebSocket Client Package", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            GUILayout.Label("Setup Instructions:", EditorStyles.label);
            GUILayout.Label("1. Make sure you're using .NET 4.x in Player Settings");
            GUILayout.Label("2. Add server URL in your GameClientManager component");
            GUILayout.Label("3. Check Samples for example usage");
            
            GUILayout.Space(20);
            
            if (GUILayout.Button("Open Player Settings"))
            {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");
            }
            
            if (GUILayout.Button("Open Sample Scene"))
            {
                OpenSampleScene();
            }
        }
        
        private void OpenSampleScene()
        {
            string sampleScenePath = "Packages/com.yourcompany.websocket.client/Samples/ExampleScene/Scenes/WebSocketExample.unity";
            if (File.Exists(sampleScenePath))
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(sampleScenePath);
            }
            else
            {
                Debug.LogWarning("Sample scene not found at: " + sampleScenePath);
            }
        }
    }

    [InitializeOnLoad]
    public class WebSocketWelcome
    {
        static WebSocketWelcome()
        {
            EditorApplication.delayCall += ShowWelcome;
        }

        static void ShowWelcome()
        {
            if (!SessionState.GetBool("WEBSOCKET_WELCOME_SHOWN", false))
            {
                SessionState.SetBool("WEBSOCKET_WELCOME_SHOWN", true);
                Debug.Log("🎮 WebSocket Client Package loaded! Go to Tools/WebSocket Client for setup guide.");
            }
        }
    }
}
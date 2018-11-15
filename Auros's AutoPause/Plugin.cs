using BeatSaberUI;
using IllusionPlugin;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AurosAutoPause
{
    public class Plugin : IPlugin
    {
        public string Name => "Auros's AutoPause";
        public string Version => "1.4.0";

        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            System.Console.WriteLine("[AutoPause] SCENE NAME: " + scene.name);
            if (scene.name == "Menu")
            {
                PauseUI.CreateSettingsUI();
            }
            else new GameObject("Auros's AutoPause").AddComponent<Pauser>();
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        public void OnLevelWasLoaded(int level)
        {
        }

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }

        public void OnLevelWasInitialized(int level)
        {
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadTrigger : MonoBehaviour
{
    [SerializeField] private SceneField[] scenesToLoad;
    [SerializeField] private SceneField[] scenesToUnload;

    private GameObject player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == player)
        {
            LoadScenes();
            UnloadScenes();
        }
    }

    private void LoadScenes()
    {
        for (int i = 0; i < scenesToLoad.Length; i++)
        {
            bool isSceneLoaded = false;
            for (int j = 0; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if (loadedScene.name == scenesToLoad[i].SceneName)
                {
                    isSceneLoaded = true;
                    break;
                }
            }

            if (!isSceneLoaded)
            {
                SceneManager.LoadSceneAsync(scenesToLoad[i], LoadSceneMode.Additive);
            }
        }
    }

    private void UnloadScenes()
    {
        for (int i = 0; i < scenesToUnload.Length; i++)
        {
            for (int j = 0; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if (loadedScene.name == scenesToLoad[i].SceneName)
                {
                    SceneManager.UnloadSceneAsync(scenesToUnload[i]);
                }
            }
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;


public class AsyncLoader : MonoBehaviour
{
    private const string LOAD_SCENE_NAME = "LoadScreen";
    private static string _sceneName;


    private void Start()
    {
        SceneManager.LoadSceneAsync(_sceneName);
    }



    public static void LoadScene(string sceneName)
    {
        _sceneName = sceneName;
        SceneManager.LoadScene(LOAD_SCENE_NAME);
    }
}